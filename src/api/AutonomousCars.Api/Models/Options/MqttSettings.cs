using AutonomousCars.Api.Models.Exceptions;

namespace AutonomousCars.Api.Models.Options;

using MQTTnet.Client.Extensions;
using System;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;

public static class MqttSettings
{
    public static MqttConnectionSettings? MqttConnectionSettings;

    public static MqttNamespaceOptions? MqttNamespaceOptions { get; set; }
    
    public static async Task InitMqttSettings(SecretClient kvClient)
    {
        var backClientIdSecret = await kvClient.GetSecretAsync("BackClientId");
        var backMqttUsernameSecret = await kvClient.GetSecretAsync("BackMqttUsername");
        var backMqttCleanSessionSecret = await kvClient.GetSecretAsync("BackMqttCleanSession");
        var backMqttUseTlsSecret = await kvClient.GetSecretAsync("BackMqttUseTLS");
        var backMqttTcpPortSecret = await kvClient.GetSecretAsync("BackMqttTcpPort");
        var backMqttHostNameSecret = await kvClient.GetSecretAsync("BackMqttHostName");
        var backSubscriptionIdSecret = await kvClient.GetSecretAsync("BackSubscriptionId");
        var backResourceGroupNameSecret = await kvClient.GetSecretAsync("BackResourceGroupName");
        var backNamespaceSecret = await kvClient.GetSecretAsync("BackNamespace");
        MqttNamespaceOptions = new MqttNamespaceOptions(backSubscriptionIdSecret.Value.Value, backResourceGroupNameSecret.Value.Value, backNamespaceSecret.Value.Value);
        
        var response = await kvClient.GetSecretAsync("Back");
        var keyVaultSecret = response?.Value;
        if (keyVaultSecret != null)
        {
            var x509Certificate2 = ExtractPrivateKeyAndCertificate(keyVaultSecret.Value);
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
    
    private static X509Certificate2? ExtractPrivateKeyAndCertificate(string input)
    {
        var privateKeyRegex = new Regex(@"(-----BEGIN PRIVATE KEY-----.*?-----END PRIVATE KEY-----)", RegexOptions.Singleline);
        var certificateRegex = new Regex(@"(-----BEGIN CERTIFICATE-----.*?-----END CERTIFICATE-----)", RegexOptions.Singleline);

        var privateKeyMatch = privateKeyRegex.Match(input);
        var certificateMatch = certificateRegex.Match(input);
        if (!privateKeyMatch.Success && !certificateMatch.Success)
        {
            return null;
        }
        var privateKeyWithHeader = privateKeyMatch.Groups[1].Value.Trim();
        var certificateWithHeader = certificateMatch.Groups[1].Value.Trim();

        var privateKeyBytes = Encoding.UTF8.GetBytes(privateKeyWithHeader);
        var certificateBytes = Encoding.UTF8.GetBytes(certificateWithHeader);
   
        var certificateSpan = Encoding.UTF8.GetString(certificateBytes).AsSpan();
        var privateKeySpan = Encoding.UTF8.GetString(privateKeyBytes).AsSpan();

        var cert = X509Certificate2.CreateFromPem(certificateSpan, privateKeySpan);

        return cert;
    }
}
