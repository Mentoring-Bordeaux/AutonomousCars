<script setup lang="ts">
import SearchInput from "~/components/searchInput.vue";

var now = new Date();
var year = now.getFullYear();
var month = now.getMonth() + 1;
var day = now.getDate();
var hour = now.getHours();
var minute = now.getMinutes();
var localDate = year + '-' + (month < 10 ? '0' + month.toString() : month) + '-' + (day < 10 ? '0' + day.toString() : day) 
var localTime = (hour < 10 ? '0' + hour.toString() : hour) + ':' + (minute < 10 ? '0' + minute.toString() : minute);
const departure = ref('')
const arrival = ref('')
const departureTime = ref(localTime)
const departureDate = ref(localDate)
const startPosition = ref({})
const endPosition = ref({})

const position = reactive({
    start: {}, 
    stop: {}
})

function handleSubmit() {
  console.log("Position de départ : ", startPosition);
  console.log("Position d'arrivée : ", endPosition);
  console.log("Date de départ : ", departureDate.value);
  console.log("Heure de départ : ", departureTime.value);
}

function handlePositionUpdate(newPosition: {lat: number, lon: number}) {
    console.log('Position updated: ', newPosition);
  }


</script>

<template>
  <form @submit.prevent="handleSubmit" class="grid gap-4 max-w-lg mx-auto bg-[#ECECEC] text-black text-bold p-6 border rounded border-black">

    <div class="grid gap-2">
      <label :for="departure" class="text-sm">Départ de : </label>
      <SearchInput v-model="departure" :position.sync="startPosition" @update:position="startPosition = $event" placeholder="Position actuelle" />
    </div>

    <div class="grid gap-2 grid-cols-2">
      <div>
        <label for="date" class="text-sm mr-2">Date de départ :</label>
        <input id="date" v-model="departureDate" type="date" class="p-2 border rounded bg-white text-gray-500">
      </div>
      <div>
        <label for="time" class="text-sm mr-2">Heure de départ : </label>
        <input id="time" v-model="departureTime" type="time" class="p-2 border rounded bg-white text-gray-500">
      </div>
    </div>

    <div class="grid gap-2">
      <label :for="arrival" class="text-sm">Arriver à :</label>
      <SearchInput v-model="departure" :position.sync="endPosition" @update:position="startPosition = $event" placeholder="Saisir une adresse..." />
    </div>

    <div class="grid gap-2 grid-cols-2">
      <button type="reset" class="p-2 bg-gray-300 rounded">Annuler</button>
      <button type="submit" class="p-2 bg-orange-500 text-white rounded">Continuer</button>
    </div>

  </form>

</template>