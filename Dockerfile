FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Aggiunta dei pacchetti NuGet richiesti
RUN dotnet add package Microsoft.EntityFrameworkCore --version 8.0.18 && \
    dotnet add package Microsoft.EntityFrameworkCore.Relational --version 8.0.18 && \
    dotnet add package MySql.EntityFrameworkCore --version 8.0.18 && \
    dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.3 && \
    dotnet add package PayPalCheckoutSdk --version 1.0.4 && \
    dotnet add package Minio

RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.dll"]

