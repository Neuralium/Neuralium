
cd ../

dotnet restore --no-cache


dotnet publish Neuralium/src/Neuralium.csproj --self-contained true -c Release /p:PublishTrimmed=true -o ./build -r win8-arm
dotnet clean
echo "publish completed"




   
