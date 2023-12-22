<script lang="ts">
import { defineComponent, ref, watch } from 'vue'
import { Address } from "~/models/address"
import { useAzureMapsAPI } from "~/composables/useAzureMapsAPI"


export default defineComponent({
  props: {
    label: String,
    modelValue: String,
    placeholder: String,
    position: Object
  },
  emits: ['update:modelValue', 'update:position'],
  setup(props, { emit }) {
    const input = ref(props.modelValue)
    const addressPosition = ref(props.position)
    const { fetchAdresses } = useAzureMapsAPI()
    const searchResults = ref<Address[]>([])
    let shouldSendRequest = true

    async function sendRequest() {
      if(input.value != undefined)  {
        searchResults.value = (await fetchAdresses(input.value)) ?? [];
        //console.log(searchResults.value)
        }
    }

    function selectResult(result: Address) {
      input.value = result.name + ', ' + result.postalCode + ', ' +  result.municipality;
      addressPosition.value = result.position;
      searchResults.value = []; // Vide la liste des résultats après la sélection
      //console.log("Position sauvegardé : ", addressPosition.value);
      emit('update:modelValue', input.value);
      emit('update:position', addressPosition.value)
      shouldSendRequest = false;
    }


    watch(input, (newVal) => {
      emit('update:modelValue', newVal)
      if(shouldSendRequest)
        sendRequest()
    })

    return { input, searchResults, sendRequest, selectResult }
  }
})
</script>

<template>
    <input @input="sendRequest" :id="label" v-model="input" type="text" :placeholder="placeholder" class="p-2 border rounded bg-white">

    <div v-if="searchResults.length > 0" class="border rounded-md bg-white">
            <div v-if="searchResults.length > 0" class="border rounded-md bg-white">
                <div class="overflow-hidden h-[200px]">
                    <div class="flex flex-col overflow-y-scroll h-[200px]">
                    <div v-for="result in searchResults" :key="result.id" 
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