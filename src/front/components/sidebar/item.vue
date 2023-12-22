<script setup lang="ts">
import type { section } from '~/models/item';
defineProps<section>();
</script>

<template>
  <UContainer class="max-w-screen-xl mx-auto">
    <div class="flex items-center justify-items-start my-5">
      <div>
        <div class="rounded-full bg-gray-100 p-3 border border-gray-300 flex items-center justify-center">          
          <i :class="`${icon} text-[#E36C39] text-lg font-bold m-0 p-0`"></i>
        </div>
      </div>
      <div class="ml-4" v-if="sideBar">
        <nuxt-link :to="{ path: '/', query: queryParams }" class="text-base text-400 hover:underline">
          {{ title }}
        </nuxt-link>
      </div>
    </div>
  </UContainer>
</template>
