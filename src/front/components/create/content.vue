<script setup lang="ts">
import type { Position } from "~/models/address"

const dataShared = ref<{
  startAddress: String, 
  endAddress: String,
  startPostion: Position, 
  endPosition: Position, 
  isInForm: Boolean
}>();

const addresses = ref<{
  start: String, 
  end: String
}>()

const positions = ref<{
  start: Position, 
  end: Position
}>()

const handleSubmit = (payload) => {
  addresses.value = payload.addresses;
  positions.value = payload.positions;
  isInForm.value = payload.isInForm;
}

const startPosition = ref<Position>({lat: NaN, lon: NaN})
const endPosition = ref<Position>({lat: NaN, lon: NaN})
const isInForm = ref<Boolean>(true)



</script>

<template>
  <!-- <CreateDestinationList :start-position="startPosition" 
      :end-position="endPosition"/> -->
  <div v-if="isInForm === true">
    <CreateDestinationForm 
    v-model:start-position="startPosition" 
      v-model:end-position="endPosition"
      v-model:is-in-form="isInForm"
      @update:start-position="startPosition = $event" 
      @update:end-position="endPosition = $event"
      @update:is-in-form="isInForm = $event"/>
  </div>
  <div v-else>
    <CreateDestinationList 
      :start-position="startPosition" 
      :end-position="endPosition"/> 
  </div>
</template>