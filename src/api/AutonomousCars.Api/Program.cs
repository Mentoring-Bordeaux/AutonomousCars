using Microsoft.Extensions.Azure;

namespace AutonomousCars.Api;

using Itinerary.Services;
using Models.Options;
using Weather.Services;
using Device.Services;

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;


public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        builder.Services.AddTransient<IWeatherService, WeatherService>();
        builder.Services.AddTransient<IItineraryService, AzureMapsItineraryService>();
        builder.Services.AddTransient<IMqttDevices, MqttDevices>();
        builder.Services.AddSingleton<MqttSettings>();
        
        builder.Services.Configure<MqttConfiguration>(builder.Configuration.GetSection("MqttNamespace"));
        
        var vaultUri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net");
        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddSecretClient(vaultUri);
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
        builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
        
        // Token credential
        builder.Services.AddSingleton<TokenCredential>(_ =>
            builder.Environment.IsDevelopment()
                ? new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                {   
                    ExcludeInteractiveBrowserCredential = false,
                })
                : new ManagedIdentityCredential());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();
        var mqttSettings =  app.Services.GetService<MqttSettings>();
        if (mqttSettings != null) await mqttSettings.InitMqttSettings();
        app.Run();
    }
}