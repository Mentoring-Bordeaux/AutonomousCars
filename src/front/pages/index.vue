<script setup lang="ts">
import type { WeatherForecast } from '~/models/weatherForecast';

const { data: forecasts } = await useFetch<WeatherForecast[]>(
  "api/weatherforecast"
);
</script>

<template>
  <div v-for="forecast in forecasts" :key="forecast.date">
    <WeatherForecastDisplay :forecast="forecast"/>
  </div>
</template>
