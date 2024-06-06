<template>
  <v-row class="m-0">
    <v-col class="pt-0">
      <v-list-item-title class="pb-1 text-body-1 fw-bold">Document Information</v-list-item-title>
      <el-scrollbar height="220px" always>
        <v-row class="m-0">
          <v-col>
            <v-row class="pb-2">
              <v-col class="pb-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Name</v-label>
              </v-col>
              <v-col class="pb-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">{{ docInfo.name }}</div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Version</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">{{ docInfo.currentVersion }}</div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Type</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">{{ docInfo.type }}</div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Modified By</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">
                  <span v-if="docInfo.modifiedBy">{{ docInfo.modifiedBy }}</span>
                  <span class="text-muted fst-italic" v-else>Deleted Account</span>
                </div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Modified Time</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">{{ dayjs(docInfo.modifiedDate).format('DD MMM YYYY, hh:mm A') }}</div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Categories</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div>
                  <el-tag class="me-1 mb-1" effect="light" v-for="category in docInfo.category.split(', ')">{{ category }}</el-tag>
                </div>
              </v-col>
            </v-row>
            <v-row class="pb-2">
              <v-col class="py-0 ps-0" cols="12" md="3" lg="3">
                <v-label class="text-caption fw-bold">Description</v-label>
              </v-col>
              <v-col class="py-0 ps-0" cols="12" md="9" lg="9">
                <div style="padding-top: 0.156rem;">{{ docInfo.description ?? '-' }}</div>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </el-scrollbar>
    </v-col>
  </v-row>
  <v-row class="m-0" v-if="edit">
    <v-col class="py-0 d-flex justify-content-end">
      <v-btn
        color="primary"
        prepend-icon="mdi-pencil-outline"
        size="small"
        variant="tonal"
        @click="emit('open-edit-modal', true)"
      >
        Edit
      </v-btn>
    </v-col>
  </v-row>
</template>

<script setup>
import dayjs from 'dayjs'

// Properties, Emit & Model
const props = defineProps({
  docInfo: Object,  
  edit: { type: Boolean, default: true },
})
const emit = defineEmits(['open-edit-modal'])

// Data

// Methods

</script>