FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solução e dependências dos projetos para restaurar antes de copiar o restante do código.
COPY OutfitTrack.sln .
COPY OutfitTrack.Api/OutfitTrack.Api.csproj OutfitTrack.Api/
COPY OutfitTrack.CrossCutting/OutfitTrack.CrossCutting.csproj OutfitTrack.CrossCutting/
COPY OutfitTrack.Application/OutfitTrack.Application.csproj OutfitTrack.Application/
COPY OutfitTrack.Domain/OutfitTrack.Domain.csproj OutfitTrack.Domain/
COPY OutfitTrack.Infrastructure/OutfitTrack.Infrastructure.csproj OutfitTrack.Infrastructure/

RUN dotnet restore "OutfitTrack.Api/OutfitTrack.Api.csproj"

COPY . .
WORKDIR /src/OutfitTrack.Api
RUN dotnet build "OutfitTrack.Api.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
COPY --from=build /src /src
COPY OutfitTrack.Api/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh
RUN dotnet tool install --global dotnet-ef --version 9.0.10
ENV PATH="${PATH}:/root/.dotnet/tools"

EXPOSE 3001 3002
ENV ASPNETCORE_URLS="http://+:3002;https://+:3001"
ENTRYPOINT ["/app/entrypoint.sh"]
