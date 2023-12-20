<script setup lang="ts">
import type { Address } from "~/models/address"
let timer: ReturnType<typeof setTimeout>|null = null;
const { fetchAdresses } = useAzureMapsAPI()

const departure = ref('')
const searchResults = ref<Address[]>([])
const startPosition = ref({})

async function sendRequest() {
  searchResults.value = (await fetchAdresses(departure.value)) ?? [];
  console.log(searchResults.value)
}

function selectResult(result: Address) {
  // Gérer la sélection d'un résultat, par exemple remplir le champ avec le résultat sélectionné
  departure.value = result.name + ', ' + result.postalCode + ', ' +  result.municipality;
  startPosition.value = result.position
  searchResults.value = []; // Vider la liste des résultats après la sélection
  console.log("Position sauvegardé : ", startPosition.value)
}


</script>

<template>
  <form 
    class="max-w-lg mx-auto mt-8 p-6 bg-white shadow-md rounded">

    <div>
      <label for="departure" 
        class="block text-gray-700 font-semibold mb-2">
        Arriver à :
      </label>
      <input type="text" id="departure" v-model="departure" 
          @input="sendRequest" 
          class="w-full border rounded-md mb-2 py-2 px-3 text-white-700 
                  leading-tight focus:outline-none focus:shadow-outline" required>
    </div>

    <div v-if="searchResults.length > 0" class="border rounded-md">
      <div class="overflow-hidden h-[200px]">
        <div class="flex flex-col overflow-y-scroll h-[200px]">
          <div v-for="result in searchResults" :key="result.id" 
              class="py-2 px-3 cursor-pointer hover:bg-gray-200 text-black" 
              @click="selectResult(result)"
            >
            <div>
              <span>{{ result.name }}</span>
              <br>
              <span>{{ result.postalCode }} {{ result.municipality }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <button type="submit" 
        class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 
          rounded focus:outline-none focus:shadow-outline">
        Soumettre
    </button>
  </form>
</template>