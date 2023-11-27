// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  typescript: {
    strict: true,
  },

  runtimeConfig: {
    public: {
      azureMapsClientId: process.env.AZURE_MAPS_CLIENT_ID,
    },
  },

  modules: ["@unocss/nuxt", "@nuxt/ui", "vue3-carousel-nuxt"],

  ssr: false,

  nitro: {
    devProxy: {
      "/api": {
        target: "https://localhost:7238/api/",
        secure: false,
      },
    },
  },
  devtools: {
    enabled: true,

    timeline: {
      enabled: true,
    },
  },
});
