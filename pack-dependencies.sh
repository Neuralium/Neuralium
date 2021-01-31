#!/bin/bash

dotnet nuget locals all --clear

mkdir deps
mkdir local-source
cd deps

rm Neuralia.NClap
rm Neuralia.Open.Nat
rm Neuralia.Blockchains.Tools
rm Neuralia.BouncyCastle
rm Neuralia.Blockchain

git clone -b MAINNET https://github.com/Neuralia/Neuralia.NClap
git clone -b MAINNET https://github.com/Neuralia/Neuralia.Open.Nat
git clone -b MAINNET https://github.com/Neuralia/Neuralia.Blockchains.Tools
git clone -b MAINNET https://github.com/Neuralia/Neuralia.BouncyCastle
git clone -b MAINNET https://github.com/Neuralia/Neuralia.Blockchain

cd Neuralia.NClap

rm Neuralia.NClap.*.nupkg
./pack.sh

cp Neuralia.NClap.*.nupkg ../Neuralia.Blockchains.Tools/local-source/
cp Neuralia.NClap.*.nupkg ../../local-source/
cp Neuralia.NClap.*.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.NClap.*.nupkg ../Neuralia.BouncyCastle/local-source/

cd ../Neuralia.Open.Nat

rm Neuralia.Open.Nat.*.nupkg
./pack.sh

cp Neuralia.Open.Nat.*.nupkg ../Neuralia.Blockchains.Tools/local-source/
cp Neuralia.Open.Nat.*.nupkg ../../local-source/
cp Neuralia.Open.Nat.*.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.Open.Nat.*.nupkg ../Neuralia.BouncyCastle/local-source/


cd ../Neuralia.Blockchains.Tools

rm Neuralia.Blockchains.Tools.*.nupkg
./pack.sh

cp Neuralia.Blockchains.Tools.*.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.Blockchains.Tools.*.nupkg ../Neuralia.BouncyCastle/local-source/
cp Neuralia.Blockchains.Tools.*.nupkg ../../local-source/


cd ../Neuralia.BouncyCastle

rm Neuralia.BouncyCastle.*.nupkg
./pack.sh

cp Neuralia.BouncyCastle.*.nupkg ../Neuralia.Blockchain/local-source/
cp Neuralia.BouncyCastle.*.nupkg ../../local-source/



cd ../Neuralia.Blockchain

rm Neuralia.Blockchain.*.nupkg
./pack.sh


cp Neuralia.Blockchains.Common.*.nupkg ../../local-source/
cp Neuralia.Blockchains.Core.*.nupkg ../../local-source/
cp Neuralia.Blockchains.Components.*.nupkg ../../local-source/


