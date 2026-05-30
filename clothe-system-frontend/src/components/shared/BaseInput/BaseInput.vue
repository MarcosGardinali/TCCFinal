<template>
  <div class="form-group" :class="{ 'full-width': fullWidth }">
    <label v-if="label" :for="inputId" class="form-label">
      {{ label }}
      <span v-if="required" class="required">*</span>
    </label>

    <div class="input-wrapper" :class="{ 'has-icon': hasIcon }">
      <!-- Input Text/Email/Password/Date -->
      <input
        v-if="['text', 'email', 'password', 'number', 'date'].includes(type)"
        :id="inputId"
        :type="type"
        :value="modelValue"
        @input="handleInput"
        @blur="handleBlur"
        :class="inputClasses"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :readonly="readonly"
        :min="min"
        :max="max"
        :step="step"
      />

      <!-- Input with Masks (CPF, RG, CEP, Phone, Currency) -->
      <div v-if="type === 'currency'" class="currency-input-wrapper">
        <input
          :id="inputId"
          type="text"
          :value="displayValue"
          @input="handleInput"
          @blur="handleBlur"
          :class="inputClasses"
          :placeholder="placeholder"
          :required="required"
          :disabled="disabled"
          :readonly="readonly"
        />
      </div>
      <input
        v-else-if="['cpf', 'rg', 'cep', 'tel'].includes(type)"
        :id="inputId"
        type="text"
        :value="modelValue"
        @input="handleInput"
        @blur="handleBlur"
        :class="inputClasses"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :readonly="readonly"
      />

      <!-- Textarea -->
      <textarea
        v-else-if="type === 'textarea'"
        :id="inputId"
        :value="modelValue"
        @input="handleInput"
        @blur="handleBlur"
        :class="inputClasses"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        :readonly="readonly"
        :rows="rows"
      ></textarea>

      <!-- Select -->
      <select
        v-else-if="type === 'select'"
        :id="inputId"
        :value="modelValue"
        @change="handleInput"
        @blur="handleBlur"
        :class="inputClasses"
        :required="required"
        :disabled="disabled"
      >
        <option value="">{{ placeholder || 'Selecione uma opção' }}</option>
        <option
          v-for="option in options"
          :key="option.value"
          :value="option.value"
        >
          {{ option.label }}
        </option>
      </select>

      <!-- Checkbox -->
      <label v-else-if="type === 'checkbox'" class="checkbox-label">
        <input
          :id="inputId"
          type="checkbox"
          :checked="modelValue"
          @change="handleInput"
          class="checkbox-input"
          :disabled="disabled"
        />
        <span class="checkbox-text">{{ checkboxText || label }}</span>
      </label>

      <div class="icon-slot">
        <slot name="icon"></slot>
      </div>
    </div>

    <!-- Error Message -->
    <span v-if="error" class="error-message">{{ error }}</span>

    <!-- Help Text -->
    <span v-if="helpText && !error" class="help-text">{{ helpText }}</span>
  </div>
</template>

<script>
import { computed, ref } from 'vue';
import { useMask } from '@/composables/useMask';

export default {
  name: 'BaseInput',
  props: {
    modelValue: {
      type: [String, Number, Boolean],
      default: '',
    },
    type: {
      type: String,
      default: 'text',
      validator: (value) =>
        [
          'text',
          'email',
          'password',
          'number',
          'date',
          'textarea',
          'select',
          'checkbox',
          'tel',
          'cpf',
          'rg',
          'cep',
          'currency',
        ].includes(value),
    },
    label: {
      type: String,
      default: '',
    },
    placeholder: {
      type: String,
      default: '',
    },
    required: {
      type: Boolean,
      default: false,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    readonly: {
      type: Boolean,
      default: false,
    },
    error: {
      type: String,
      default: '',
    },
    helpText: {
      type: String,
      default: '',
    },
    fullWidth: {
      type: Boolean,
      default: false,
    },
    hasIcon: {
      type: Boolean,
      default: false,
    },
    // Props específicas para diferentes tipos
    options: {
      type: Array,
      default: () => [],
    },
    checkboxText: {
      type: String,
      default: '',
    },
    rows: {
      type: Number,
      default: 3,
    },
    min: {
      type: [String, Number],
      default: undefined,
    },
    max: {
      type: [String, Number],
      default: undefined,
    },
    step: {
      type: [String, Number],
      default: undefined,
    },
  },
  emits: ['update:modelValue', 'blur'],
  setup(props, { emit }) {
    const {
      applyCPFMask,
      applyRGMask,
      applyCEPMask,
      applyPhoneMask,
      applyCurrencyMask,
      applyNumberMask,
    } = useMask();

    const inputId = ref(`input-${Math.random().toString(36).substr(2, 9)}`);

    const displayValue = computed(() => {
      if (props.type === 'currency' && props.modelValue) {
        return applyCurrencyMask(props.modelValue);
      }
      return props.modelValue;
    });

    const inputClasses = computed(() => {
      const baseClasses = ['form-input'];

      if (props.type === 'textarea') {
        baseClasses.push('form-textarea');
      }

      if (props.error) {
        baseClasses.push('error');
      }

      return baseClasses.join(' ');
    });

    const applyMask = (value) => {
      switch (props.type) {
        case 'cpf':
          return applyCPFMask(value);
        case 'rg':
          return applyRGMask(value);
        case 'cep':
          return applyCEPMask(value);
        case 'tel':
          return applyPhoneMask(value);
        case 'currency':
          return applyCurrencyMask(value);
        default:
          return value;
      }
    };

    const getRawValue = (value) => {
      switch (props.type) {
        case 'cpf':
        case 'rg':
        case 'cep':
          return value.replace(/\D/g, '');
        case 'tel':
          const cleaned = value.replace(/\D/g, '');
          if (cleaned.length === 11) {
            return '55' + cleaned;
          } else if (cleaned.length === 10) {
            return '55' + cleaned.substring(0, 2) + '9' + cleaned.substring(2);
          }
          return cleaned;
        case 'currency':
          return value.replace(/[^\d]/g, '');
        default:
          return value;
      }
    };

    const handleInput = (event) => {
      let value = event.target.value;

      if (props.type === 'checkbox') {
        value = event.target.checked;
      } else if (props.type === 'currency') {
        const maskedValue = applyMask(value);
        event.target.value = maskedValue;
        value = getRawValue(value);
      } else if (['cpf', 'rg', 'cep', 'tel'].includes(props.type)) {
        const maskedValue = applyMask(value);
        event.target.value = maskedValue;
        value = getRawValue(value);
      }

      emit('update:modelValue', value);
    };

    const handleBlur = (event) => {
      emit('blur', event);
    };

    return {
      inputId,
      inputClasses,
      displayValue,
      handleInput,
      handleBlur,
    };
  },
};
</script>

<style lang="scss" scoped>
@import './BaseInput.scss';
</style>
