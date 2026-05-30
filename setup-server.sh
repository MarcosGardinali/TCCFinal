#!/bin/bash

# Script de Setup para Servidor Azure
# Execute este script no servidor Azure para preparar o ambiente

set -e

echo "=========================================="
echo "Setup do Servidor Azure para TCC"
echo "=========================================="

# Update sistema
echo "📦 Atualizando sistema..."
sudo apt-get update
sudo apt-get upgrade -y

# Instalar Docker usando o repositório oficial
echo "🐳 Instalando Docker..."
sudo apt-get remove -y docker docker-engine docker.io containerd runc || true
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg lsb-release
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo systemctl start docker
sudo systemctl enable docker

# Instalar Docker Compose
# Usamos o plugin oficial, compatível com o comando `docker compose`.
echo "📦 Instalando Docker Compose..."
sudo apt-get install -y docker-compose-plugin

# Adicionar usuário ao grupo docker (sem sudo)
echo "👤 Configurando permissões do Docker..."
sudo usermod -aG docker $USER
newgrp docker

# Criar diretório de deploy
echo "📁 Criando diretório de deploy..."
mkdir -p /home/marcosgardinali/tcc-deploy
cd /home/marcosgardinali/tcc-deploy

# Criar diretório .ssh se não existir
echo "🔐 Configurando SSH..."
mkdir -p ~/.ssh
chmod 700 ~/.ssh

echo ""
echo "=========================================="
echo "✅ Setup concluído!"
echo "=========================================="
echo ""
echo "Próximos passos:"
echo "1. Faça logout e login novamente (ou execute: newgrp docker)"
echo "2. Configure a chave SSH no GitHub Secrets"
echo "3. Faça um push para a branch 'main' para acionar o deploy"
echo ""
echo "Para testar a conexão SSH:"
echo "ssh -v git@github.com"
echo ""
