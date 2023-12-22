<script setup>
import { ref, watch, toRef } from 'vue';
import create from '~/components/create/content.vue';
import list from '~/components/list/content.vue';
import update from '~/components/update/content.vue';

const showSidebar = ref(true);
const tabs = { create, list, update };
const key = ref(0);

const route = useRoute();
const tab = toRef(() => route.query.tab);

const isTab = computed(() => Object.keys(tabs).includes(tab.value));

const toggleSidebar = () => {
  showSidebar.value = !showSidebar.value;
};

watch(showSidebar, () => {
  key.value += 1;
});
</script>

<template>
  <div class="flex">
    <div :class="{ 'w-9/10': showSidebar, 'w-full': !showSidebar }">
      <MapBackground :key="key" />
    </div>
    <div :class="[showSidebar ? 'w-4/10' : 'ml-auto', 'bg-white p-4', showSidebar ? 'shadow-2xl' : 'shadow-2xl']">
      <button class="btn" @click.stop="toggleSidebar">
        <i :class="showSidebar ? 'i-heroicons-chevron-right' : 'i-heroicons-chevron-left'"></i>
      </button>
      <div v-if="!isTab">
        <SidebarItem icon="i-heroicons-list-bullet-solid" title="Les véhicules" :queryParams="{ tab: 'list' }" :sideBar=showSidebar />
        <SidebarItem icon="i-heroicons-plus-solid" title="Créer un trajet" :queryParams="{ tab: 'create' }" :sideBar=showSidebar />
        <SidebarItem icon="i-heroicons-pencil-square-solid" title="Modifier un trajet" :queryParams="{ tab: 'update' }" :sideBar=showSidebar />
      </div>
      <component v-else :is="tabs[tab]"></component>
    </div>
  </div>
  <!-- Bouton de retour en bas de la page -->
  <div class="fixed bottom-4 right-4" v-if="isTab">
    <nuxt-link :to="{ path: '/'}" class="text-base text-400 hover:underline">
      <i class="i-heroicons-chevron-left"></i>Retour
    </nuxt-link>
  </div>
</template>
