<script setup lang="ts">
import type { section } from '~/models/item';
defineProps<section>();
</script>

<template>
  <UContainer class="max-w-screen-xl mx-auto">
    <div class="flex items-center justify-items-start my-5">
      <div>
        <div class="rounded-full bg-gray-100 p-3 border border-gray-300">
          <i :class="`${icon} text-orange text-lg font-bold`"></i>
        </div>
      </div>
      <div class="ml-4" v-if="sideBar">
        <nuxt-link :to="link" class="text-base text-400 hover:underline">
          {{ title }}
        </nuxt-link>
      </div>
    </div>
  </UContainer>
</template>
