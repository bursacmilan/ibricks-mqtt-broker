#!/bin/sh

apt-get update
apt-get install jq -y

ip=$(jq -r '.ip' /data/options.json)
mac=$(jq -r '.mac' /data/options.json)

echo "test ${ip}"

export Logging__LogLevel__ibricks_mqtt_broker="$ip"
dotnet /app/ibricks-mqtt-broker-webapp.dll