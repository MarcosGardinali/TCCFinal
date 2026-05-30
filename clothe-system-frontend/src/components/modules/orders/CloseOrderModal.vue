<template>
  <BaseModal
    :show="show"
    title="Finalizar Pedido"
    subtitle="Defina o status de cada item antes de finalizar"
    confirm-type="success"
    confirm-text="Finalizar Pedido"
    :loading="loading"
    :confirm-disabled="!canClose"
    @close="$emit('close')"
    @confirm="$emit('confirm')"
  >
    <div class="close-order-content">
      <p class="close-order-instruction">
        Para finalizar o pedido <strong>#{{ order?.id }}</strong>, você deve definir o status de todos os itens:
      </p>
      
      <div v-if="items.length > 0" class="items-status-list">
        <div v-for="item in items" :key="item.id" class="item-status-row">
          <div class="item-info">
            <h4 class="item-name">{{ getItemName(item) }}</h4>
            <p v-if="item.variation" class="item-variation">{{ item.variation }}</p>
          </div>
          <div class="item-status-selector">
            <label class="status-option">
              <input 
                type="radio" 
                :name="`item-${item.id}`" 
                :value="2" 
                v-model="item.newStatus"
              >
              <span class="status-label bought">Comprado</span>
            </label>
            <label class="status-option">
              <input 
                type="radio" 
                :name="`item-${item.id}`" 
                :value="1" 
                v-model="item.newStatus"
              >
              <span class="status-label returned">Devolvido</span>
            </label>
          </div>
        </div>
      </div>
      
      <div v-else class="no-items-message">
        <p>Este pedido não possui itens pendentes. Pode ser finalizado diretamente.</p>
      </div>
      
      <div v-if="items.length > 0 && !canClose" class="warning-message">
        <i class="fas fa-exclamation-triangle"></i>
        Todos os itens devem ter um status definido para finalizar o pedido.
      </div>
    </div>
  </BaseModal>
</template>

<script>
import BaseModal from '@/components/shared/BaseModal/BaseModal.vue'

export default {
  name: 'CloseOrderModal',
  components: {
    BaseModal
  },
  props: {
    show: {
      type: Boolean,
      default: false
    },
    order: {
      type: Object,
      default: null
    },
    items: {
      type: Array,
      default: () => []
    },
    loading: {
      type: Boolean,
      default: false
    }
  },
  emits: ['close', 'confirm'],
  computed: {
    canClose() {
      return this.items.length === 0 || this.items.every(item => item.newStatus !== null && item.newStatus !== undefined)
    }
  },
  methods: {
    getItemName(item) {
      if (item.product?.description) return item.product.description
      if (item.product?.name) return item.product.name
      if (item.productName) return item.productName
      if (item.name) return item.name
      return 'Produto não encontrado'
    }
  }
}
</script>

<style lang="scss" scoped>
.close-order-content {
  padding: $spacing-2 0;
}

.close-order-instruction {
  margin-bottom: $spacing-5;
  color: $text-secondary;
  font-size: $font-size-base;
  line-height: 1.5;
}

.items-status-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
  margin-bottom: $spacing-5;
}

.item-status-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-4;
  background: $gray-50;
  border-radius: $border-radius-md;
  border: 1px solid $border;
}

.item-info .item-name {
  font-size: $font-size-base;
  font-weight: 500;
  color: $text-primary;
  margin: 0 0 4px 0;
}

.item-info .item-variation {
  font-size: $font-size-sm;
  color: $text-secondary;
  margin: 0;
  font-style: italic;
}

.item-status-selector {
  display: flex;
  gap: $spacing-3;
}

.status-option {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  cursor: pointer;
  padding: $spacing-2 $spacing-3;
  border-radius: $border-radius-md;
  transition: all 0.2s ease;
  border: 1px solid transparent;

  &:hover {
    background: $gray-100;
  }
  
  input[type="radio"] {
    margin: 0;
    accent-color: $primary-color;
  }
}

.status-label {
  font-size: $font-size-sm;
  font-weight: 500;
  
  &.bought {
    color: $success-color;
  }
  
  &.returned {
    color: $warning-color;
  }
}

.no-items-message {
  text-align: center;
  padding: $spacing-6;
  color: $text-secondary;
  font-style: italic;
}

.warning-message {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3 $spacing-4;
  background: rgba($warning-color, 0.1);
  border-left: 4px solid $warning-color;
  color: darken($warning-color, 20%);
  font-size: $font-size-sm;
  
  i {
    color: $warning-color;
  }
}

@media (max-width: 768px) {
  .item-status-row {
    flex-direction: column;
    align-items: stretch;
    gap: $spacing-3;
  }
  
  .item-status-selector {
    justify-content: center;
  }
}
</style>