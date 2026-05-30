#!/bin/sh
set -e

MAX_TRIES=10
COUNT=1

while [ "$COUNT" -le "$MAX_TRIES" ]; do
  echo "[entrypoint] Tentativa $COUNT de aplicar migrations..."
  if dotnet ef database update --project /src/OutfitTrack.Infrastructure/OutfitTrack.Infrastructure.csproj -s /src/OutfitTrack.Api/OutfitTrack.Api.csproj -c OutfitTrackContext; then
    echo "[entrypoint] Migrations aplicadas com sucesso."
    break
  fi
  if [ "$COUNT" -eq "$MAX_TRIES" ]; then
    echo "[entrypoint] Falha ao aplicar migrations após $MAX_TRIES tentativas."
    exit 1
  fi
  COUNT=$((COUNT + 1))
  echo "[entrypoint] Banco de dados não disponível ainda, aguardando 5 segundos..."
  sleep 5
  echo ""
done

exec dotnet /app/OutfitTrack.Api.dll
