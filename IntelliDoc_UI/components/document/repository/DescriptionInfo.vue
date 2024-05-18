<template>
  <v-row class="py-3">
    <v-col cols="6">
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Name</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.name }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Version</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.currentVersion }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Type</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.type }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Modified By</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">
            <span v-if="docInfo.modifiedBy">{{ docInfo.modifiedBy }}</span>
            <span class="text-muted fst-italic" v-else>Deleted Account</span>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Modified Time</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ dayjs(docInfo.modifiedDate).format('DD MMM YYYY, hh:mm A') }}</div>
        </v-col>
      </v-row>
    </v-col>
    <v-col cols="6">
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Categories</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="9" lg="9">
          <div class="text-body-2">
            <el-tag class="me-1 mb-1" effect="light" v-for="category in docInfo.category.split(', ')">{{ category }}</el-tag>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="pt-0 pb-1" cols="12" md="3" lg="3">
          <v-label class="text-caption fw-bold">Description</v-label>
        </v-col>
        <v-col class="pt-0 pb-1" cols="12" md="9" lg="9">
          <div class="text-body-2">{{ docInfo.description ?? '-' }}</div>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docInfo: Object
})

// Data

// Methods

</script>