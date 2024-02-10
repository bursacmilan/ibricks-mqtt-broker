#!/bin/sh

ip=$(jq -r '.ip' /data/options.json)
mac=$(jq -r '.mac' /data/options.json)

echo "test ${ip}"

dotnet /app/ibricks-mqtt-webapp.dll