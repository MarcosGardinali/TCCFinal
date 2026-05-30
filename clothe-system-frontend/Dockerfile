# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Copiar package.json e package-lock.json
COPY package*.json ./

# Instalar dependências
RUN npm install

# Copiar código-fonte
COPY . .

# Build da aplicação Vue
RUN npm run build

# Stage 2: Runtime
FROM node:20-alpine
WORKDIR /app

# Instalar serve para servir a aplicação
RUN npm install -g serve

# Copiar arquivos built do stage anterior
COPY --from=build /app/dist ./dist

# Copiar package.json para dependências de produção
COPY package.json .
RUN npm install --production

EXPOSE 5173

# Comando para iniciar a aplicação
CMD ["serve", "-s", "dist", "-l", "5173"]
