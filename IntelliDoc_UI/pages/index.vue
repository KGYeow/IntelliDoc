<template>
  <v-row>
    <v-col cols="12">
      <v-row>
        <!-- Sales overview -->
        <v-col cols="12" lg="12">
          <SalesOverview />
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
/*Call Components*/
import SalesOverview from '@/components/dashboard/SalesOverview.vue';
</script>