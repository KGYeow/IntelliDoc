<template>
  <v-row class="pt-3">
    <v-col>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Name</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">{{ docInfo.name }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Version</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">{{ docInfo.currentVersion }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Type</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">{{ docInfo.type }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Modified By</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">
            <span v-if="docInfo.modifiedBy">{{ docInfo.modifiedBy }}</span>
            <span class="text-muted fst-italic" v-else>Deleted Account</span>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Modified Time</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">{{ dayjs(docInfo.modifiedDate).format('DD MMM YYYY, hh:mm A') }}</div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Categories</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2">
            <el-tag class="me-1 mb-1" effect="light" v-for="category in docInfo.category.split(', ')">{{ category }}</el-tag>
          </div>
        </v-col>
      </v-row>
      <v-row class="pb-2">
        <v-col class="py-0" cols="12" md="2" lg="1">
          <v-label class="text-caption fw-bold">Description</v-label>
        </v-col>
        <v-col class="py-0" cols="12" md="10" lg="11">
          <div class="text-body-2" style="padding-top: 0.156rem;">{{ docInfo.description ?? '-' }}</div>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
  <v-row>
    <v-col class="d-flex justify-content-end">
      <v-btn color="primary" prepend-icon="mdi-file-document-multiple-outline" size="small" flat @click="emit('open-related-doc', true)">Related Documents</v-btn>
      <v-btn color="primary" prepend-icon="mdi-pencil-outline" size="small" flat @click="emit('open-edit-modal', true)" class="ms-1">Edit</v-btn>
    </v-col>
  </v-row>
</template>

<script setup>
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docInfo: Object
})
const emit = defineEmits(['open-edit-modal', 'open-related-doc'])

// Data

// Methods

</script>