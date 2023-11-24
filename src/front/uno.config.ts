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
        ['title-large', {'font-family': 'Inter Regular', 'font-size': '22px',}],
        ['title-medium', {'font-family': 'Inter Medium', 'font-size': '16px',}],
        ['title-small', {'font-family': 'Inter Medium', 'font-size': '14px',}],
        ['body-large', {'font-family': 'Inter', 'font-size': '16px',}],
        ['body-medium', {'font-family': 'Inter', 'font-size': '14px',}],
        ['body-small', {'font-family': 'Inter', 'font-size': '12px',}],
    ]
})