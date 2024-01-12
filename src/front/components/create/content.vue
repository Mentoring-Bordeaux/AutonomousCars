<script setup lang="ts">
import type { Position } from "~/models/address"
import type { carItem } from "~/models/dropdownCar"

const addresses = ref<{
  start: string, 
  end: string
}>({start: "", end: ""});

const positions = ref<{
  start: Position, 
  end: Position
}>({start: {lat: NaN, lon: NaN}, end: {lat: NaN, lon: NaN}});

const chosenCar = ref<carItem>({name: '', icon: ''});

const isInForm = ref<boolean>(true);

const handleSubmit = (payload: { departure: string; arrival: string; startPosition: Position; endPosition: Position; chosenCar: carItem }) => {
  addresses.value = {start: payload.departure, end: payload.arrival};
  positions.value = {start: payload.startPosition, end: payload.endPosition};
  chosenCar.value = payload.chosenCar;
  isInForm.value = false;


}

</script>

<template>
  <div v-if="isInForm === true">
      <CreateDestinationForm @submit="handleSubmit"></CreateDestinationForm>
  </div>
  <div v-else>
    <CreateDestinationList 
    :addresses="addresses" 
    :positions="positions"
    :chosen-car="chosenCar"
    @update:is-in-form="isInForm = $event" />
  </div>
</template>