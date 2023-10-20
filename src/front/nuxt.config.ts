// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  typescript: {
    strict: true,
  },
  ssr: false,
  nitro: {
    devProxy: {
      "/api": {
        target: "https://localhost:7238/api/",
        secure: false
      },
    }
  }
})
