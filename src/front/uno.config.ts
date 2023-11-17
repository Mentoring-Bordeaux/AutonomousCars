import { defineConfig } from 'unocss'
import presetWind from '@unocss/preset-wind'
import presetWebFonts from '@unocss/preset-web-fonts'
import axios from 'axios'

export default defineConfig({
    presets: [
        presetWind(),
        presetWebFonts({
            // use axios with an https proxy
            customFetch: (url: string) => axios.get("https://fonts.googleapis.com/css?family=Inter").then(it => it.data),
            provider: 'google',
            fonts: {
              sans: 'Inter',
              mono: ['Fira Code', 'Fira Mono:400,700'],
            },
          }),
    ],
    theme: {
        colors: {
            'primary': '#E36C39',
            'secondary': '#1D192B',
        }
    },
    rules: [
        ['title-large', {'font-family': 'Inter Regular', 'font-size': '22px',}],
        ['title-medium', {'font-family': 'Inter Medium', 'font-size': '16px',}],
        ['title-small', {'font-family': 'Inter Medium', 'font-size': '14px',}],
        ['body-large', {'font-family': 'Inter', 'font-size': '16px',}],
        ['body-medium', {'font-family': 'Inter', 'font-size': '14px',}],
        ['body-small', {'font-family': 'Inter', 'font-size': '12px',}],
    ]
})