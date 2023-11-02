<template>
    <div id="container" class="min-h-screen flex flex-col bg-white">
        <div id="header" class="flex flex-row p-8 p-b-0">
            <AboutContent></AboutContent>
            <AboutContactList></AboutContactList>
        </div>
        <div id="footer" class="bg-white p-16 p-t-0 p-b-0">
            <h2 class="text-black display-large font-bold">Les technologies utilisées</h2>

            <AboutTechnoCarousel/>
        </div>
    </div>

</template>