import { defineConfig } from 'unocss'
import presetWind from '@unocss/preset-wind'

export default defineConfig({
    presets: [
        presetWind(),
    ],
    theme: {
        colors: {
            'primary': '#E36C39',
            'secondary': '#1D192B',
            'background-color': "#FFFFF",
            'green': "#068422",
            'red': "#C51313",
            'gray': "#5C5C5C",
        }
    },
    rules: [
        ['title-large', {'font-family': 'Inter', 'font-size': '22px',}],
        ['title-medium', {'font-family': 'Inter', 'font-size': '16px',}],
        ['title-small', {'font-family': 'Inter', 'font-size': '14px',}],
        ['body-large', {'font-family': 'Inter', 'font-size': '16px',}],
        ['body-medium', {'font-family': 'Inter', 'font-size': '14px',}],
        ['body-small', {'font-family': 'Inter', 'font-size': '12px',}],
    ]
})