# Neuralium

[![Build Status](http://jenkins.neuralium.com/buildStatus/icon?job=Neuralium.Node-Linux64&subject=Linux-x64)](http://jenkins.neuralium.com/job/Neuralium.Node-Linux64/)
[![Build Status](http://jenkins.neuralium.com/buildStatus/icon?job=Neuralium.Node-Linux-Arm64&subject=Linux-ARM64)](http://jenkins.neuralium.com/job/Neuralium.Node-ARM64/)
[![Build Status](http://jenkins.neuralium.com/buildStatus/icon?job=Neuralium.Node-Win64&subject=Windows-x64)](http://jenkins.neuralium.com/job/Neuralium.Node-Win64/)

##### Version:  MAINNET 1.0.0

The Neuralium crypto token console server node

### Neuralium.Blockchains.Neuralium
The Neuralium blockchain implementation
### Neuralium.Api.Common
API public Interfaces
### Neuralium.Core
The Neuralium server node components
### Neuralium
The actual command line interface for the Server node.

## Build Instructions

##### First, ensure dotnet core 5.0 SDK is installed

#### The first step is to ensure that the dependencies have been built and copied into the local-source folder.

##### the source code to the below dependencies can be found here: [Neuralia Technologies source code](https://github.com/Neuralia) 

 - Neuralia.Blockchains.Tools
 - Neuralia.BouncyCastle
 - Neuralia.Blockchains.Core
 - Neuralia.Blockchains.Common
 - Neuralia.Blockchains.Components
 - Neuralia.NClap
 - Neuralia.Open.Nat

Then, simply invoke the right build file for your needs
>cd targets
> ./linux.sh
this will produce the executable in the folder /build
