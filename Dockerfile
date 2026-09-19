FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Quipu.Bookstore.sln ./
COPY src/Quipu.Bookstore.Api/Quipu.Bookstore.Api.csproj src/Quipu.Bookstore.Api/
COPY src/Quipu.Bookstore.Application/Quipu.Bookstore.Application.csproj src/Quipu.Bookstore.Application/
COPY src/Quipu.Bookstore.Contracts/Quipu.Bookstore.Contracts.csproj src/Quipu.Bookstore.Contracts/
COPY src/Quipu.Bookstore.Domain/Quipu.Bookstore.Domain.csproj src/Quipu.Bookstore.Domain/
COPY src/Quipu.Bookstore.Storage/Quipu.Bookstore.Storage.csproj src/Quipu.Bookstore.Storage/

RUN dotnet restore src/Quipu.Bookstore.Api/Quipu.Bookstore.Api.csproj

COPY src/ src/

RUN dotnet publish src/Quipu.Bookstore.Api/Quipu.Bookstore.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Quipu.Bookstore.Api.dll"]
