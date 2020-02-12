#!/bin/bash

cd ../

dotnet restore --no-cache


if  dotnet publish ./Neuralium/src/Neuralium.csproj --self-contained true -c Release /p:PublishTrimmed=true -o ./build -r linux-x64 ; then
dotnet clean ;
 echo "publish completed"
else
echo "publish failed"
fi



   
