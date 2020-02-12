#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src

#note: we need  <TargetLatestRuntimePatch>true</TargetLatestRuntimePatch> and <NoWarn>NU1605</NoWarn> in certain projects to fix a dotnet core issue with netstandard2.0.
#RI=linux-x64
RI=win-x64
#RI=osx-x64

#build first to clean resharper issue

if  dotnet publish -c Release  --self-contained true /p:PublishTrimmed=true -r $RI -o bin/publish ; then
     echo "publish completed"
else
    echo "build failed"
fi


#    
#if msbuild *.csproj -t:clean,build -restore -maxcpucount:3 -p:PublishWithAspNetCoreTargetManifest=false -p:RuntimeIdentifier=$RI -p:Configuration=Release -p:DebugSymbols=false -p:DebugType=None  ; then
#    if  msbuild *.csproj -t:publish -maxcpucount:3 -p:PublishWithAspNetCoreTargetManifest=false -p:RuntimeIdentifier=$RI -p:SelfContained=true -p:PublishDir=bin/publish -p:Configuration=Release -p:DebugSymbols=false -p:DebugType=None  ; then
#         echo "publish completed"
#    else
#        echo "build failed"
#    fi
#else
#    echo "build failed"
#fi

    





