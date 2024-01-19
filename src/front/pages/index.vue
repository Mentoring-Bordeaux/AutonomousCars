<script setup>
import { ref, watch, toRef } from 'vue';
import create from '~/components/create/content.vue';
import vehicles from '~/components/vehicles/content.vue';
import update from '~/components/update/content.vue';

definePageMeta({
  colorMode: 'light',
})

const showSidebar = ref(true);
const tabs = { create, vehicles, update };

const route = useRoute();
const tab = toRef(() => route.query.tab);

const isTab = computed(() => Object.keys(tabs).includes(tab.value));

const toggleSidebar = () => {
  showSidebar.value = !showSidebar.value;
};

const key = computed(() => showSidebar.value ? 1 : 0);
</script>
<template>
  <div class="flex h-[calc(100vh-56px)]">
    <div :class="{ 'w-9/10': showSidebar, 'w-screen': !showSidebar }">
      <MapBackground :key="key" />
    </div>
    <div :class="[showSidebar ? 'w-4/10' : 'ml-auto', 'bg-white', showSidebar ? 'shadow-2xl' : 'shadow-2xl']">
      <button v-if="!isTab" class="btn ml-5 pt-4" @click.stop="toggleSidebar" >
        <i :class="showSidebar ? 'i-heroicons-chevron-right' : 'i-heroicons-chevron-left'"></i>
      </button>
      <div v-if="!isTab">
        <SidebarItem icon="i-heroicons-list-bullet-solid" title="Les véhicules" :queryParams="{ tab: 'vehicles' }" :sideBar=showSidebar />
        <SidebarItem icon="i-heroicons-plus-solid" title="Créer un trajet" :queryParams="{ tab: 'create' }" :sideBar=showSidebar />
        <SidebarItem icon="i-heroicons-pencil-square-solid" title="Modifier un trajet" :queryParams="{ tab: 'update' }" :sideBar=showSidebar />
      </div>
      <component v-else :is="tabs[tab]"></component>
    </div>
  </div>
</template>
