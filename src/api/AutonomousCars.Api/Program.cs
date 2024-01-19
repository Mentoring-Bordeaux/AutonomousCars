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

        // Token credential
        builder.Services.AddSingleton<TokenCredential>(_ =>
            builder.Environment.IsDevelopment()
                ? new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                {   
                    ExcludeInteractiveBrowserCredential = false,
                })
                : new ManagedIdentityCredential());

        var kvUri = new Uri("https://kv-autonomouscars.vault.azure.net");
        var secretClient = new SecretClient(kvUri, new DefaultAzureCredential());

        await MqttSettings.InitMqttSettings(secretClient);
        
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