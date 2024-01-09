<script setup lang="ts">
import SearchInput from "~/components/searchInput.vue";
import type { Position } from "~/models/address"

const now = new Date();
const year = now.getFullYear();
const month = now.getMonth() + 1;
const day = now.getDate();
const hour = now.getHours();
const minute = now.getMinutes();
const localDate = year + '-' + (month < 10 ? '0' + month.toString() : month) + '-' + (day < 10 ? '0' + day.toString() : day) 
const localTime = (hour < 10 ? '0' + hour.toString() : hour) + ':' + (minute < 10 ? '0' + minute.toString() : minute);



const departureTime = ref(localTime)
const departureDate = ref(localDate)
const departure = ref('')
const arrival = ref('')

const props = defineProps<{
  startPosition: Position, 
  endPosition: Position
  isInForm: Boolean
}>()



const emit = defineEmits(['update:startPosition', 'update:endPosition', 'update:isInForm']);
const startPosition = ref(props.startPosition)
const endPosition = ref(props.endPosition)

function handleSubmit() {
  console.log("Position de départ : ", startPosition.value);
  console.log("Position d'arrivée : ", endPosition.value);
  console.log("Date de départ : ", departureDate.value);
  console.log("Heure de départ : ", departureTime.value);
  emit('update:startPosition', startPosition.value);
  emit('update:endPosition', endPosition.value)
  emit('update:isInForm', false)
  sendPosition();
}

async function sendPosition() {
  fetch('api/Position/position', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify(startPosition.value),
})
.then(response => response)
.then(data => console.log(data))
.catch((error) => console.error('Error:', error));
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
  <div class="p-4">
    <form @submit.prevent="handleSubmit" class="grid gap-4 max-w-lg mx-auto bg-gray-100 text-black text-bold p-6 border rounded border-lightgray">
        <div class="grid gap-2">
          <label :for="departure" class="text-sm">Départ de : </label>
          <SearchInput v-model="departure" :position.sync="startPosition" @update:position="startPosition = $event" placeholder="Position actuelle" />
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
          <SearchInput v-model="departure" :position.sync="endPosition" @update:position="endPosition = $event" placeholder="Saisir une adresse..." />
        </div>

        <div class="grid gap-2 grid-cols-2">
          <button type="reset" class="p-2 bg-gray-300 text-gray rounded">Annuler</button>
          <button type="submit" class="p-2 bg-orange-500 text-white rounded">Continuer</button>
        </div>
      </form>
  </div>
</template>