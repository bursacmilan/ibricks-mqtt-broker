FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY . .


EXPOSE 5000
ENTRYPOINT ["./run.sh"]
