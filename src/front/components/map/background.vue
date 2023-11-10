<script lang="ts">
    import * as atlas from "azure-maps-control";
    import "azure-maps-control/dist/atlas.min.css";
import { LanguageServiceMode } from "typescript";

    export default {
        mounted() {this.initializeMap()}, 
        methods: {
            initializeMap() {
                const map = new atlas.Map("map", {
                    view: "Auto",
                    language: "fr-FR",
                    center: [-0.604945545248392, 44.806516403595744],
                    zoom: 10,
                    authOptions: {
                        authType: "subscriptionKey" as atlas.AuthenticationType,
                        subscriptionKey: "76c6F3ZTQ9iJDRo20oAU-D5YtyP7wkG-s1BRffLLgbc"
                    }
                });
            }
        }
    }
    

    
</script>

<template>
    <div id="map" class="w-screen h-screen p-0 m-0"></div>
</template>