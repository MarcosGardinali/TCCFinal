<template>
  <div :id="id" class="chart">
    <Line
      :ref="`lineChart_${id}`"
      class="chart-component"
      :options="chartOptions"
      :data="chartData"
    />
  </div>
</template>

<script>
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

export default {
  name: 'LineChart',
  components: { Line },
  props: {
    id: { type: [String, Number], required: true },
    data: { type: Array, default: () => [] },
    labels: { type: Array, default: () => [] },
    chartTitle: { type: String, default: '' },
    color: { type: String, default: '#7FB3D3' },
    fill: { type: Boolean, default: true },
    tension: { type: Number, default: 0.4 },
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
          borderColor: this.color,
          backgroundColor: this.fill ? `${this.color}20` : 'transparent',
          borderWidth: 3,
          fill: this.fill,
          tension: this.tension,
          pointBackgroundColor: this.color,
          pointBorderColor: '#ffffff',
          pointBorderWidth: 2,
          pointRadius: 6
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
@import './LineChart.scss';
</style>