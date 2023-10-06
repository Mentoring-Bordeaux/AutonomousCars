// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },
  devServer: {
    https: {
      key: "./certs/localhost-key.pem",
      cert: "./certs/localhost.pem",
    },
  },
  typescript: {
    strict: true,
  },
  ssr: false,
});
