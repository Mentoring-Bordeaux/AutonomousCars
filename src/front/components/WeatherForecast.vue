<script setup lang="ts">
import type { WeatherForecast } from "~/models/weather-forecast";

const { forecast } = defineProps<WeatherForecast>();
</script>

<template>
  <p>
    {{ forecast.date }}: {{ forecast.summary }} ({{ forecast.temperatureC }}°C,
    {{ forecast.temperatureF }}°F)
  </p>
</template>
