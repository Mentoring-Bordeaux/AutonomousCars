<script setup lang="ts">
import { ref } from 'vue';

const tabs = ref(['Tous', 'Disponible', 'Indisponible']);
const tabContents = ref(['', '', '']);  
const activeTab = ref(0);

function changeTab(index: number) {
  activeTab.value = index;
}
</script>
<template>
    <div class="bg-gray-100 p-4 shadow">
      <div class="flex items-center">
        <nuxt-link :to="{ path: '/'}" class="text-gray-600 hover:text-gray-800 focus:outline-none">
          <i class="i-heroicons-chevron-left"></i>
        </nuxt-link>
        <h1 class="text-xl text-[#E36C39] font-400 ml-4">Les véhicules</h1>
      </div>
    </div>

    <div class="py-3">
      <ul class="hidden text-xs font-medium text-center text-gray-500 rounded-xl shadow sm:flex dark:divide-gray-700 dark:text-gray-400">
        <li v-for="(tab, index) in tabs" :key="index" class="w-full">
          <a @click="changeTab(index)" :class="{ 'text-gray-900 bg-gray-100': activeTab === index, 'bg-white hover:text-gray-700 hover:bg-gray-50': activeTab !== index }" class="inline-block w-full p-3 border-r border-gray-200 dark:border-gray-700 rounded-s-lg focus:ring-4 focus:ring-blue-300 active focus:outline-none dark:bg-gray-700 dark:text-white" aria-current="page">{{ tab }}</a>
        </li>
      </ul>
    </div>
      <div v-if=useVehiclesListStore().isLoaded v-for="(_, index) in tabContents" :key="index" v-show="activeTab === index" class="h-125 overflow-y-scroll">
        <VehiclesCardList :type="index"></VehiclesCardList>
      </div>
      <div v-else > 
        <VehiclesLoader></VehiclesLoader>
      </div>
  </template>

  