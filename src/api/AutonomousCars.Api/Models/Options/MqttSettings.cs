namespace AutonomousCars.Api.Models.Options;

using Exceptions;
using Microsoft.Extensions.Options;
using MQTTnet.Client.Extensions;
using System;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;

public class MqttSettings
{
    private readonly SecretClient _secretClient;
    private readonly MqttConfiguration _mqttConfiguration;

    public MqttSettings(IOptions<MqttConfiguration> mqttConfiguration, SecretClient secretClient)
    {
        _secretClient = secretClient;
        _mqttConfiguration = mqttConfiguration.Value;
    }
    public MqttConnectionSettings? MqttConnectionSettings;

    public async Task InitMqttSettings()
    {
        var response = await _secretClient.GetSecretAsync("Back");
        var keyVaultSecret = response?.Value;
        if (keyVaultSecret != null)
        {
            var x509Certificate2 = ExtractPrivateKeyAndCertificate(keyVaultSecret.Value);
            if (x509Certificate2 == null)
            {
                throw new CertificationException("Failure to extract certificate from Azure Key Vault");
            }

            MqttConnectionSettings = MqttConnectionSettings.CreateFromValues(
                    _mqttConfiguration.Hostname,
                    _mqttConfiguration.Username,
                    _mqttConfiguration.ClientId,
                    _mqttConfiguration.CleanSession,
                    _mqttConfiguration.UseTLS,
                    _mqttConfiguration.TcpPort,
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
