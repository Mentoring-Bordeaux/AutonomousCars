namespace AutonomousCars.Api.Models.Exceptions;

public class CertificationException(string settingName) : InvalidOperationException($"The required setting '{settingName}' is missing.");