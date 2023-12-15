<script setup>
import { ref } from 'vue';

const showSidebar = ref(true);

const toggleSidebar = () => {
  showSidebar.value = !showSidebar.value;
};
</script>

<template>
  <div class="flex h-screen">
    <div :class="{ 'w-9/10': showSidebar, 'w-full': !showSidebar }">
            <MapBackground />
    </div>
    <div :class="[showSidebar ? 'w-4/10' : 'ml-auto', 'bg-white p-4', showSidebar ? 'shadow-2xl' : 'shadow-2xl']" @click.stop="toggleSidebar">    
      <UButton color="orange" variant="solid">
        <i :class="showSidebar ? 'i-heroicons-chevron-right' : 'i-heroicons-chevron-left'"></i>
      </UButton>
      <ItemSidebar icon="i-heroicons-chevron-right" title="Les véhicules" link="/about" :sideBar=showSidebar />
      <ItemSidebar icon="i-heroicons-plus" title="Créer un trajet" link="/about" :sideBar=showSidebar />
      <ItemSidebar icon="i-heroicons-pencil" title="Modifier un trajet" link="/about" :sideBar=showSidebar />
      <slot />
    </div>
  </div>
</template>

