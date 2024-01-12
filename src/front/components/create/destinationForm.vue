<script setup lang="ts">
import SearchInput from "~/components/searchInput.vue";
import type { Position } from "~/models/address"
import type { carItem } from "~/models/dropdownCar"

const now = new Date();
const optionsDate: Intl.DateTimeFormatOptions = { year: 'numeric', month: '2-digit', day: '2-digit' };
const localDate = now.toLocaleDateString('fr-FR', optionsDate).split('/').reverse().join('-');
const optionsTime: Intl.DateTimeFormatOptions = { hour: '2-digit', minute: '2-digit', hour12: false };
const localTime = now.toLocaleTimeString('fr-FR', optionsTime);

const departureTime = ref(localTime);
const departureDate = ref(localDate);
const startPosition = ref<Position>();
const endPosition = ref<Position>();
const chosenCar = ref<carItem>();
const departure = ref('');
const arrival = ref('');

const emit = defineEmits({
  submit: ({departure, arrival, startPosition, endPosition, chosenCar}) => {
    if(departure !== '' && arrival !== '' && startPosition && endPosition && chosenCar !== undefined)
      return true; 
    return false; 
  }
});

function handleSubmit() {
  emit('submit', {departure, arrival, startPosition, endPosition, chosenCar});
}

const handleChosenCar = (payload: carItem) => {
  chosenCar.value = payload;
}

</script>

<template>
  <div class="bg-gray-100 p-4 shadow">
    <div class="flex items-center">
      <nuxt-link :to="{ path: '/'}" class="text-gray-600 hover:text-gray-800 focus:outline-none">
        <i class="i-heroicons-chevron-left"></i>
      </nuxt-link>
      <h1 class="text-xl text-[#E36C39] font-400 ml-4">Créer un nouveau trajet</h1>
    </div>
  </div>
  <div class="relative m-4">
    <CreateCarList @click="handleChosenCar"/>

    <form class="grid gap-4 max-w-lg mx-auto bg-gray-100 text-black text-bold p-6 border rounded mt-4 border-lightgray" @submit.prevent="handleSubmit">
        <div class="grid gap-2">
          <label :for="departure" class="text-sm">Départ de : </label>
          <SearchInput v-model="departure" v-model:position="startPosition" placeholder="Position actuelle" @update:position="startPosition = $event" />
        </div>

        <div class="grid gap-2 grid-cols-2">
          <div>
            <label for="date" class="text-sm mr-2">Date de départ :</label>
            <input id="date" v-model="departureDate" type="date" class="p-2 border rounded bg-white text-gray-500 w-full">
          </div>
          <div>
            <label for="time" class="text-sm mr-2">Heure de départ : </label>
            <input id="time" v-model="departureTime" type="time" class="p-2 border rounded bg-white text-gray-500 w-full">
          </div>
        </div>

        <div class="grid gap-2">
          <label :for="arrival" class="text-sm">Arriver à :</label>
          <SearchInput v-model="arrival" v-model:position="endPosition" placeholder="Saisir une adresse..." @update:position="endPosition = $event" />
        </div>

        <div class="grid gap-2 grid-cols-2">
          <button type="reset" class="p-2 bg-white text-[#E36C39] border border-[#E36C39] rounded-lg hover:bg-gray-200">Annuler</button>
          <button type="submit" class="p-2 bg-[#E36C39] text-white rounded-lg hover:bg-orange-600">Continuer</button>
        </div>
      </form>
  </div>
</template>