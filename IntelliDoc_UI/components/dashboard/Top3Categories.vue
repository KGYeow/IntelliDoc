<template>
  <v-card elevation="10" class="withbg h-100">
    <v-card-item>
      <div class="d-sm-flex align-center justify-space-between pt-sm-2">
        <v-card-title class="text-h5">Top 3 Most Frequent Categories</v-card-title>
      </div>
      <v-row v-if="dashboardData.totalStoredDoc != 0">
        <v-col cols="5" sm="12" class="pl-lg-0">
          <div class="d-flex align-center flex-shrink-0 justify-content-center">
            <apexchart class="pt-6" type="donut" height="185" :options="chartOptions" :series="Chart"/>
          </div>
        </v-col>
        <v-col cols="7" sm="12">
          <div class="d-flex align-center mt-0">
            <h6 class="text-subtitle-1 text-muted d-flex align-center" v-if="dashboardData.top3CategoriesFrequency[0] > 0">
              <v-icon icon="mdi mdi-checkbox-blank-circle" class="mr-1" size="10" color="primary"/>
              {{ dashboardData.top3CategoriesName[0] }}
            </h6>
            <h6 class="text-subtitle-1 text-muted d-flex align-center pl-5" v-if="dashboardData.top3CategoriesFrequency[1] > 0">
              <v-icon icon="mdi mdi-checkbox-blank-circle" class="mr-1" size="10" color="lightprimary"/>
              {{ dashboardData.top3CategoriesName[1] }}
            </h6>
            <h6 class="text-subtitle-1 text-muted d-flex align-center pl-5" v-if="dashboardData.top3CategoriesFrequency[2] > 0">
              <v-icon icon="mdi mdi-checkbox-blank-circle" class="mr-1" size="10" color="#F9F9FD"/>
              {{ dashboardData.top3CategoriesName[2] }}
            </h6>
          </div>
        </v-col>
      </v-row>
      <v-row class="justify-content-center" v-else>
        <v-col cols="12" class="pl-lg-0">
          <el-empty class="p-0 pt-2" :image-size="190" description="No document"/>
        </v-col>
      </v-row>
    </v-card-item>
  </v-card>
</template>

<script setup>
import { useTheme } from 'vuetify'
import * as dashboardData from '@/data/dashboard/dashboardData'

// Data
const theme = useTheme()
const primary = theme.current.value.colors.primary
const lightprimary = theme.current.value.colors.lightprimary
const Chart = dashboardData.top3CategoriesFrequency
const chartOptions = computed(() => {
  return {
    labels: dashboardData.top3CategoriesName,
    chart: {
      type: 'donut',
      fontFamily: `inherit`,
      foreColor: '#a1aab2',
      toolbar: {
        show: false
      }
    },
    colors: [primary, lightprimary, '#F9F9FD'],
    plotOptions: {
      pie: {
        startAngle: 0,
        endAngle: 360,
        donut: {
          size: '75%',
          background: 'transparent'
        }
      }
    },
    stroke: {
      show: false
    },
    dataLabels: {
      enabled: false
    },
    legend: {
      show: false
    },
    tooltip: { theme: "light", fillSeriesColor: false },
  }
})
</script>