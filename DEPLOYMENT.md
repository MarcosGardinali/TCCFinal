# Guia de Deployment com GitHub Actions

Este projeto está configurado para fazer deploy automático na Azure sempre que você fazer um push na branch `main`.

## Pré-requisitos

1. **Chave SSH privada** gerada e adicionada ao servidor Azure
2. **Secrets configurados** no repositório GitHub
3. **Docker e Docker Compose** instalados no servidor Azure

## Configuração Necessária

### 1. Gerar Chave SSH (se ainda não tiver)

No seu computador:
```bash
ssh-keygen -t rsa -b 4096 -f ~/.ssh/id_rsa_azure
```

### 2. Adicionar chave pública ao servidor Azure

```bash
# No servidor Azure
mkdir -p ~/.ssh
echo "$(cat ~/.ssh/id_rsa_azure.pub)" >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
chmod 700 ~/.ssh
```

### 3. Configurar Secrets no GitHub

1. Vá para: `Settings` → `Secrets and variables` → `Actions`
2. Clique em `New repository secret`
3. Adicione o seguinte secret:

**Nome:** `SSH_PRIVATE_KEY`  
**Valor:** Cole o conteúdo da chave privada (`~/.ssh/id_rsa_azure`)

```bash
# Para copiar a chave:
cat ~/.ssh/id_rsa_azure
```

### 4. Verificar Configurações do Workflow

O arquivo `.github/workflows/deploy.yml` já está configurado com:

- **Deploy Path:** `/home/marcosgardinali/tcc-deploy`
- **SSH User:** `marcosgardinali`
- **Server IP:** `172.210.234.104`

Se precisar alterar estes valores, edite o arquivo `.github/workflows/deploy.yml`.

## Como Funciona

1. **Trigger:** Quando você faz `git push` na branch `main`, o workflow é acionado automaticamente
2. **Checkout:** Código é baixado do repositório
3. **SSH Setup:** Chave SSH é configurada
4. **Deploy:** Arquivos são sincronizados via `rsync` (excluindo pastas de build e dependências)
5. **Build:** Docker containers são reconstruídos com `docker compose up --build`
6. **Verificação:** Status dos containers é verificado

## Monitorando o Deploy

1. Vá para `Actions` no seu repositório GitHub
2. Veja o status do último workflow executado
3. Clique para ver os logs detalhados

## Exclusões Automáticas

O workflow exclui automaticamente durante o deploy:
- `.git`, `.github`
- `node_modules`, `dist`
- `bin`, `obj` (C#)
- `.vs`, `.vscode`
- `*.env` (arquivos de ambiente locais)
- Logs do npm/yarn

## Estrutura no Servidor Azure

```
/home/marcosgardinali/tcc-deploy/
├── clothe-system-frontend/
├── outfit-track/
├── docker-compose.yml
└── .github/
```

## Acessar o Servidor Manualmente

```bash
ssh marcosgardinali@172.210.234.104
cd /home/marcosgardinali/tcc-deploy
docker compose ps
docker compose logs backend
```

## Troubleshooting

### ❌ "Permission denied (publickey)"

- Verifique se a chave pública foi adicionada ao `~/.ssh/authorized_keys` no servidor
- Teste a conexão: `ssh marcosgardinali@172.210.234.104`

### ❌ "Docker compose command not found"

- Instale Docker e Docker Compose no servidor:
  ```bash
  sudo apt-get update
  sudo apt-get install -y docker.io docker-compose
  ```

### ❌ "Port already in use"

- Verifique os containers rodando: `docker ps`
- Libere portas conflitantes: `docker compose down`

## Endpoints após Deploy

- **Frontend:** `http://172.210.234.104:5173`
- **Backend:** `http://172.210.234.104:3002`
- **phpMyAdmin:** `http://172.210.234.104:8080`
- **MySQL:** `172.210.234.104:3307`

## Segurança

⚠️ **Importante:**
- Nunca compartilhe a chave SSH privada
- A senha do MySQL (`123456`) é apenas para desenvolvimento
- Para produção, use variáveis de ambiente e senhas fortes
- Considere usar um `.env` seguro ou secrets na Azure
