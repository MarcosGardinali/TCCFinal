module.exports = {
  env: {
    node: true,
  },
  extends: [
    'eslint:recommended',
    'plugin:vue/vue3-essential',
  ],
  parserOptions: {
    ecmaVersion: 'latest',
  },
  rules: {
    'vue/multi-word-component-names': 'off',
    'prettier/prettier': 'off',
    'no-unused-vars': 'warn',
    'no-console': 'off',
    'vue/no-unused-components': 'warn',
  },
}
