import { ref, watch } from 'vue'

export function useMask() {
  // Máscara para CPF
  const applyCPFMask = (value) => {
    if (!value) return ''
    const cleaned = value.replace(/\D/g, '')
    return cleaned.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
  }

  // Máscara para RG
  const applyRGMask = (value) => {
    if (!value) return ''
    const cleaned = value.replace(/\D/g, '')
    return cleaned.replace(/(\d{2})(\d{3})(\d{3})(\d{1})/, '$1.$2.$3-$4')
  }

  // Máscara para CEP
  const applyCEPMask = (value) => {
    if (!value) return ''
    const cleaned = value.replace(/\D/g, '')
    return cleaned.replace(/(\d{5})(\d{3})/, '$1-$2')
  }

  // Máscara para telefone
  const applyPhoneMask = (value) => {
    if (!value) return ''
    const cleaned = value.replace(/\D/g, '')
    
    if (cleaned.length <= 10) {
      return cleaned.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3')
    } else if (cleaned.length <= 11) {
      return cleaned.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
    } else {
      // 13 dígitos com código do país
      return cleaned.replace(/(\d{2})(\d{2})(\d{5})(\d{4})/, '+$1 ($2) $3-$4')
    }
  }

  // Máscara para moeda
  const applyCurrencyMask = (value) => {
    if (!value) return ''
    
    // Remove tudo que não é dígito
    let cleaned = value.replace(/\D/g, '')
    
    // Converte para centavos
    let amount = parseInt(cleaned) / 100
    
    // Formata apenas os números sem o símbolo R$
    return new Intl.NumberFormat('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  // Máscara para números apenas
  const applyNumberMask = (value) => {
    if (!value) return ''
    return value.replace(/\D/g, '')
  }

  // Composable para input com máscara
  const useMaskedInput = (initialValue = '', maskFunction) => {
    const maskedValue = ref(maskFunction(initialValue))
    const rawValue = ref(initialValue)

    const updateValue = (newValue) => {
      rawValue.value = newValue
      maskedValue.value = maskFunction(newValue)
    }

    const handleInput = (event) => {
      const inputValue = event.target.value
      const masked = maskFunction(inputValue)
      
      // Atualiza o valor do input
      event.target.value = masked
      maskedValue.value = masked
      
      // Extrai o valor limpo baseado no tipo de máscara
      if (maskFunction === applyCPFMask || maskFunction === applyRGMask || 
          maskFunction === applyCEPMask || maskFunction === applyNumberMask) {
        rawValue.value = inputValue.replace(/\D/g, '')
      } else if (maskFunction === applyPhoneMask) {
        const cleaned = inputValue.replace(/\D/g, '')
        // Adiciona código do país se necessário
        if (cleaned.length === 11) {
          rawValue.value = '55' + cleaned
        } else if (cleaned.length === 10) {
          rawValue.value = '55' + cleaned.substring(0, 2) + '9' + cleaned.substring(2)
        } else {
          rawValue.value = cleaned
        }
      } else if (maskFunction === applyCurrencyMask) {
        rawValue.value = inputValue.replace(/\D/g, '')
      } else {
        rawValue.value = inputValue
      }
    }

    return {
      maskedValue,
      rawValue,
      updateValue,
      handleInput
    }
  }

  return {
    applyCPFMask,
    applyRGMask,
    applyCEPMask,
    applyPhoneMask,
    applyCurrencyMask,
    applyNumberMask,
    useMaskedInput
  }
}
