// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  typescript: {
    strict: true,
  },

  modules: [
    '@unocss/nuxt', '@nuxt/ui'
  ],

  ssr: false,

  nitro: {
    devProxy: {
      "/api": {
        target: "https://localhost:7238/api/",
        secure: false
      },
    }
  },
  devtools: {
    enabled: true,

    timeline: {
      enabled: true
    }
  }
})