
# Etapa 1: Compilacion
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copiar archivos de proyecto para aprovechar la cache
COPY src/services/TicketService/ServiceDesk.Tickets.Api/*.csproj src/services/TicketService/ServiceDesk.Tickets.Api/
COPY src/services/TicketService/ServiceDesk.Tickets.Application/*.csproj src/services/TicketService/ServiceDesk.Tickets.Application/
COPY src/services/TicketService/ServiceDesk.Tickets.Domain/*.csproj src/services/TicketService/ServiceDesk.Tickets.Domain/
COPY src/services/TicketService/ServiceDesk.Tickets.Infrastructure/*.csproj src/services/TicketService/ServiceDesk.Tickets.Infrastructure/

# Restaurar dependencias
RUN dotnet restore "src/services/TicketService/ServiceDesk.Tickets.Api/ServiceDesk.Tickets.Api.csproj"

# Copiar el codigo fuente
COPY . .

# Publicar la API
RUN dotnet publish "src/services/TicketService/ServiceDesk.Tickets.Api/ServiceDesk.Tickets.Api.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish

# Etapa 2: Ejecucion
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

# Ejecutar con usuario sin privilegios de root
USER app

ENTRYPOINT ["dotnet", "ServiceDesk.Tickets.Api.dll"]
