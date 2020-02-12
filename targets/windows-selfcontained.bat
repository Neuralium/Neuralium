
cd ../

dotnet restore --no-cache


dotnet publish Neuralium/src/Neuralium.csproj --self-contained true /p:PublishTrimmed=true -c Release -o ./build -r win-x64
dotnet clean
echo "publish completed"




   
