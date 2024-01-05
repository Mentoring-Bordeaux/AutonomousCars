namespace AutonomousCars.Api.Models.Exceptions;

public class MissingSettingException(string settingName) : InvalidOperationException($"The required setting '{settingName}' is missing.")
{
}