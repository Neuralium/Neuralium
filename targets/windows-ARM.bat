
cd ../

dotnet restore --no-cache


dotnet publish Neuralium/src/Neuralium.csproj --self-contained true -c Release -p:PublishTrimmed=true -p:PublishSingleFile=true -o ./build -r win10-arm64
dotnet clean
echo "publish completed"




   
