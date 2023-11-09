<script setup lang="ts">
    import { technologyComponent } from '~/models/about';

    defineProps<technologyComponent>()
</script>

<template>
    <UContainer class="flex flex-col items-center bg-white p-4 drop-shadow-xl h-fit">
        <img :src="img" class="w-40 rounded-full drop-shadow-xl"/>
        <p class="display-small uppercase text-center text-black">{{ surname }}</p>
        <p class="display-small text-center text-black">{{ name }}</p>
        <UBadge :label="status" color="orange" size="md" variant="subtle"></UBadge>
    </UContainer>
</template>