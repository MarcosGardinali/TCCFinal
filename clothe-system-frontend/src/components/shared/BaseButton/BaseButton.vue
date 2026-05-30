<template>
  <component
    :is="tag"
    :type="tag === 'button' ? type : undefined"
    :to="tag === 'router-link' ? to : undefined"
    :href="tag === 'a' ? href : undefined"
    :disabled="disabled || loading"
    :class="buttonClasses"
    @click="handleClick"
  >
    <i v-if="loading" class="fas fa-spinner fa-spin"></i>
    <i v-else-if="icon" :class="iconClasses"></i>
    <span v-if="$slots.default" class="btn-text">
      <slot></slot>
    </span>
  </component>
</template>

<script>
export default {
  name: 'BaseButton',
  props: {
    // Tipo do botão
    variant: {
      type: String,
      default: 'primary',
      validator: (value) => [
        'primary', 'secondary', 'success', 'warning', 'danger', 'info', 'outline'
      ].includes(value)
    },
    // Tamanho do botão
    size: {
      type: String,
      default: 'md',
      validator: (value) => ['xs', 'sm', 'md', 'lg', 'xl'].includes(value)
    },
    // Tag HTML a ser renderizada
    tag: {
      type: String,
      default: 'button',
      validator: (value) => ['button', 'a', 'router-link'].includes(value)
    },
    // Tipo do button (quando tag é button)
    type: {
      type: String,
      default: 'button',
      validator: (value) => ['button', 'submit', 'reset'].includes(value)
    },
    // Props para links
    to: {
      type: [String, Object],
      default: undefined
    },
    href: {
      type: String,
      default: undefined
    },
    // Estados
    disabled: {
      type: Boolean,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    },
    // Ícone
    icon: {
      type: String,
      default: ''
    },
    iconPosition: {
      type: String,
      default: 'left',
      validator: (value) => ['left', 'right'].includes(value)
    },
    // Estilo
    block: {
      type: Boolean,
      default: false
    },
    rounded: {
      type: Boolean,
      default: false
    }
  },
  emits: ['click'],
  computed: {
    buttonClasses() {
      const classes = ['btn']

      // Variante
      classes.push(`btn-${this.variant}`)

      // Tamanho
      classes.push(`btn-${this.size}`)

      // Modificadores
      if (this.block) classes.push('btn-block')
      if (this.rounded) classes.push('btn-rounded')
      if (this.loading) classes.push('btn-loading')
      if (this.disabled) classes.push('btn-disabled')

      return classes
    },
    iconClasses() {
      const classes = []

      if (this.icon) {
        // Se o ícone não começar com 'fa', adicionar 'fas'
        if (!this.icon.startsWith('fa')) {
          classes.push('fas', `fa-${this.icon}`)
        } else {
          classes.push(this.icon)
        }
      }

      return classes
    }
  },
  methods: {
    handleClick(event) {
      if (!this.disabled && !this.loading) {
        this.$emit('click', event)
      }
    }
  }
}
</script>

<style lang="scss" scoped>
@import './BaseButton.scss';
</style>
