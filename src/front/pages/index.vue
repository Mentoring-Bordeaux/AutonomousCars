<script setup lang="ts">
import type { WeatherForecastType } from "~/models/weather-forecast";

const { data: forecasts } = await useFetch<WeatherForecastType[]>(
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
