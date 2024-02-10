FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY . .
RUN chmod 777 run.sh

RUN apt-get update
RUN apt-get install jq -y

EXPOSE 5000
ENTRYPOINT ["run.sh"]
