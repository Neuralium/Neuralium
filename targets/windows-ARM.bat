
cd ../

dotnet restore


dotnet publish Neuralium/src/Neuralium.csproj --self-contained -c Release -o ../../build -r win8-arm
dotnet clean
echo "publish completed"




   
