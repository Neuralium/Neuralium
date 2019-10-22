#!/bin/bash

mkdir deps
mkdir local-source
cd deps
git clone https://github.com/Neuralia/Neuralia.Data.HashFunction.xxHash
git clone https://github.com/Neuralia/Neuralia.STUN
git clone https://github.com/Neuralia/Neuralia.Blockchains.Tools
git clone https://github.com/Neuralia/Neuralia.BouncyCastle
git clone https://github.com/Neuralia/Neuralia.Blockchain

cd Neuralia.Data.HashFunction.xxHash

if test -f "Neuralia.Data.HashFunction.xxHash.1.0.0.nupkg"; then
echo "file exists."
else
	dotnet restore
	if dotnet build -c Release --no-incremental ; then
		if  dotnet pack -c Release -o ../ ; then
		dotnet clean ;
			 echo "pack completed"
		else
			echo "build failed"
		fi
	else
		echo "build failed"
	fi
fi

cp Neuralia.Data.HashFunction.xxHash.1.0.0.nupkg ../Neuralia.Blockchains.Tools/local-source/
cp Neuralia.Data.HashFunction.xxHash.1.0.0.nupkg ../../local-source/
cp Neuralia.Data.HashFunction.xxHash.1.0.0.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.Data.HashFunction.xxHash.1.0.0.nupkg ../Neuralia.BouncyCastle/local-source/


cd ../Neuralia.Blockchains.Tools

if test -f "Neuralia.Blockchains.Tools.1.0.0.nupkg"; then
echo "file exists."
else
	dotnet restore
	if dotnet build -c Release --no-incremental ; then
		if  dotnet pack -c Release -o ../ ; then
		dotnet clean ;
			 echo "pack completed"
		else
			echo "build failed"
		fi
	else
		echo "build failed"
	fi
fi

cp Neuralia.Blockchains.Tools.1.0.0.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.Blockchains.Tools.1.0.0.nupkg ../Neuralia.BouncyCastle/local-source/
cp Neuralia.Blockchains.Tools.1.0.0.nupkg ../../local-source/

cd ../Neuralia.STUN

if test -f "Neuralia.STUN.1.0.0.nupkg"; then
echo "file exists."
else
	dotnet restore
	if dotnet build -c Release --no-incremental ; then
		if  dotnet pack -c Release -o ../ ; then
		dotnet clean ;
			 echo "pack completed"
		else
			echo "build failed"
		fi
	else
		echo "build failed"
	fi
fi


cp Neuralia.STUN.1.0.0.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.STUN.1.0.0.nupkg ../../local-source/


cd ../
if test -f "Neuralia.BouncyCastle.1.0.0.nupkg"; then
cd Neuralia.BouncyCastle
echo "file exists."
else
cd Neuralia.BouncyCastle
	dotnet restore
	if dotnet build Neuralia.BouncyCastle.sln -c Release --no-incremental ; then
		if  dotnet pack Neuralia.BouncyCastle.sln -c Release -o ../ ; then
		dotnet clean ;
			 echo "pack completed"
		else
			echo "build failed"
		fi
	else
		echo "build failed"
	fi
fi

cd ../
cp Neuralia.BouncyCastle.1.0.0.nupkg Neuralia.Blockchain/local-source/
cp Neuralia.BouncyCastle.1.0.0.nupkg ../local-source/

cd Neuralia.Blockchain

if test -f "Neuralia.Blockchains.Common.1.0.0.nupkg" & test -f "Neuralia.Blockchains.Core.1.0.0.nupkg"; then
echo "files exists."
else
	dotnet restore
	if dotnet build -c Release --no-incremental ; then
		if  dotnet pack -c Release -o ../../ ; then
		dotnet clean ;
			 echo "pack completed"
		else
			echo "build failed"
		fi
	else
		echo "build failed"
	fi
fi

cp Neuralia.Blockchains.Common.1.0.0.nupkg ../../local-source/
cp Neuralia.Blockchains.Core.1.0.0.nupkg ../../local-source/
