<script setup lang="ts">
import type { FetchAddressesFunction, Address } from "~/models/address";
import { useAzureMapsAPI } from "~/composables/useAzureMapsAPI"

const fetchAdresses = ref<(FetchAddressesFunction | null)>(null);
  

  onMounted(async () => {
    const api = await useAzureMapsAPI()
    fetchAdresses.value = api.fetchAdresses
  }); 

  const props = defineProps({
    label: String,
    modelValue: String,
    placeholder: String,
    position: Object
  });

  const emit = defineEmits(['update:modelValue', 'update:position']);
  const input = ref(props.modelValue)
  const addressPosition = ref(props.position)
  const searchResults = ref<Address[]>([])
  let shouldSendRequest = true

    async function sendRequest() {
      if(input.value !== undefined && fetchAdresses.value instanceof Function)  {
        searchResults.value = (await fetchAdresses.value(input.value)) ?? [];
        }
    }

    function selectResult(result: Address) {
      input.value = `${result.name !== '' ? `${result.name}`: ''}${result.postalCode !== '' ? `, ${result.postalCode}, `: ''}${result.municipality !== '' ? `${result.municipality}`: ''}`;
      addressPosition.value = result.position;
      searchResults.value = []; // Vide la liste des résultats après la sélection
      emit('update:modelValue', input.value);
      emit('update:position', addressPosition.value)
      shouldSendRequest = false;
    }


    watch(input, (newVal) => {
      emit('update:modelValue', newVal)
      if(shouldSendRequest)
        sendRequest()
    })
</script>

<template>
  <div class="relative w-full flex flex-col">
    <input :id="label" v-model="input" :placeholder="placeholder" type="text" class="p-2 border text-sm rounded bg-white w-full" @input="sendRequest">

    <div v-if="searchResults.length > 0" class="absolute z-10 w-full top-full mt-1 border rounded-md bg-white shadow-lg overflow-hidden">
        <div class="max-h-[200px] overflow-y-auto">
          <div 
          v-for="result in searchResults" :key="result.id" 
            class="py-2 px-3 cursor-pointer hover:bg-gray-100 text-black" 
            @click="selectResult(result)"
          >
            <div>
              <span v-if="result.name">{{result.name}}</span>
              <div v-if="result.postalCode || result.municipality">
                <span v-if="result.postalCode">{{result.postalCode}}</span>
                <span v-if="result.postalCode && result.municipality">&nbsp;</span>
                <span v-if="result.municipality">{{result.municipality}}</span>
              </div>
            </div>
          </div>
      </div>
    </div>
  </div>
  </template>