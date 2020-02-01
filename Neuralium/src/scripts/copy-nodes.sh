#!/bin/bash



BASEPATH=/home/jdb/builds/
wallet_path=/home/jdb/work/Neuralia/projects/neuralium-wallet/packaging/neuralium




full_origin_path="$BASEPATH/linux/node/*"
full_wallet_path="$wallet_path/linux/"

rm -r  $full_wallet_path
mkdir $full_wallet_path
cp -R $full_origin_path $full_wallet_path


full_origin_path="$BASEPATH/windows/node/*"
full_wallet_path="$wallet_path/win32/"

rm -r  $full_wallet_path
mkdir $full_wallet_path
cp -R $full_origin_path $full_wallet_path

full_origin_path="$BASEPATH/macos/node/*"
full_wallet_path="$wallet_path/mac/"

rm -r  $full_wallet_path
mkdir $full_wallet_path
cp -R $full_origin_path $full_wallet_path