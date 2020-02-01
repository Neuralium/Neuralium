#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src


dotnet restore

if dotnet build -c Release -r alpine-x64 --no-incremental ; then
    if  dotnet publish -c Release -r alpine-x64 -o bin/publish ; then
        echo "building docker image"

        docker rm neuralium.node
        docker rmi neuralium.node
    
        docker build -t neuralium.node .
    
    
        docker tag neuralium.node us.gcr.io/neuralium/neuralium.node
    else
        echo "publish failed"
    fi
else
    echo "build failed"
fi


#if msbuild *.csproj -t:publish -p:PublishWithAspNetCoreTargetManifest=false -p:RuntimeIdentifiers=alpine-x64 -p:PublishDir=bin/publish -p:Configuration=Release -p:DebugSymbols=false -p:DebugType=None ; then
#     echo "building docker image"
#     
#    docker rm neuralium.node
#    docker rmi neuralium.node
#
#    docker build -t neuralium.node .
#
#else
#    echo "build failed"
#fi
