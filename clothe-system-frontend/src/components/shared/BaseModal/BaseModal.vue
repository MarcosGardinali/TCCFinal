<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="modal-overlay" @click="handleOverlayClick">
        <div
          class="modal"
          :class="[sizeClass, { 'modal-fullscreen': fullscreen }]"
          @click.stop
        >
          <!-- Header -->
          <div v-if="!hideHeader" class="modal-header">
            <div class="modal-title-section">
              <h3 class="modal-title">{{ title }}</h3>
              <p v-if="subtitle" class="modal-subtitle">{{ subtitle }}</p>
            </div>
            <button
              v-if="!hideCloseButton"
              @click="handleClose"
              class="btn-close"
              :disabled="loading"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>

          <!-- Body -->
          <div class="modal-body" :class="{ 'no-padding': noPadding }">
            <slot></slot>
          </div>

          <!-- Footer -->
          <div v-if="!hideFooter" class="modal-footer">
            <slot name="footer">
              <BaseButton
                variant="secondary"
                @click="handleCancel"
                :disabled="loading"
              >
              {{ cancelText }}
              </BaseButton>
              <BaseButton
                v-if="!hideConfirmButton"
                @click="handleConfirm"
                class="btn"
                :class="confirmButtonClass"
                :disabled="loading || confirmDisabled"
              >
              {{ confirmText }}
              </BaseButton>
            </slot>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script>
import { computed } from 'vue'
import BaseButton from '@/components/shared/BaseButton/BaseButton.vue'

export default {
  name: 'BaseModal',
  components: {
    BaseButton
  },
  props: {
    show: {
      type: Boolean,
      default: false
    },
    title: {
      type: String,
      default: ''
    },
    subtitle: {
      type: String,
      default: ''
    },
    size: {
      type: String,
      default: 'md',
      validator: (value) => ['sm', 'md', 'lg', 'xl'].includes(value)
    },
    fullscreen: {
      type: Boolean,
      default: false
    },
    hideHeader: {
      type: Boolean,
      default: false
    },
    hideFooter: {
      type: Boolean,
      default: false
    },
    hideCloseButton: {
      type: Boolean,
      default: false
    },
    hideCancelButton: {
      type: Boolean,
      default: false
    },
    hideConfirmButton: {
      type: Boolean,
      default: false
    },
    cancelText: {
      type: String,
      default: 'Cancelar'
    },
    confirmText: {
      type: String,
      default: 'Confirmar'
    },
    confirmType: {
      type: String,
      default: 'primary',
      validator: (value) => ['primary', 'success', 'warning', 'danger'].includes(value)
    },
    confirmDisabled: {
      type: Boolean,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    },
    closeOnOverlay: {
      type: Boolean,
      default: true
    },
    noPadding: {
      type: Boolean,
      default: false
    }
  },
  emits: ['close', 'cancel', 'confirm'],
  setup(props, { emit }) {
    const sizeClass = computed(() => `modal-${props.size}`)
    
    const confirmButtonClass = computed(() => {
      const typeMap = {
        primary: 'btn-primary',
        success: 'btn-success',
        warning: 'btn-warning',
        danger: 'btn-danger'
      }
      return typeMap[props.confirmType] || 'btn-primary'
    })

    const handleClose = () => {
      emit('close')
    }

    const handleCancel = () => {
      emit('cancel')
      emit('close')
    }

    const handleConfirm = () => {
      emit('confirm')
    }

    const handleOverlayClick = () => {
      if (props.closeOnOverlay && !props.loading) {
        handleClose()
      }
    }

    return {
      sizeClass,
      confirmButtonClass,
      handleClose,
      handleCancel,
      handleConfirm,
      handleOverlayClick
    }
  }
}
</script>

<style lang="scss" scoped>
@import './BaseModal.scss';
</style>
