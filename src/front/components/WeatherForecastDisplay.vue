<script setup lang="ts">
import { WeatherForecast } from "~/models/weatherForecast";
defineProps<{forecast: WeatherForecast}>();
</script>

<template>
  <p>
    {{ forecast.date }}: {{ forecast.summary }} ({{ forecast.temperatureC }}°C,
    {{ forecast.temperatureF }}°F)
  </p>
</template>