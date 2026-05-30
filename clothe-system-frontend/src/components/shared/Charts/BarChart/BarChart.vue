<template>
  <div :id="id" class="chart">
    <Bar
      :ref="`barChart_${id}`"
      class="chart-component"
      :options="chartOptions"
      :data="chartData"
    />
  </div>
</template>

<script>
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
)

export default {
  name: 'BarChart',
  components: { Bar },
  props: {
    id: { type: [String, Number], required: true },
    data: { type: Array, default: () => [] },
    labels: { type: Array, default: () => [] },
    chartTitle: { type: String, default: '' },
    colors: { type: Array, default: () => ['#7FB3D3', '#C8D982', '#E8B4B8', '#B8CDD9', '#F0E4D7'] },
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
          label: this.chartTitle,
          data: this.data,
          backgroundColor: this.colors.length ? this.colors.slice(0, this.data.length) : ['#7FB3D3', '#C8D982', '#E8B4B8', '#B8CDD9', '#F0E4D7'],
          borderRadius: 8,
          maxBarThickness: 50
        }]
      }
    },
    initializeChartOptions() {
      this.chartOptions = this.customOptions || {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          x: { grid: { display: false } },
          y: { beginAtZero: true, grid: { color: 'rgba(0, 0, 0, 0.1)' } }
        }
      }
    }
  }
}
</script>

<style scoped lang="scss">
@import './BarChart.scss';
</style>