<script setup lang="ts">
import { defineProps, ref, watch } from 'vue'
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
      input.value = `${result.name !== '' ? `${result.name}`: ''} ${result.postalCode !== '' ? `, ${result.postalCode}`: ''} ${result.municipality}`;
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
    <input v-model="input" :placeholder="placeholder" @input="sendRequest" :id="label" type="text" class="p-2 border text-sm rounded bg-white">

    <div v-if="searchResults.length > 0" class="border rounded-md bg-white">
            <div v-if="searchResults.length > 0" class="border rounded-md bg-white">
                <div class="overflow-hidden h-[200px]">
                    <div class="flex flex-col overflow-y-scroll h-[200px]">
                    <div 
                    v-for="result in searchResults" :key="result.id" 
                        class="py-2 px-3 cursor-pointer hover:bg-white text-black" 
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
        </div>
  </template>