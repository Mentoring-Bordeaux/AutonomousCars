
<script setup lang="ts">
const technos = ["/img/nuxt.png", "/img/nuke.png", "/img/pulumi.svg", "/img/dotnet.png", "/img/azure.svg", "/img/unocss.png", "/img/github.png"]
const sites = ["https://nuxt.com/",
"https://nuke.build/",
"https://www.pulumi.com/",
"https://dotnet.microsoft.com/en-us/",
"https://azure.microsoft.com/fr-fr",
"https://unocss.dev/",
"https://github.com/Mentoring-Bordeaux/AutonomousCars"]
</script>


<template>
    <carousel :items-to-show="3"
    :wrap-around="true"
    :items-to-scroll="1"
    :autoplay="2000"
    :transition="800"
    :pause-autoplay-on-hover="true"
    class="ml-60 mr-60">

    <slide v-for="slide in 7" :key="slide" >
        <a :href="sites[slide - 1]" target="_blank">
            <AboutTechnology :img="technos[slide - 1]" />
        </a>
    </slide>

    <template #addons>
        <Navigation />
        <Pagination />
    </template>

</carousel>

</template>
