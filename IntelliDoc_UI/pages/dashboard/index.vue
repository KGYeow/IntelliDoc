<template>
  <!-- Dashboard Items -->
  <v-row>
    <v-col cols="12">
      <v-row>
        <v-col cols="12" lg="8">
          <DashboardDocCategories/>
        </v-col>
        <v-col cols="12" lg="4" class="d-flex flex-column">
          <div class="mb-6">
            <DashboardTotalStoredDoc/>
          </div>
          <div class="flex-grow-1">
            <DashboardTop3Categories/>
          </div>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import { LayoutDashboardIcon } from "vue-tabler-icons"

// Head
useHead({
  title: "Dashboard | USM Document Management System",
})

// Page Meta
definePageMeta({
  breadcrumbsIcon: shallowRef(LayoutDashboardIcon),
  breadcrumbs: [
    {
      title: 'Dashboard',
      disabled: false,
    },
  ],
})
</script>