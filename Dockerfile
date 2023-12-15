FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY . .
EXPOSE 5000

ENTRYPOINT dotnet "./ibricks-mqtt-broker-webapp.dll" --urls "http://+:5000"