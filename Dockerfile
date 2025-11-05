FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrate
WORKDIR /src
COPY . .
RUN dotnet tool install --global dotnet-ef
RUN dotnet ef database update --connection "Server=mysql;Port=3306;Database=ecommerce;Uid=user;Pwd=pass;"

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.dll"]
