<template>
  <v-card elevation="10" class="withbg">
    <v-card-item>
      <div class="d-sm-flex align-center justify-space-between pt-sm-2">
        <div>
          <v-card-title class="text-h5">Document Categories</v-card-title>
        </div>
      </div>
      <div class="mt-6">
        <apexchart type="bar" height="400px" :options="chartOptions.chartOptions" :series="chartOptions.series"/>
      </div>
    </v-card-item>
  </v-card>
</template>

<script setup>
import { useTheme } from 'vuetify'
import * as dashboardData from '@/data/dashboard/dashboardData'

// Data
const theme = useTheme()
const primary = theme.current.value.colors.primary
const secondary = theme.current.value.colors.secondary
const chartOptions = computed(() => {
  return {
    series: [
      { name: "Stored Document", data: dashboardData.storedDocNum },
      { name: "Archived Document", data: dashboardData.archivedDocNum },
    ],
    chartOptions: {
      grid: {
        borderColor: 'rgba(0,0,0,0.1)',
        strokeDashArray: 3,
        xaxis: {
          lines: {
            show: false
          }
        },
      },
      plotOptions: {
        bar: { horizontal: false, columnWidth: "35%", barHeight: "65%", borderRadius: [5]},
      },
      colors: [primary, secondary],
      chart: {
        type: "bar",
        height: 400,
        offsetX: 0,
        offsetY: 0,
        toolbar: { show: false },
        foreColor: "#adb0bb",
        fontFamily: 'inherit',
        sparkline: { enabled: false },
      },
      dataLabels: { enabled: false },
      markers: { size: 3 },
      legend: { show: true },
      xaxis: {
        type: "category",
        categories: dashboardData.docCategory,
        labels: {
          style: { cssClass: "grey--text lighten-2--text fill-color" },
        },
      },
      yaxis: {
        show: true,
        min: 0,
        max: dashboardData.yAxisMax,
        tickAmount: 4,
        labels: {
          style: {
            cssClass: "grey--text lighten-2--text fill-color",
          },
        },
      },
      stroke: {
        show: true,
        width: 3,
        lineCap: "butt",
        colors: ["transparent"],
      },
      tooltip: { theme: "light" },
      responsive: [{
        breakpoint: 600,
        options: {
          plotOptions: {
            bar: {
              borderRadius: 3,
            }
          },
        }
      }]
    }
  }
})
</script>