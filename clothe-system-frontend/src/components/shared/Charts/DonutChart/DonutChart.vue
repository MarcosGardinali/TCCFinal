<template>
  <div :id="id" class="chart">
    <Doughnut
      :ref="`donutChart_${id}`"
      class="chart-component"
      :options="chartOptions"
      :data="chartData"
    />
  </div>
</template>

<script>
import { Doughnut } from 'vue-chartjs'
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(
  ArcElement,
  Tooltip,
  Legend
)

export default {
  name: 'DonutChart',
  components: { Doughnut },
  props: {
    id: { type: [String, Number], required: true },
    data: { type: Array, default: () => [] },
    labels: { type: Array, default: () => [] },
    colors: { type: Array, default: () => ['#7FB3D3', '#C8D982', '#E8B4B8', '#B8CDD9', '#F0E4D7'] },
    cutout: { type: String, default: '60%' },
    showPercentage: { type: Boolean, default: true },
    customOptions: { type: Object, default: null }
  },
  data: () => ({
    chartData: {},
    chartOptions: {}
  }),
  watch: {
    data: { handler() { this.initializeChartData() }, deep: true },
    labels: { handler() { this.initializeChartData() }, deep: true }
  },
  created() {
    this.initializeChartData()
    this.initializeChartOptions()
  },
  methods: {
    initializeChartData() {
      this.chartData = {
        labels: this.labels,
        datasets: [{
          data: this.data,
          backgroundColor: this.colors.slice(0, this.data.length),
          borderWidth: 0,
          hoverBorderWidth: 2,
          hoverBorderColor: '#ffffff'
        }]
      }
    },
    initializeChartOptions() {
      this.chartOptions = this.customOptions || {
        responsive: true,
        maintainAspectRatio: false,
        cutout: this.cutout,
        plugins: {
          legend: {
            display: true,
            position: 'bottom',
            labels: { usePointStyle: true, padding: 20 }
          }
        }
      }
    }
  }
}
</script>

<style scoped lang="scss">
@import './DonutChart.scss';
</style>