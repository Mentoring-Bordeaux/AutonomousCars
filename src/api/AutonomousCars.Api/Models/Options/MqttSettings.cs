namespace AutonomousCars.Api.Models.Options;

using MQTTnet.Client.Extensions;
using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Text.RegularExpressions;
using System.IO;

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
                ExtractPrivateKeyAndCertificate(keyVaultSecret.Value);
            }

            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string privateKeyPath = Path.Combine(directoryPath, "back.key");
            string certificatePath = Path.Combine(directoryPath, "back.pem");

            MqttConnectionSettings = MqttConnectionSettings.CreateFromValues(
                backMqttHostNameSecret.Value.Value,
                backMqttUsernameSecret.Value.Value,
                backClientIdSecret.Value.Value,
                certificatePath,
                privateKeyPath,
                bool.Parse(backMqttCleanSessionSecret.Value.Value),
                bool.Parse(backMqttUseTlsSecret.Value.Value),
                int.Parse(backMqttTcpPortSecret.Value.Value));
        }
    }
    
    private static void ExtractPrivateKeyAndCertificate(string input)
    {
        Regex privateKeyRegex = new Regex(@"(-----BEGIN PRIVATE KEY-----.*?-----END PRIVATE KEY-----)", RegexOptions.Singleline);
        Regex certificateRegex = new Regex(@"(-----BEGIN CERTIFICATE-----.*?-----END CERTIFICATE-----)", RegexOptions.Singleline);

        Match privateKeyMatch = privateKeyRegex.Match(input);
        string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        if (privateKeyMatch.Success)
        {
            string privateKeyWithHeader = privateKeyMatch.Groups[1].Value.Trim();
            File.WriteAllText(directoryPath+"back.key", privateKeyWithHeader);
        }
        
        Match certificateMatch = certificateRegex.Match(input);
        if (certificateMatch.Success)
        {
            string certificateWithHeader = certificateMatch.Groups[1].Value.Trim();
            File.WriteAllText(directoryPath+"back.pem", certificateWithHeader);
        }
    }
}
