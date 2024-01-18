using AutonomousCars.Api.Models.Exceptions;

namespace AutonomousCars.Api.Models.Options;

using MQTTnet.Client.Extensions;
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public static class MqttSettings
{
    public static MqttConnectionSettings? MqttConnectionSettings;

    public static MqttNamespaceOptions? MqttNamespaceOptions { get; set; }
    
    public static async Task InitMqttSettings(KeyVaultOptions? keyVaultOptions)
    {
        if (keyVaultOptions != null)
        {
            string keyVaultName = keyVaultOptions.KeyVaultName;
            var kvUri = "https://" + keyVaultName + ".vault.azure.net";

            var secretClient = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            var backClientIdSecret = await secretClient.GetSecretAsync("BackClientId");
            var backMqttUsernameSecret = await secretClient.GetSecretAsync("BackMqttUsername");
            var backMqttCleanSessionSecret = await secretClient.GetSecretAsync("BackMqttCleanSession");
            var backMqttUseTlsSecret = await secretClient.GetSecretAsync("BackMqttUseTLS");
            var backMqttTcpPortSecret = await secretClient.GetSecretAsync("BackMqttTcpPort");
            var backMqttHostNameSecret = await secretClient.GetSecretAsync("BackMqttHostName");
            var backSubscriptionIdSecret = await secretClient.GetSecretAsync("BackSubscriptionId");
            var backResourceGroupNameSecret = await secretClient.GetSecretAsync("BackResourceGroupName");
            var backNamespaceSecret = await secretClient.GetSecretAsync("BackNamespace");
            MqttNamespaceOptions = new MqttNamespaceOptions(backSubscriptionIdSecret.Value.Value, backResourceGroupNameSecret.Value.Value, backNamespaceSecret.Value.Value);
            
            var response = await secretClient.GetSecretAsync("Back");
            var keyVaultSecret = response?.Value;
            if (keyVaultSecret != null)
            {
                X509Certificate2? x509Certificate2 = ExtractPrivateKeyAndCertificate(keyVaultSecret.Value);
                if (x509Certificate2 == null)
                {
                    throw new CertificationException("Failure to extract certificate from Azure Key Vault");
                }

                MqttConnectionSettings = MqttConnectionSettings.CreateFromValues(
                        backMqttHostNameSecret.Value.Value,
                        backMqttUsernameSecret.Value.Value,
                        backClientIdSecret.Value.Value,
                        bool.Parse(backMqttCleanSessionSecret.Value.Value),
                        bool.Parse(backMqttUseTlsSecret.Value.Value),
                        int.Parse(backMqttTcpPortSecret.Value.Value),
                        x509Certificate2)
                    ;
            }
        }
    }
    
    private static X509Certificate2? ExtractPrivateKeyAndCertificate(string input)
    {
        Regex privateKeyRegex = new Regex(@"(-----BEGIN PRIVATE KEY-----.*?-----END PRIVATE KEY-----)", RegexOptions.Singleline);
        Regex certificateRegex = new Regex(@"(-----BEGIN CERTIFICATE-----.*?-----END CERTIFICATE-----)", RegexOptions.Singleline);

        Match privateKeyMatch = privateKeyRegex.Match(input);
        Match certificateMatch = certificateRegex.Match(input);
        if (!privateKeyMatch.Success && !certificateMatch.Success)
        {
            return null;
        }
        string privateKeyWithHeader = privateKeyMatch.Groups[1].Value.Trim();
        string certificateWithHeader = certificateMatch.Groups[1].Value.Trim();

        byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKeyWithHeader);
        byte[] certificateBytes = Encoding.UTF8.GetBytes(certificateWithHeader);
   
        var certificateSpan = Encoding.UTF8.GetString(certificateBytes).AsSpan();
        var privateKeySpan = Encoding.UTF8.GetString(privateKeyBytes).AsSpan();

        X509Certificate2 cert = X509Certificate2.CreateFromPem(certificateSpan, privateKeySpan);

        return cert;
    }
}
