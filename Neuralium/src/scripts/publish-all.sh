#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src/

BASEPATH=/home/jdb/builds/

#RI=win-x64
#RI=osx-x64

dotnet restore

RI=linux-x64

full_path="$BASEPATH/linux/node"


rm -r  $full_path
mkdir $full_path

echo "building linux..."
if dotnet build -c Release -r $RI --no-incremental ; then
echo "publishing linux..."
    if  dotnet publish -c Release  --self-contained true -r $RI -o $full_path ; then
    
        rm -r "$full_path/config"
        cp -R "$BASEPATH/config/" "$full_path/config/"
        
         echo "publish completed"
    else
        echo "linux publish failed"
        exit 1
    fi
else
    echo "linux build failed"
    exit 1
fi

RI=win-x64

full_path="$BASEPATH/windows/node"

rm -r  $full_path
mkdir $full_path

echo "building windows..."
if dotnet build -c Release -r $RI --no-incremental ; then
echo "publishing windows..."
    if  dotnet publish -c Release  --self-contained true -r $RI -o $full_path ; then
    
        rm -r "$full_path/config"
        cp -R "$BASEPATH/config/" "$full_path/config/"
        
         echo "publish completed"
    else
        echo "windows publish failed"
        exit 1
    fi
else
    echo "windows build failed"
    exit 1
fi


RI=osx-x64

full_path="$BASEPATH/macos/node"

rm -r  $full_path
mkdir $full_path

echo "building macos..."
if dotnet build -c Release -r $RI --no-incremental ; then
echo "publishing macos..."
    if  dotnet publish -c Release  --self-contained true -r $RI -o $full_path  ; then
    
         rm -r "$full_path/config"
        cp -R "$BASEPATH/config/" "$full_path/config/"
        
         echo "publish completed"
    else
        echo "macos publish failed"
        exit 1
    fi
else
    echo "macos build failed"
    exit 1
fi





