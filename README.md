# Neuralium

##### Version:  Trial run IV

The Neuralium crypto token console server node

### Blockchains.Neuralium
The Neuralium blockchain implementation
### Neuralium.Api.Common
API public Interfaces
### Neuralium.Shell
The Neuralium server node components
### Neuralium
The actual command line interface for the Server node.

## Build Instructions

##### First, ensure dotnet core 3.0 SDK is installed

#### The first step is to ensure that the dependencies have been built and copied into the local-source folder.

##### the source code to the below dependencies can be found here: [Neuralia Technologies source code](https://github.com/Neuralia)
_NOTE: The dependencies must be build in the order listed_

 - Neuralia.Data.HashFunction.xxHash
 - Neuralia.STUN
 - Neuralia.Blockchains.Tools
 - Neuralia.BouncyCastle
 - Neuralia.Blockchains.Core
 - Neuralia.Blockchains.Common

Then, simply invoke the right build file for your needs
>cd targets
> ./linux.sh
this will produce the executable in the folder /build
