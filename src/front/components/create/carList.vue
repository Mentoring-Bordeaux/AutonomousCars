<script setup lang="ts">
import type { carItem } from "~/models/dropdownCar"
import type { Vehicle } from '~/models/Vehicle';

const vehiclesListStore = useVehiclesListStore();
const carList = ref<carItem[]>([])
const carName: string[] = ['Model S - Tesla', 'Model 3 - Tesla', 'Model X - Tesla', 'Model Y - Tesla', 'Model S Plaid - Tesla'];
const isDropdownOpen = ref<boolean>(false);
const areAvailableCar = ref<boolean>(false);
const selectedCar = ref<carItem>();
const emit = defineEmits(['click']);

onMounted(() =>{
    if(vehiclesListStore.isLoaded)
        populateCarList();
    watch(() => vehiclesListStore.isLoaded, (isLoaded) => {
        if(isLoaded){
            populateCarList();
        }
    })
});

function toggleDropdown() {
    isDropdownOpen.value = !isDropdownOpen.value;
}

function populateCarList(){
    const availableCar: Vehicle[]  = vehiclesListStore.vehiclesList.filter((vehicule) => !vehicule.available);
    if(availableCar.length !== 0){
        const newCarList: carItem[] = availableCar.map(car => ({
            name: carName[Math.floor(Math.random() * carName.length)],
            icon: 'img/car_icon.png',
            vehicle: car
        }));

        carList.value.push(...newCarList);
        areAvailableCar.value = true;
    }
}

function selectCar(car: carItem) {
    selectedCar.value = car;
    isDropdownOpen.value = false;
    emit('click', selectedCar.value);
}
</script>

<template>
    <div class="button-list w-full px-4 py-2 bg-gray-100 flex items-center justify-between cursor-pointer rounded border-lightgray" @click="toggleDropdown">
        <div v-if="selectedCar !== undefined" class="flex items-center justify-between">
            <CreateCarItem :car="selectedCar" />
        </div>
        <div v-else-if="selectedCar === undefined" class="flex items-center justify-between h-12">
            <span class="text-md">Choisir un véhicule</span>
        </div>
        <i class="i-heroicons-chevron-down"></i>
    </div>
    <div v-if="isDropdownOpen" class="w-full rounded-md shadow-lg bg-white absolute">
        <div v-if="vehiclesListStore.isLoaded && areAvailableCar">
            <div 
                v-for="(car, index) in carList" :key="index" 
                class="flex items-center px-4 py-2 hover:bg-gray-100 cursor-pointer"
                @click="selectCar(car)">
                    <CreateCarItem :car="car" />
            </div>
        </div>
        <div v-else-if="!vehiclesListStore.isLoaded" class="flex justify-center items-center px-4 py-2">
            <i class="i-heroicons-arrow-path text-[#E36C39] text-5xl mr-12 flex items-center cursor-pointer animate-spin"></i>
            <span class="flex-1 text-md text-gray-700">Chargement des données...</span>
        </div>
        <div v-else-if="vehiclesListStore.isError" class="flex justify-center items-center px-4 py-2">
            <i class="i-heroicons-arrow-path text-[#E36C39] text-5xl mr-12 flex items-center"></i>
            <span class="flex-1 text-md text-gray-700">Erreur lors du chargement...</span>
        </div>
        <div v-else-if="!areAvailableCar" class="flex justify-center items-center px-4 py-2">
            <span class="text-md text-gray-700 flex items-center h-12">Aucune voiture disponible</span>
        </div>
    </div>
</template>