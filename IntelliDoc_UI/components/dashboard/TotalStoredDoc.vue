<template>
  <v-card elevation="10" class="withbg overflow-hidden">
    <v-card-item>
      <FileTextIcon class="position-absolute z-n1 text-grey100 top-0" size="250" style="right: -80; transform: rotate(-40deg);"/>
      <div class="d-flex align-center justify-space-between pt-sm-2">
        <v-card-title class="text-h5">Total Stored Documents</v-card-title>
        <!-- <v-btn size="large" icon  class="bg-primary">
          <v-avatar size="large" class="text-white">
            <FileTextIcon size="25"/>
          </v-avatar>
        </v-btn> -->
      </div>
      <v-row>
        <v-col cols="12">
          <div class="mt-2">
            <h2 class="text-h2 text-truncate">
              <v-tooltip :text="`${props.totalStoredDoc}`" activator="parent" location="bottom" offset="2"/>
              {{ props.totalStoredDoc }}
            </h2>
          </div>
        </v-col>
      </v-row>
    </v-card-item>
  </v-card>
</template>

<script setup>
// Properties
const props = defineProps({
  totalStoredDoc: { type: Number },
})
</script>