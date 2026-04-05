FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY karaoke-place.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish karaoke-place.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /src
COPY . .
COPY --from=build /app/publish /app

EXPOSE 8080
ENTRYPOINT ["dotnet", "/app/karaoke-place.dll"]
