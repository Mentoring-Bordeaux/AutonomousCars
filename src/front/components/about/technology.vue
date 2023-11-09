<script setup>
defineProps(['img'])
</script>


<template>
    <UContainer class="flex items-center h-40 w-40 drop-shadow-xl bg-white m-5">
        <img :src="img" alt="techno_img" class="w-fit"/>
    </UContainer>
</template>