<script>
export default {
  colorMode: 'light',
}
definePageMeta({
  colorMode: 'light',
})
</script>
<template>
    <div id="container" class=" flex flex-col ">
        <div id="header" class=" flex flex-row p-8">
            <AboutContent></AboutContent>
            <AboutContactList></AboutContactList>
        </div>

        <div id="footer" class="h-1/3">
            <h2 class="text-black title-large font-bold p-l-16">Les technologies utilisées</h2>

            <AboutTechnoCarousel />
        </div>
    </div>
</template>