import * as resources from "@pulumi/azure-native/resources";
import * as azure_native from "@pulumi/azure-native";
import { ManagedServiceIdentityType } from "@pulumi/azure-native/app";
import { PrincipalType } from "@pulumi/azure-native/authorization";

// Create an Azure Resource Group
const resourceGroup = new resources.ResourceGroup("rg-autonomouscars", {
    location: "westeurope",
    resourceGroupName: "rg-autonomouscars",
});

// Create a Static Web App
const staticWebApp = new azure_native.web.StaticSite("stapp-autonomouscars", {
    location: resourceGroup.location,
    resourceGroupName: resourceGroup.name,
    branch: "main",
    sku: {
        name: "Free",
        tier: "Free",
    },
});

// Export the URL of the Static Web App
export const appUrl = staticWebApp.defaultHostname;

// Create an Azure Maps Account
const mapsAccount = new azure_native.maps.Account("maps-autonomouscars", {
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
    identity: {
        type: ManagedServiceIdentityType.SystemAssigned,
    },
    siteConfig: {
        appSettings: [
            {
                name: "AzureMaps:ClientId",
                value: mapsAccount.properties.uniqueId,
            },
        ],
    },
});

// Assign Azure Maps Data Reader role to API managed identity
const roleAssignment = new azure_native.authorization.RoleAssignment(
    "ra-autonomouscars",
    {
        principalId: webApp.identity.apply((x) => x?.principalId!),
        roleDefinitionId:
            "providers/Microsoft.Authorization/roleDefinitions/423170ca-a8f6-4b0f-8487-9e4eb8f49bfa",
        scope: mapsAccount.id,
        principalType: PrincipalType.ServicePrincipal,
    }
);
