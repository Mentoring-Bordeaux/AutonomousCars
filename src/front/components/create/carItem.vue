<script setup lang="ts">
import type { carItem } from "~/models/dropdownCar"

const props = defineProps<{
    car : carItem;
}>()

</script>

<template>
    <div class="rounded-full flex items-center mr-12 w-12 h-12 border border-gray-300 hover:drop-shadow-xl hover:rounded-full">
        <img :src="props.car.icon" alt="Action Avatar">
    </div>
    <span class="flex-1 text-md text-gray-700">{{ props.car.vehicle?.carName }}</span>
</template>