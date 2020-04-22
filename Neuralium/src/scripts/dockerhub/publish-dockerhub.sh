#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src || exit

rm -r bin/publish

if dotnet publish -c Release --self-contained true /p:PublishTrimmed=true -r alpine-x64 -o bin/publish; then
  echo "publish completed"

  docker rm neuralium
  docker rmi neuralium

  docker build -t neuralium .

  docker tag neuralium neuralium/neuralium:testnet-0.1.0
else
  echo "build failed"
fi

# docker push neuralium/neuralium:testnet-0.1.0
