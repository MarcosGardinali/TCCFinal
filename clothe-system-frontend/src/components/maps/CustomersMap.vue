<template>
  <div class="map-container">
    <div ref="mapContainer" class="map"></div>
    <div v-if="loading" class="map-loading">
      <div class="loading-spinner"></div>
      <p v-if="customers.length > 0">Geocodificando endereços dos clientes...</p>
      <p v-else>Carregando mapa...</p>
      <small v-if="customers.length > 0">{{ processedCustomers }}/{{ customers.length }} processados</small>
    </div>
  </div>
</template>

<script>
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

delete L.Icon.Default.prototype._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
})

export default {
  name: 'CustomersMap',
  props: {
    customers: {
      type: Array,
      default: () => []
    }
  },
  data() {
    return {
      map: null,
      markers: [],
      loading: true,
      processedCustomers: 0,
      geocodeCache: new Map()
    }
  },
  mounted() {
    this.initMap()
  },
  beforeUnmount() {
    if (this.map) this.map.remove()
  },
  watch: {
    customers: {
      handler() {
        this.updateMarkers()
      },
      deep: true
    }
  },
  methods: {
    initMap() {
      const centerLat = -14.235
      const centerLng = -51.9253

      this.map = L.map(this.$refs.mapContainer).setView([centerLat, centerLng], 4)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
      }).addTo(this.map)

      this.loading = false
      this.$nextTick(() => this.updateMarkers())
    },

    async updateMarkers() {
      if (!this.map) return

      this.loading = true
      this.processedCustomers = 0

      this.markers.forEach(marker => this.map.removeLayer(marker))
      this.markers = []

      if (this.customers && this.customers.length > 0) {
        const batchSize = 3
        for (let i = 0; i < this.customers.length; i += batchSize) {
          const batch = this.customers.slice(i, i + batchSize)

          await Promise.all(
            batch.map(async (customer) => {
              await this.addCustomerMarker(customer)
              this.processedCustomers++
            })
          )

          if (i + batchSize < this.customers.length) {
            await new Promise(resolve => setTimeout(resolve, 800))
          }
        }

        if (this.markers.length > 0) {
          const group = new L.featureGroup(this.markers)
          this.map.fitBounds(group.getBounds().pad(0.1))
        }
      }

      this.loading = false
    },

    async addCustomerMarker(customer) {
      console.log(customer)
      try {
        const coordinates = await this.geocodeAddress(customer)

        if (!coordinates) return

        const marker = L.marker([coordinates.lat, coordinates.lng]).addTo(this.map)

        const openOrder = customer.orders && customer.orders.length > 0 ? customer.orders[0] : null;

        const popupContent = `
          <div class="customer-popup">
            <h4>${customer.firstName} ${customer.lastName}</h4>
            <p><strong>Email:</strong> ${customer.email || 'Não informado'}</p>
            <p><strong>Telefone:</strong> ${this.formatPhone(customer.mobilePhoneNumber)}</p>
            <p><strong>Endereço:</strong> ${customer.street || ''}, ${customer.number || ''} - ${customer.cityName || 'Não informado'}</p>
            ${openOrder ? `
              <hr>
              <h5>Pedido em Aberto</h5>
              <p><strong>ID do Pedido:</strong> #${openOrder.id}</p>
              <p><strong>Data:</strong> ${new Date(openOrder.creationDate).toLocaleDateString('pt-BR')}</p>
            ` : ''}
          </div>
        `;

        marker.bindPopup(popupContent)
        this.markers.push(marker)
      } catch (error) {
        console.warn('Erro ao geocodificar endereço do cliente:', customer.firstName, error)
      }
    },

    async geocodeAddress(customer) {
      const addressParts = []
      if (customer.street) addressParts.push(customer.street)
      if (customer.number) addressParts.push(customer.number)
      if (customer.cityName) addressParts.push(customer.cityName)
      if (customer.stateAbbreviation) addressParts.push(customer.stateAbbreviation)
      addressParts.push('Brasil')

      const qString = addressParts.join(', ')

      const params = new URLSearchParams({
        format: 'json',
        limit: '1',
        addressdetails: '1',
        countrycodes: 'br',
        q: qString
      })

      const cacheKey = params.toString()
      if (this.geocodeCache.has(cacheKey)) return this.geocodeCache.get(cacheKey)

      try {
        const response = await fetch(`https://nominatim.openstreetmap.org/search?${params}`)
        if (!response.ok) throw new Error('Erro na requisição de geocodificação')

        const data = await response.json()
        if (data && data.length > 0) {
          const coordinates = { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon) }
          this.geocodeCache.set(cacheKey, coordinates)
          return coordinates
        }

        return null
      } catch (error) {
        console.warn('Erro ao geocodificar endereço:', cacheKey, error)
        return null
      }
    },

    formatPhone(phone) {
      if (!phone) return 'Não informado'
      const cleaned = phone.replace(/\D/g, '')
      if (cleaned.length === 11) return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 7)}-${cleaned.slice(7)}`
      return phone
    }
  }
}
</script>

<style lang="scss" scoped>
.map-container {
  position: relative;
  height: 400px;
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.map { height: 100%; width: 100%; }

.map-loading {
  position: absolute;
  top:0; left:0; right:0; bottom:0;
  background: linear-gradient(135deg, rgba(255,255,255,0.95) 0%, rgba(248,250,252,0.95) 100%);
  backdrop-filter: blur(10px);
  display:flex; flex-direction:column; align-items:center; justify-content:center;
  z-index:1000; border-radius:8px;

  .loading-spinner {
    width:50px; height:50px;
    border:4px solid rgba(102,126,234,0.1);
    border-top:4px solid #667eea;
    border-radius:50%;
    animation:spin 1s linear infinite;
    margin-bottom:20px;
    box-shadow:0 4px 8px rgba(0,0,0,0.1);
  }

  p { color:#374151; font-weight:600; font-size:16px; margin:0 0 8px 0; text-align:center; }
  small { color:#6b7280; font-size:14px; font-weight:500; }
}

@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }

:global(.customer-popup) {
  h4 { margin:0 0 8px 0; color:#374151; font-size:16px; font-weight:600; }
  p { margin:4px 0; font-size:14px; color:#6b7280; strong { color:#374151; } }
}
</style>
