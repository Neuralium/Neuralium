
cd ../

dotnet restore --no-cache


dotnet publish Neuralium/src/Neuralium.csproj -c Release -p:PublishTrimmed=true -p:PublishSingleFile=true -o ./build -r win-x64
dotnet clean
echo "publish completed"




   
