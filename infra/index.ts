import * as resources from "@pulumi/azure-native/resources";
import * as pulumi from "@pulumi/pulumi";
import * as azure_native from "@pulumi/azure-native";

// Create an Azure Resource Group
const resourceGroup = new resources.ResourceGroup("rg-autonomouscars", {
    location:"westeurope", 
    resourceGroupName:"rg-autonomouscars"
});



const webApp = new azure_native.web.WebApp("stapp-autonomouscars", {
    // cloningInfo: {
    //     appSettingsOverrides: {
    //         Setting1: "NewValue1",
    //         Setting3: "NewValue5",
    //     },
    //     cloneCustomHostNames: true,
    //     cloneSourceControl: true,
    //     configureLoadBalancing: false,
    //     hostingEnvironment: "/subscriptions/34adfa4f-cedf-4dc0-ba29-b6d1a69ab345/resourceGroups/testrg456/providers/Microsoft.Web/hostingenvironments/aseforsites",
    //     overwrite: false,
    //     sourceWebAppId: "/subscriptions/34adfa4f-cedf-4dc0-ba29-b6d1a69ab345/resourceGroups/testrg456/providers/Microsoft.Web/sites/srcsiteg478",
    //     sourceWebAppLocation: "West Europe",
    // },
    // kind: "app",
    location: resourceGroup.location,
    // name: "sitef6141",
    resourceGroupName: resourceGroup.name
});


// const staticSite = new azure_native.web.StaticSite("stapp-autonomouscars", {
//     branch: "main",
//     // buildProperties: {
//     //     apiLocation: "api",
//     //     appArtifactLocation: "build",
//     //     appLocation: "app",
//     // },
//     location,
//     name: "stapp-autonomouscars",
//     // repositoryToken: "repoToken123",
//     // repositoryUrl: "https://github.com/username/RepoName",
//     resourceGroupName: resourceGroup.name,
//     // sku: {
//     //     name: "Basic",
//     //     tier: "Basic",
//     // },
// });