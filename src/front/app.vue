<script setup>
useVehiclesListStore().initVehiclesDevices()
</script>

<template>
  <div>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>

      <UNotifications class="w-100"/>
  </div>
</template>