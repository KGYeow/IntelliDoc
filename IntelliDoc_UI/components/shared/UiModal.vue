<template>
  <v-dialog v-model="model" :width="width">
    <v-card elevation="10" class="rounded-3 overflow-visible">
      <v-card-item class="px-4 py-4 d-sm-flex align-center justify-space-between bg-background rounded-top-3 position-relative">
        <v-card-title class="text-h5 mb-0 d-flex align-center">
          {{ title }}
        </v-card-title>
        <v-btn class="top-0 end-0 m-3" density="compact" variant="plain" icon="mdi-close" position="absolute" @click="model = false"/>
      </v-card-item>
      <v-divider class="m-0"/>
      <slot/>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
// Properties
const props = defineProps({
  title: String,
  width: {
    type: String || Number || undefined,
    default: "600px"
  },
})

// Model
const model = defineModel<boolean>()
</script>