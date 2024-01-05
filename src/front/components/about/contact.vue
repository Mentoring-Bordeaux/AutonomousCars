<script setup lang="ts">
import type { technologyComponent } from '~/models/about';


    defineProps<technologyComponent>()
</script>

<template>
    <UContainer class="flex flex-col items-center bg-white p-4 drop-shadow-xl">
        <div class="w-20 h-20 rounded-full overflow-hidden">
            <img :src="img" class="w-full h-full object-cover"/>
        </div>
        <p class="display-small uppercase text-center text-black">{{ surname }}</p>
        <p class="display-small text-center text-black">{{ name }}</p>
        <UBadge :label="status" color="orange" size="md" variant="subtle"></UBadge>
    </UContainer>
</template>