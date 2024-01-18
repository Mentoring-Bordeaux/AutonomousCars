using AutonomousCars.Api.Itinerary.Services;
using AutonomousCars.Api.Models.Exceptions;

namespace AutonomousCars.Api;

using Models.Options;
using Weather.Services;
using Device.Services;

using Azure.Core;
using Azure.Identity;

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

        // Options
        builder.Services.Configure<AzureMapsOptions>(builder.Configuration.GetSection("AzureMaps"));
        builder.Services.Configure<MqttNamespaceOptions>(builder.Configuration.GetSection("KeyVault"));

        // Token credential
        builder.Services.AddSingleton<TokenCredential>(_ =>
            builder.Environment.IsDevelopment()
                ? new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                {   
                    ExcludeInteractiveBrowserCredential = false,
                })
                : new ManagedIdentityCredential());

        var keyVaultOptions = builder.Configuration.GetSection("KeyVault").Get<KeyVaultOptions>();
        
        if (keyVaultOptions != null)
        {
           await MqttSettings.InitMqttSettings(keyVaultOptions);
        }
        else
        {
            throw new MissingSettingException($"{nameof(keyVaultOptions)}.{nameof(keyVaultOptions.KeyVaultName)}");
        }
        ;
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();

        app.Run();
    }
}