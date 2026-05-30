<template>
  <div v-if="show" class="modal-overlay" @click="handleOverlayClick">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h3 class="modal-title">{{ title }}</h3>
      </div>
      
      <div class="modal-body">
        <p class="modal-message">{{ message }}</p>
      </div>
      
      <div class="modal-footer">
        <button 
          class="btn btn-secondary"
          @click="handleCancel"
          :disabled="loading"
        >
          Cancelar
        </button>
        <button 
          class="btn btn-danger"
          @click="handleConfirm"
          :disabled="loading"
        >
          <span v-if="loading">Processando...</span>
          <span v-else>{{ confirmText }}</span>
        </button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ConfirmDialog',
  props: {
    show: {
      type: Boolean,
      default: false
    },
    title: {
      type: String,
      default: 'Confirmar'
    },
    message: {
      type: String,
      default: 'Tem certeza que deseja continuar?'
    },
    confirmText: {
      type: String,
      default: 'Confirmar'
    },
    loading: {
      type: Boolean,
      default: false
    }
  },
  emits: ['confirm', 'cancel'],
  methods: {
    handleConfirm() {
      this.$emit('confirm')
    },
    handleCancel() {
      this.$emit('cancel')
    },
    handleOverlayClick() {
      if (!this.loading) {
        this.handleCancel()
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: $z-modal;
  padding: $spacing-4;
}

.modal-content {
  background: $white;
  border-radius: $border-radius-lg;
  box-shadow: $shadow-lg;
  max-width: 400px;
  width: 100%;
  overflow: hidden;
}

.modal-header {
  padding: $spacing-6 $spacing-6 $spacing-4;
  border-bottom: 1px solid $gray-200;
}

.modal-title {
  font-size: $font-size-lg;
  font-weight: 600;
  color: $gray-900;
  margin: 0;
}

.modal-body {
  padding: $spacing-4 $spacing-6;
}

.modal-message {
  color: $gray-700;
  line-height: 1.5;
  margin: 0;
}

.modal-footer {
  padding: $spacing-4 $spacing-6 $spacing-6;
  display: flex;
  gap: $spacing-3;
  justify-content: flex-end;
}

.btn {
  padding: $spacing-2 $spacing-4;
  border: none;
  border-radius: $border-radius;
  font-size: $font-size-sm;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  
  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
  
  &.btn-secondary {
    background: $gray-100;
    color: $gray-700;
    
    &:hover:not(:disabled) {
      background: $gray-200;
    }
  }
  
  &.btn-danger {
    background: $error-color;
    color: $white;
    
    &:hover:not(:disabled) {
      background: darken($error-color, 10%);
    }
  }
}
</style>
