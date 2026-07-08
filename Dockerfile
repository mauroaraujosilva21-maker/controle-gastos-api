FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia os arquivos do projeto e restaura as dependências
COPY ["ControleGastos.csproj", "./"]
RUN dotnet restore "ControleGastos.csproj"

# Copia o restante dos arquivos e compila o projeto
COPY . .
RUN dotnet publish "ControleGastos.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Configura o ambiente de execução (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Instala a biblioteca de segurança necessária para o driver do PostgreSQL
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends libgssapi-krb5-2 && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ControleGastos.dll"]