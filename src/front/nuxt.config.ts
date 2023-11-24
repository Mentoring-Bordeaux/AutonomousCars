import { join } from "path";

const certificateName = process.env.npm_package_name;
const certificateFolder = process.env.APPDATA
  ? `${process.env.APPDATA}/ASP.NET/https`
  : `${process.env.HOME}/.aspnet/https`;

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devServer: {
    https: {
      key: join(certificateFolder, `${certificateName}.key`),
      cert: join(certificateFolder, `${certificateName}.pem`),
    },
  },
  devtools: { enabled: true },
  modules: ["@unocss/nuxt"],
  nitro: {
    devProxy: {
      "/api": {
        target: "https://localhost:7238/api/",
        secure: false,
      },
    },
  },
  ssr: false,
  typescript: {
    strict: true,
  },
});
