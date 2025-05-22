# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime-env
WORKDIR /app

COPY --from=build-env /app/publish .

ENV DISCORD_TOKEN=""

ENTRYPOINT [ "dotnet", "mika-bot-revamped.dll" ]