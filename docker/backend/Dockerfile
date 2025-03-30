FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ./LeisureConnect.API/ ./LeisureConnect.API/
COPY ./LeisureConnect.Core/ ./LeisureConnect.Core/
COPY ./LeisureConnect.Infrastructure/ ./LeisureConnect.Infrastructure/

RUN dotnet restore LeisureConnect.API/LeisureConnect.API.csproj
RUN dotnet publish LeisureConnect.API/LeisureConnect.API.csproj -c Debug -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "LeisureConnect.API.dll" ]