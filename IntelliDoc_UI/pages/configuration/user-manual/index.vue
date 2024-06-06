<template>
  <v-row>
    <v-col cols="12" md="12">
      <SharedUiCard :header="false" :footer="false">
        <v-row>
          <v-col>
            <!-- Add New User Manual Document -->
            <v-btn class="float-end" color="primary" prepend-icon="mdi-plus" flat @click="addUserManualModal = true">New User Manual Document</v-btn>
          </v-col>
        </v-row>
      </SharedUiCard>
    </v-col>
  </v-row>

  <!-- Add User Modal -->
  <SharedUiModal v-model="addUserManualModal" title="Add User Manual Document">
    <SettingCreateForm @close-modal="(e) => addUserManualModal = e"/>
  </SharedUiModal>
</template>

<script setup>
import { SettingsIcon } from "vue-tabler-icons"

// Data
const addUserManualModal = ref(false)

// Head
useHead({
  title: "User Manual | USM Document Management System",
})

// Page Meta
definePageMeta({
  breadcrumbsIcon: shallowRef(SettingsIcon),
  breadcrumbs: [
    {
      title: 'Configuration',
      disabled: false,
    },
    {
      title: 'User Manual',
      disabled: false,
    },
  ],
})

// Methods
</script>