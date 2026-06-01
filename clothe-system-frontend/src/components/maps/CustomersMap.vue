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
      geocodeCache: new Map(),
      customerOrders: new Map() // Agrupar pedidos por cliente
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
      this.customerOrders.clear()

      this.markers.forEach(marker => this.map.removeLayer(marker))
      this.markers = []

      console.log('[CustomersMap] Recebido:', this.customers)

      if (this.customers && this.customers.length > 0) {
        // Agrupar pedidos por cliente
        for (const order of this.customers) {
          const customerId = order.customer?.id || order.customerId
          if (!this.customerOrders.has(customerId)) {
            this.customerOrders.set(customerId, {
              customer: order.customer,
              orders: []
            })
          }
          this.customerOrders.get(customerId).orders.push(order)
        }

        const batchSize = 3
        const customers = Array.from(this.customerOrders.values())
        
        for (let i = 0; i < customers.length; i += batchSize) {
          const batch = customers.slice(i, i + batchSize)

          await Promise.all(
            batch.map(async (custData) => {
              await this.addCustomerMarker(custData.customer, custData.orders)
              this.processedCustomers++
            })
          )

          if (i + batchSize < customers.length) {
            await new Promise(resolve => setTimeout(resolve, 800))
          }
        }

        console.log('[CustomersMap] Total de markers adicionados:', this.markers.length)

        if (this.markers.length > 0) {
          const group = new L.featureGroup(this.markers)
          this.map.fitBounds(group.getBounds().pad(0.1))
        }
      }

      this.loading = false
    },

    async addCustomerMarker(customer, orders = []) {
      console.log('[addCustomerMarker] Customer:', customer)
      console.log('[addCustomerMarker] Orders:', orders)

      if (!customer) return

      try {
        const coordinates = await this.geocodeAddress(customer)

        if (!coordinates) {
          console.warn('[addCustomerMarker] Sem coordenadas para:', customer?.firstName || customer?.FirstName)
          return
        }

        // Usar marker padrão do Leaflet
        const marker = L.marker([coordinates.lat, coordinates.lng]).addTo(this.map)

        const get = (obj, ...keys) => {
          for (const k of keys) {
            if (!obj) continue
            if (obj[k] !== undefined && obj[k] !== null) return obj[k]
          }
          return null
        }

        const popupContent = `
          <div class="map-popup" data-customer-id="${customer.id}">
            <div class="popup-header">
              <h4>${get(customer, 'firstName', 'FirstName', 'primeiroNome') || ''} ${get(customer, 'lastName', 'LastName', 'sobrenome') || ''}</h4>
              <p class="customer-contact">
                ${get(customer, 'email', 'Email') || 'Não informado'} | 
                ${this.formatPhone(get(customer, 'mobilePhoneNumber', 'numero_celular', 'Number'))}
              </p>
              <p class="customer-address">
                ${get(customer, 'street', 'endereco') || ''}, ${get(customer, 'number', 'numero') || ''} - ${get(customer, 'cityName', 'nome_cidade') || 'Não informado'}
              </p>
            </div>

            <div class="popup-orders">
              <h5>Pedidos em Aberto (${orders.length})</h5>
              
              ${orders.length > 0 ? `
                <div class="orders-list">
                  ${orders.map((order) => {
                    const orderId = get(order, 'id', 'Id', 'number', 'Number')
                    const rawDate = get(order, 'creationDate', 'CreationDate', 'dataCadastro', 'data_cadastro')
                    const orderDate = rawDate ? new Date(rawDate) : null
                    const totalValue = get(order, 'totalValue', 'TotalValue', 'billedValue', 'BilledValue')
                    
                    return `
                      <details class="order-summary">
                        <summary class="summary-header">
                          <span class="order-id">Pedido #${orderId || order.id}</span>
                          <span class="order-total">${totalValue ? this.formatCurrency(totalValue) : 'R$ 0,00'}</span>
                        </summary>
                        <div class="summary-content">
                          <div class="detail-row">
                            <label>Data:</label>
                            <span>${orderDate ? orderDate.toLocaleDateString('pt-BR') : 'Não informado'}</span>
                          </div>
                          <button class="btn-view-order" data-order-id="${order.id}">Ver Detalhes →</button>
                        </div>
                      </details>
                    `
                  }).join('')}
                </div>
              ` : '<p class="no-orders">Nenhum pedido em aberto</p>'}
            </div>
          </div>
        `;

        marker.bindPopup(popupContent, {
          className: 'custom-popup',
          maxWidth: 340,
          minWidth: 300
        })

        // Adicionar event listeners após o popup ser aberto
        marker.on('popupopen', () => {
          this.setupPopupEventListeners(customer.id, orders)
        })

        this.markers.push(marker)
      } catch (error) {
        console.error('[addCustomerMarker] Erro ao processar marker:', error)
      }
    },

    setupPopupEventListeners(customerId, orders) {
      const popup = document.querySelector(`[data-customer-id="${customerId}"]`)
      if (!popup) return

      // Event listeners para os botões de ver detalhes
      const viewButtons = popup.querySelectorAll('.btn-view-order')
      viewButtons.forEach(btn => {
        btn.addEventListener('click', (e) => {
          e.stopPropagation()
          const orderId = btn.getAttribute('data-order-id')
          this.$router.push(`/orders/${orderId}`)
        })
      })
    },

    async geocodeAddress(customer) {
      const addressParts = []
      if (customer.street) addressParts.push(String(customer.street).trim())
      if (customer.number) addressParts.push(String(customer.number).trim())
      if (customer.cityName) addressParts.push(String(customer.cityName).trim())
      
      // Validar state abbreviation - deve ter 2 caracteres de letra
      const stateAbbr = customer.stateAbbreviation ? String(customer.stateAbbreviation).trim() : null
      if (stateAbbr && stateAbbr.length === 2 && /^[A-Z]{2}$/i.test(stateAbbr)) {
        addressParts.push(stateAbbr.toUpperCase())
      }
      
      addressParts.push('Brasil')

      const qString = addressParts.join(', ')
      console.log('[geocodeAddress] Endereço:', qString, 'Customer:', customer)

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
        console.log('[geocodeAddress] Fazendo requisição para:', `https://nominatim.openstreetmap.org/search?${params}`)
        const response = await fetch(`https://nominatim.openstreetmap.org/search?${params}`)
        console.log('[geocodeAddress] Response status:', response.status)
        if (!response.ok) throw new Error('Erro na requisição de geocodificação')

        const data = await response.json()
        console.log('[geocodeAddress] Dados retornados:', data)
        if (data && data.length > 0) {
          const coordinates = { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon) }
          console.log('[geocodeAddress] Coordenadas encontradas:', coordinates)
          this.geocodeCache.set(cacheKey, coordinates)
          return coordinates
        }

        console.log('[geocodeAddress] Nenhuma coordenada encontrada para:', qString)
        return null
      } catch (error) {
        console.warn('[geocodeAddress] Erro ao geocodificar:', cacheKey, error)
        return null
      }
    },

    formatPhone(phone) {
      if (!phone) return 'Não informado'
      const cleaned = phone.replace(/\D/g, '')
      if (cleaned.length === 11) return `(${cleaned.slice(0, 2)}) ${cleaned.slice(2, 7)}-${cleaned.slice(7)}`
      return phone
    }
    ,
    formatCurrency(value) {
      try {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value)
      } catch {
        return value || 'R$ 0,00'
      }
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

:global(.custom-popup) {
  .leaflet-popup-content-wrapper {
    border-radius: 8px;
    box-shadow: 0 3px 12px rgba(0, 0, 0, 0.15);
    border: none;
    padding: 0 !important;
  }

  .leaflet-popup-content {
    margin: 0 !important;
    padding: 0 !important;
    width: auto !important;
  }

  .leaflet-popup-tip {
    box-shadow: none;
  }
}

:global(.custom-popup .map-popup) {
  font-size: 13px;
  color: #374151;
  display: block !important;
}

:global(.custom-popup .popup-header) {
  background: #f3f4f6;
  padding: 12px 14px;
  border-bottom: 1px solid #e5e7eb;
  border-radius: 8px 8px 0 0;

  h4 {
    margin: 0 0 6px 0;
    font-size: 15px;
    font-weight: 600;
    color: #111827;
  }

  p {
    margin: 0;
    font-size: 12px;
  }

  .customer-contact {
    margin: 4px 0;
    color: #6b7280;
  }

  .customer-address {
    margin: 4px 0 0 0;
    color: #6b7280;
    line-height: 1.3;
  }
}

:global(.custom-popup .popup-orders) {
  padding: 12px 14px;

  h5 {
    margin: 0 0 12px 0;
    font-size: 12px;
    font-weight: 600;
    color: #667eea;
    text-transform: uppercase;
    letter-spacing: 0.3px;
  }

  .no-orders {
    text-align: center;
    color: #9ca3af;
    font-size: 12px;
    margin: 0;
    padding: 10px 0;
  }
}

:global(.custom-popup .orders-list) {
  display: flex;
  flex-direction: column;
  gap: 8px;

  .order-summary {
    border: 1px solid #e5e7eb;
    border-radius: 6px;
    overflow: hidden;

    .summary-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 10px 12px;
      background: linear-gradient(135deg, #f9fafb 0%, #ffffff 100%);
      cursor: pointer;
      user-select: none;
      font-weight: 500;
      color: #374151;

      &:hover {
        background: linear-gradient(135deg, #f3f4f6 0%, #f9fafb 100%);
      }

      .order-id {
        font-weight: 600;
        color: #667eea;
        font-size: 13px;
      }

      .order-total {
        color: #667eea;
        font-weight: 700;
        font-size: 13px;
      }
    }

    &[open] .summary-header {
      border-bottom: 1px solid #e5e7eb;
      background: #f3f4f6;
    }

    .summary-content {
      padding: 10px 12px;
      background: #ffffff;
      display: flex;
      flex-direction: column;
      gap: 10px;

      .detail-row {
        display: flex;
        justify-content: space-between;
        font-size: 12px;

        label {
          color: #6b7280;
          font-weight: 500;
        }

        span {
          color: #374151;
          font-weight: 500;
        }
      }
    }
  }
}

:global(.custom-popup .btn-view-order) {
  width: 100% !important;
  padding: 8px 12px !important;
  background: #667eea !important;
  color: white !important;
  border: none !important;
  border-radius: 4px !important;
  font-size: 12px !important;
  font-weight: 600 !important;
  cursor: pointer !important;
  transition: background 0.2s ease !important;
  margin-top: 4px !important;
  display: block !important;
  box-sizing: border-box !important;
}

:global(.custom-popup .btn-view-order:hover) {
  background: #5568d3 !important;
}

:global(.custom-popup .btn-view-order:active) {
  transform: scale(0.98) !important;
}
</style>
