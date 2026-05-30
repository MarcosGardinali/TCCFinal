import * as yup from 'yup'
import { isValidCPF, isValidEmail, isValidCEP } from './formatters'

// Esquemas de validação usando Yup

export const loginSchema = yup.object({
  email: yup
    .string()
    .required('E-mail é obrigatório')
    .test('valid-email', 'E-mail inválido', isValidEmail),
  password: yup
    .string()
    .required('Senha é obrigatória')
    .min(8, 'Senha deve ter pelo menos 8 caracteres')
})

export const registerSchema = yup.object({
  email: yup
    .string()
    .required('E-mail é obrigatório')
    .test('valid-email', 'E-mail inválido', isValidEmail),
  password: yup
    .string()
    .required('Senha é obrigatória')
    .min(8, 'Senha deve ter pelo menos 8 caracteres')
    .matches(
      /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
      'Senha deve conter pelo menos uma letra maiúscula, um número e um caractere especial'
    ),
  confirmPassword: yup
    .string()
    .required('Confirmação de senha é obrigatória')
    .oneOf([yup.ref('password')], 'Senhas não coincidem')
})

export const customerSchema = yup.object({
  firstName: yup
    .string()
    .required('Nome é obrigatório')
    .max(50, 'Nome deve ter no máximo 50 caracteres'),
  lastName: yup
    .string()
    .max(50, 'Sobrenome deve ter no máximo 50 caracteres'),
  birthDate: yup
    .date()
    .required('Data de nascimento é obrigatória')
    .max(new Date(), 'Data de nascimento não pode ser futura'),
  cpf: yup
    .string()
    .required('CPF é obrigatório')
    .test('valid-cpf', 'CPF inválido', isValidCPF),
  street: yup
    .string()
    .required('Endereço é obrigatório')
    .max(100, 'Endereço deve ter no máximo 100 caracteres'),
  complement: yup
    .string()
    .max(100, 'Complemento deve ter no máximo 100 caracteres'),
  neighborhood: yup
    .string()
    .required('Bairro é obrigatório')
    .max(50, 'Bairro deve ter no máximo 50 caracteres'),
  number: yup
    .string()
    .required('Número é obrigatório')
    .max(10, 'Número deve ter no máximo 10 caracteres'),
  cityName: yup
    .string()
    .required('Cidade é obrigatória')
    .max(50, 'Cidade deve ter no máximo 50 caracteres'),
  stateAbbreviation: yup
    .string()
    .required('Estado é obrigatório'),
  postalCode: yup
    .string()
    .test('valid-cep', 'CEP inválido', isValidCEP),
  rg: yup
    .string()
    .required('RG é obrigatório')
    .matches(/^\d{9}$/, 'RG deve conter apenas 9 dígitos numéricos'),
  mobilePhoneNumber: yup
    .string()
    .required('Telefone é obrigatório')
    .matches(/^\d{13}$/, 'Telefone deve conter 13 dígitos (com código do país)'),
  email: yup
    .string()
    .test('valid-email', 'E-mail inválido', (value) => !value || isValidEmail(value))
})

export const productSchema = yup.object({
  code: yup
    .string()
    .required('Código é obrigatório')
    .max(20, 'Código deve ter no máximo 20 caracteres'),
  description: yup
    .string()
    .required('Descrição é obrigatória')
    .max(100, 'Descrição deve ter no máximo 100 caracteres'),
  price: yup
    .number()
    .required('Preço é obrigatório')
    .min(0.01, 'Preço deve ser maior que zero')
    .max(10000, 'Preço deve ser menor que R$ 10.000,00'),
  brand: yup
    .string()
    .max(50, 'Marca deve ter no máximo 50 caracteres'),
  category: yup
    .string()
    .max(100, 'Categoria deve ter no máximo 100 caracteres')
})

export const orderSchema = yup.object({
  customerId: yup
    .number()
    .required('Cliente é obrigatório'),
  observation: yup
    .string()
    .max(500, 'Observação deve ter no máximo 500 caracteres'),
  items: yup
    .array()
    .of(
      yup.object({
        productId: yup
          .number()
          .required('Produto é obrigatório'),
        variation: yup
          .string()
          .required('Variação é obrigatória')
          .max(50, 'Variação deve ter no máximo 50 caracteres')
      })
    )
    .min(1, 'Pelo menos um item é obrigatório')
})

export const orderItemSchema = yup.object({
  productId: yup
    .number()
    .required('Produto é obrigatório'),
  variation: yup
    .string()
    .required('Variação é obrigatória')
    .max(50, 'Variação deve ter no máximo 50 caracteres'),
  status: yup
    .number()
    .oneOf([0, 1, 2], 'Status inválido')
})
