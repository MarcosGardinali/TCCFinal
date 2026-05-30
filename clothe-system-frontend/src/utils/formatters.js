// Formatadores de dados

export const formatCurrency = (value) => {
  if (!value && value !== 0) return 'R$ 0,00'
  
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}

export const formatDate = (date) => {
  if (!date) return ''
  
  const dateObj = new Date(date)
  return new Intl.DateTimeFormat('pt-BR').format(dateObj)
}

export const formatDateTime = (date) => {
  if (!date) return ''
  
  const dateObj = new Date(date)
  return new Intl.DateTimeFormat('pt-BR', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  }).format(dateObj)
}

export const formatCPF = (cpf) => {
  if (!cpf) return ''
  
  const cleanCPF = cpf.replace(/\D/g, '')
  return cleanCPF.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
}

export const formatRG = (rg) => {
  if (!rg) return ''
  
  const cleanRG = rg.replace(/\D/g, '')
  return cleanRG.replace(/(\d{2})(\d{3})(\d{3})(\d{1})/, '$1.$2.$3-$4')
}

export const formatPhone = (phone) => {
  if (!phone) return ''

  const cleanPhone = phone.replace(/\D/g, '')

  if (cleanPhone.length === 13) {
    // Formato: +55 (11) 99999-9999
    return cleanPhone.replace(/(\d{2})(\d{2})(\d{5})(\d{4})/, '+$1 ($2) $3-$4')
  } else if (cleanPhone.length === 11) {
    // Formato: (11) 99999-9999
    return cleanPhone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
  } else if (cleanPhone.length === 10) {
    // Formato: (11) 9999-9999
    return cleanPhone.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3')
  }

  return phone
}

export const formatCEP = (cep) => {
  if (!cep) return ''
  
  const cleanCEP = cep.replace(/\D/g, '')
  return cleanCEP.replace(/(\d{5})(\d{3})/, '$1-$2')
}

export const formatOrderNumber = (number) => {
  if (!number) return ''
  
  return `#${String(number).padStart(6, '0')}`
}

// Funções para limpar formatação
export const cleanCPF = (cpf) => {
  return cpf ? cpf.replace(/\D/g, '') : ''
}

export const cleanRG = (rg) => {
  return rg ? rg.replace(/\D/g, '') : ''
}

export const cleanPhone = (phone) => {
  if (!phone) return ''

  const cleaned = phone.replace(/\D/g, '')

  // Se tem 11 dígitos, adiciona o código do país (55)
  if (cleaned.length === 11) {
    return '55' + cleaned
  }

  // Se tem 10 dígitos, adiciona 55 + 9 no celular
  if (cleaned.length === 10) {
    return '55' + cleaned.substring(0, 2) + '9' + cleaned.substring(2)
  }

  return cleaned
}

export const cleanCEP = (cep) => {
  return cep ? cep.replace(/\D/g, '') : ''
}

// Validadores
export const isValidCPF = (cpf) => {
  const cleanedCPF = cleanCPF(cpf)
  
  if (cleanedCPF.length !== 11) return false
  
  // Verifica se todos os dígitos são iguais
  if (/^(\d)\1{10}$/.test(cleanedCPF)) return false
  
  // Validação do primeiro dígito verificador
  let sum = 0
  for (let i = 0; i < 9; i++) {
    sum += parseInt(cleanedCPF.charAt(i)) * (10 - i)
  }
  let remainder = 11 - (sum % 11)
  if (remainder === 10 || remainder === 11) remainder = 0
  if (remainder !== parseInt(cleanedCPF.charAt(9))) return false
  
  // Validação do segundo dígito verificador
  sum = 0
  for (let i = 0; i < 10; i++) {
    sum += parseInt(cleanedCPF.charAt(i)) * (11 - i)
  }
  remainder = 11 - (sum % 11)
  if (remainder === 10 || remainder === 11) remainder = 0
  if (remainder !== parseInt(cleanedCPF.charAt(10))) return false
  
  return true
}

export const isValidEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export const isValidCEP = (cep) => {
  const cleanedCEP = cleanCEP(cep)
  return cleanedCEP.length === 8
}
