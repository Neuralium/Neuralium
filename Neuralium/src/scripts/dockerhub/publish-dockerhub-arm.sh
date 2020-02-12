#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src || exit

rm -r bin/publish

if  dotnet publish -c Release --self-contained true /p:PublishTrimmed=true -r linux-arm -o bin/publish ; then
     echo "publish completed"
     
    docker rm neuralium-arm
    docker rmi neuralium-arm

    docker build -t neuralium-arm .

    docker tag neuralium-arm neuralium/neuralium:arm-testnet-0.1.0
else
    echo "build failed"
fi
