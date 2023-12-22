<script setup lang="ts">
import { ref } from 'vue';

const tabs = ref(['Tous', 'Disponible', 'Indisponible']);
// TODO: tabContents get all cars 
const tabContents = ref(['1', '2', '3']);
const activeTab = ref(0);

function changeTab(index: number) {
  activeTab.value = index;
}
</script>
<template>
    <div class="py-3">
      <ul class="hidden text-xs font-medium text-center text-gray-500 rounded-xl shadow sm:flex dark:divide-gray-700 dark:text-gray-400">
        <li v-for="(tab, index) in tabs" :key="index" class="w-full">
          <a @click="changeTab(index)" :class="{ 'text-gray-900 bg-gray-100': activeTab === index, 'bg-white hover:text-gray-700 hover:bg-gray-50': activeTab !== index }" class="inline-block w-full p-3 border-r border-gray-200 dark:border-gray-700 rounded-s-lg focus:ring-4 focus:ring-blue-300 active focus:outline-none dark:bg-gray-700 dark:text-white" aria-current="page">{{ tab }}</a>
        </li>
      </ul>
    </div>
      <div v-for="(content, index) in tabContents" :key="index" v-show="activeTab === index">
        <!-- Contenu dynamique de l'onglet -->
        {{ content }}
        <ListCard></ListCard>
      </div>
  </template>

  