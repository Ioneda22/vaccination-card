# Build context esperado: a pasta ./src (ver docker-compose.yml).
# Multi-stage: compila com o SDK e publica numa imagem ASP.NET enxuta.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os .csproj primeiro para aproveitar o cache de restore.
COPY VaccinationCard.Domain/*.csproj VaccinationCard.Domain/
COPY VaccinationCard.Application/*.csproj VaccinationCard.Application/
COPY VaccinationCard.Infrastructure/*.csproj VaccinationCard.Infrastructure/
COPY VaccinationCard.API/*.csproj VaccinationCard.API/
RUN dotnet restore VaccinationCard.API/VaccinationCard.API.csproj

# Copia o restante do código e publica.
COPY . .
RUN dotnet publish VaccinationCard.API/VaccinationCard.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VaccinationCard.API.dll"]
