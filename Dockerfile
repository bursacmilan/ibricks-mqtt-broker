FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY . .
EXPOSE 5000

RUN adduser --disabled-password \
--home /app \
--gecos '' dotnetuser && chown -R dotnetuser /app

USER dotnetuser

ENTRYPOINT dotnet "./ibricks-mqtt-broker-webapp.dll" --urls "http://+:5000"