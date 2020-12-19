#!/bin/bash

cd ../

dotnet restore  --no-cache


if  dotnet publish ./Neuralium/src/Neuralium.csproj -c Release /p:PublishTrimmed=true  /p:PublishSingleFile=true -o ./build -r linux-x64 ; then
	dotnet clean ;
 echo "publish completed"
else
echo "publish failed"
fi



   
