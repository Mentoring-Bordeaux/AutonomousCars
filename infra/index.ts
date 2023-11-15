import * as resources from "@pulumi/azure-native/resources";
import * as azure from "@pulumi/azure";
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
    repositoryUrl: "https://github.com/Mentoring-Bordeaux/AutonomousCars",
    branch: "main", 
    sku: {
        name: "Standard",
        tier: "Standard",
    }
});

// Export the URL of the Static Web App
export const appUrl = staticWebApp.defaultHostname;

// Create an Azure Maps Account
const account = new azure_native.maps.Account("maps-autonomouscars", {
    accountName: "maps-autonomouscars",
    kind: "Gen2",
    location: resourceGroup.location,
    resourceGroupName: resourceGroup.name,
    sku: {
        name: "G2",
    },
    tags: {
        test: "true",
    },
});

// Create an App Service for the API
const webApp = new azure_native.web.WebApp("app-autonomouscars", {
    kind: "app",
    location: resourceGroup.location,
    resourceGroupName: resourceGroup.name,
});