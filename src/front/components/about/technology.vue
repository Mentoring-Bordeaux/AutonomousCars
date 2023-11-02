<script setup>
defineProps(['img', 'website'])
</script>


<template>
    <UContainer class="flex items-center w-40 h-40 m-4 drop-shadow-xl bg-white">
        <a :href="website" target="_blank">
            <img :src="img" alt="techno_img" class="w-fit"/>
        </a>
    </UContainer>
</template>