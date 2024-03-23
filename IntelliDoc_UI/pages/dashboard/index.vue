<template>
  <!-- Dashboard Items -->
  <v-row>
    <v-col cols="12">
      <v-row>
        <v-col cols="12" lg="8">
          <DashboardDocCategories
            :doc-category="data.docCategory"
            :stored-doc-num="data.storedDocNum"
            :archived-doc-num="data.archivedDocNum"
            :y-axis-max="data.yAxisMax"
          />
        </v-col>
        <v-col cols="12" lg="4" class="d-flex flex-column">
          <div class="mb-6">
            <DashboardTotalStoredDoc :total-stored-doc="data.totalStoredDoc"/>
          </div>
          <div class="flex-grow-1">
            <DashboardTop3Categories
              :total-stored-doc="data.totalStoredDoc"
              :top3-categories-name="data.top3CategoriesName"
              v-bind:top3-categories-frequency="data.top3CategoriesFrequency"
            />
          </div>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup>
import { LayoutDashboardIcon } from "vue-tabler-icons"

// Data
const { data } = await useFetchCustom.$get("/Dashboard/DashboardData")

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