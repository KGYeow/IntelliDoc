<template>
  <!-- Dashboard Items -->
  <v-row>
    <v-col cols="12">
    </v-col>
  </v-row>
</template>

<script setup>
import { LayoutDashboardIcon } from "vue-tabler-icons"

// Data
// const { data: dashboardData } = await fetchData.$get(`/Dashboard/Data/${userRole.value}`)

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