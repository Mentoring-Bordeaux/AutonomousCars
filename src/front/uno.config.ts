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
        }
    },
    rules: [
        ['display-large', {'font-family': 'Inter', 'font-size': '57px',}],
        ['display-medium', {'font-family': 'Inter', 'font-size': '22px',}],
        ['display-small', {'font-family': 'Inter', 'font-size': '16px',}]
    ]
})