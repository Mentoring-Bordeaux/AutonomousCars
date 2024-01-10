<script setup lang="ts">

import type { carItem } from "~/models/dropdownCar"

const carList = ref<carItem[]>([
    { name: 'Model S - Tesla', icon: 'img/car_icon.png' },
    { name: 'Model 3 - Tesla', icon: 'img/car_icon.png' },
    { name: 'Model X - Tesla', icon: 'img/car_icon.png' },
    { name: 'Model Y - Tesla', icon: 'img/car_icon.png' }
]);

const isDropdownOpen = ref(false);
const selectedCar = ref<carItem>(carList.value[0]); // Initialisez avec le label de la première action

function toggleDropdown() {
    isDropdownOpen.value = !isDropdownOpen.value;
}

const emit = defineEmits(['click']);

onMounted(() =>{
    emit('click', selectedCar.value);
});

function selectCar(car: carItem) {
    selectedCar.value = car;
    isDropdownOpen.value = false;
    emit('click', selectedCar.value);
}
</script>

<template>
    <div class="button-list w-full px-4 py-2 bg-gray-100 flex items-center justify-between cursor-pointer rounded border-lightgray" @click="toggleDropdown">
        <CreateCarItem :car="selectedCar" />
        <i class="i-heroicons-chevron-down"></i>
    </div>
    <div v-if="isDropdownOpen" class="w-full rounded-md shadow-lg bg-white absolute">
      <div 
        v-for="(car, index) in carList" :key="index" 
        class="flex items-center px-4 py-2 hover:bg-gray-100 cursor-pointer"
        @click="selectCar(car)">
            <CreateCarItem :car="car" />
      </div>
    </div>
</template>