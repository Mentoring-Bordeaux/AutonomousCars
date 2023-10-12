<script setup lang="ts">
import type { WeatherForecast } from '~/models/weatherForecast';

const { data: forecasts } = await useFetch<WeatherForecast[]>(
  "https://localhost:7238/weatherforecast"
);
</script>

<template>
  <h1>Weather forecasts</h1>
  <WeatherForecast
    v-for="forecast in forecasts"
    :key="forecast.date"
    :forecast="forecast"
  />
</template>
