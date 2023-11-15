import * as resources from "@pulumi/azure-native/resources";
import * as pulumi from "@pulumi/pulumi";
import * as azure_native from "@pulumi/azure-native";

// Create an Azure Resource Group
const resourceGroup = new resources.ResourceGroup("rg-autonomouscars", {
    location:"westeurope", 
    resourceGroupName:"rg-autonomouscars"
});

// Create a Static Web App
const staticWebApp = new azure_native.web.StaticSite("stapp-autonomouscars", {
    location: resourceGroup.location,
    resourceGroupName: resourceGroup.name,
    repositoryUrl: "https://github.com/Mentoring-Bordeaux/AutonomousCars", // Entrez ici votre propre URL
    branch: "main", // Branche du dépôt à utiliser
    sku: {
        name: "Standard",
        tier: "Standard",
    }
});

// Export the URL of the Static Web App
export const appUrl = staticWebApp.defaultHostname;


