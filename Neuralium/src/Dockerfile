FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.1-alpine as runtime
WORKDIR /app

COPY /bin/publish/ ./

ENTRYPOINT ["./Neuralium", "--runtime-mode=DOCKER"]
