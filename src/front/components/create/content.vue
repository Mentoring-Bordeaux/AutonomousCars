<script setup lang="ts">
import type { Position } from "~/models/address"

const addresses = ref<{
  start: string, 
  end: string
}>({start: "", end: ""})

const positions = ref<{
  start: Position, 
  end: Position
}>({start: {lat: NaN, lon: NaN}, end: {lat: NaN, lon: NaN}})

const isInForm = ref<Boolean>(true)

const handleSubmit = (payload: { departure: string; arrival: string; startPosition: Position; endPosition: Position; isInForm: Boolean }) => {
  addresses.value = {start: payload.departure, end: payload.arrival};
  positions.value = {start: payload.startPosition, end: payload.endPosition};
  isInForm.value = payload.isInForm;
}

</script>

<template>
  <div v-if="isInForm === true">
      <CreateDestinationForm @submit="handleSubmit"></CreateDestinationForm>
  </div>
  <div v-else>
    <CreateDestinationList 
      :addresses="addresses" 
      :positions="positions" />
  </div>
</template>