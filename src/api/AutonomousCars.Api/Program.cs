namespace AutonomousCars.Api;

using AutonomousCars.Api.Models.Options;
using AutonomousCars.Api.Weather.Services;

using Azure.Core;
using Azure.Identity;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers();

        builder.Services.AddTransient<IWeatherService, WeatherService>();

        // Options
        builder.Services.Configure<AzureMapsOptions>(builder.Configuration.GetSection("AzureMaps"));

        // Token credential
        builder.Services.AddSingleton<TokenCredential>((serviceProvider) =>
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

        app.Run();
    }
}