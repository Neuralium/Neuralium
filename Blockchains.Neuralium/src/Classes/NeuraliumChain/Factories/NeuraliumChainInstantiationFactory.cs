using System;
using System.IO.Abstractions;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Serialization;
using Blockchains.Neuralium.Classes.NeuraliumChain.Managers;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Factories;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages;
using Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Services;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Factories {
	public interface INeuraliumChainInstantiationFactory : IChainInstantiationFactory {
	}

	/// <summary>
	///     this base class is used to simplify the partial class wrappers. this way we dont have to carry around all those
	///     generics
	/// </summary>
	public class NeuraliumChainInstantiationFactory : NeuraliumChainInstantiationFactoryGenerix<NeuraliumChainInstantiationFactory> {

		public TimeSpan? TaskCheckSpan { get; set; }

		public override INeuraliumBlockChainInterface CreateNewChain(IServiceProvider serviceProvider, ChainRuntimeConfiguration chainRuntimeConfiguration = null, IFileSystem fileSystem = null) {

			chainRuntimeConfiguration ??= new ChainRuntimeConfiguration();
			fileSystem ??= new FileSystem();

			DIService.Instance.AddServiceProvider(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, serviceProvider);
			BlockchainServiceSet serviceSet = new BlockchainServiceSet(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium);

			INeuraliumCentralCoordinator centralCoordinator = this.CreateCentralCoordinator(serviceSet, chainRuntimeConfiguration, fileSystem);

			NeuraliumBlockChainInterface chainInterface = new NeuraliumBlockChainInterface(centralCoordinator, this.TaskCheckSpan);

			centralCoordinator.InitializeContents(this.CreateChainComponents(centralCoordinator, chainInterface));

			return chainInterface;
		}

		protected override INeuraliumCentralCoordinator CreateCentralCoordinator(BlockchainServiceSet serviceSet, ChainRuntimeConfiguration chainRuntimeConfiguration, IFileSystem fileSystem) {
			return new NeuraliumCentralCoordinator(serviceSet, chainRuntimeConfiguration, fileSystem);
		}

		/// <summary>
		///     The main method where we will create all useful dependencies for the chain
		/// </summary>
		/// <param name="centralCoordinator"></param>
		/// <returns></returns>
		protected override INeuraliumChainComponentsInjection CreateChainComponents(INeuraliumCentralCoordinator centralCoordinator, INeuraliumBlockChainInterface chainInterface) {

			NeuraliumWalletProvider walletProvider = new NeuraliumWalletProvider(centralCoordinator);
			NeuraliumWalletProviderProxy walletProviderProxy = new NeuraliumWalletProviderProxy(centralCoordinator, walletProvider);

			NeuraliumChainDalCreationFactory chainDalCreationFactory = new NeuraliumChainDalCreationFactory();
			NeuraliumChainTypeCreationFactory chainTypeCreationFactory = new NeuraliumChainTypeCreationFactory(centralCoordinator);
			NeuraliumClientWorkflowFactory clientWorkflowFactory = new NeuraliumClientWorkflowFactory(centralCoordinator);
			NeuraliumGossipWorkflowFactory gossipWorkflowFactory = new NeuraliumGossipWorkflowFactory(centralCoordinator);
			NeuraliumServerWorkflowFactory serverWorkflowFactory = new NeuraliumServerWorkflowFactory(centralCoordinator);
			NeuraliumMainChainMessageFactory messageFactory = new NeuraliumMainChainMessageFactory(centralCoordinator.BlockchainServiceSet);
			NeuraliumWorkflowTaskFactory taskFactory = new NeuraliumWorkflowTaskFactory(centralCoordinator);
			NeuraliumBlockchainEventsRehydrationFactory blockchainEventsRehydrationFactory = new NeuraliumBlockchainEventsRehydrationFactory(centralCoordinator);
			NeuraliumChainWorkflowFactory workflowFactory = new NeuraliumChainWorkflowFactory(centralCoordinator);

			INeuraliumChainFactoryProvider chainFactoryProvider = new NeuraliumChainFactoryProvider(chainDalCreationFactory, chainTypeCreationFactory, workflowFactory, messageFactory, clientWorkflowFactory, serverWorkflowFactory, gossipWorkflowFactory, taskFactory, blockchainEventsRehydrationFactory);

			INeuraliumChainStateProvider chainStateProvider = new NeuraliumChainStateProvider(centralCoordinator);
			INeuraliumAccountSnapshotsProvider accountSnapshotsProvider = new NeuraliumAccountSnapshotsProvider(centralCoordinator);
			INeuraliumChainConfigurationProvider chainConfigurationProvider = new NeuraliumChainConfigurationProvider();

			INeuraliumChainMiningProvider chainMiningProvider = new NeuraliumChainMiningProvider(centralCoordinator);

			INeuraliumChainDataLoadProvider chainDataLoadProvider = new NeuraliumChainDataWriteProvider(centralCoordinator);

			INeuraliumInterpretationProvider interpretationProvider = new NeuraliumInterpretationProvider(centralCoordinator);

			INeuraliumAccreditationCertificateProvider accreditationCertificateProvider = new NeuraliumAccreditationCertificateProvider(centralCoordinator);

			INeuraliumChainNetworkingProvider chainNetworkingProvider = new NeuraliumChainNetworkingProvider(DIService.Instance.GetService<IBlockchainNetworkingService>(), centralCoordinator);

			INeuraliumChainValidationProvider chainValidationProvider = new NeuraliumChainValidationProvider(centralCoordinator);
			
			INeuraliumAssemblyProvider assemblyProvider = new NeuraliumAssemblyProvider(centralCoordinator);

			// build the final component
			NeuraliumChainComponentsInjection componentsInjector = new NeuraliumChainComponentsInjection();

			componentsInjector.ChainComponentProvider = new NeuraliumChainComponentProvider(walletProviderProxy, assemblyProvider, chainFactoryProvider, chainStateProvider, chainConfigurationProvider, chainValidationProvider, chainMiningProvider, chainDataLoadProvider, accreditationCertificateProvider, accountSnapshotsProvider, chainNetworkingProvider, interpretationProvider);

			NeuraliumBlockchainManager transactionBlockchainManager = new NeuraliumBlockchainManager(centralCoordinator);
			NeuraliumGossipManager gossipManager = new NeuraliumGossipManager(centralCoordinator);

			componentsInjector.blockchainManager = transactionBlockchainManager;
			componentsInjector.gossipManager = gossipManager;

			componentsInjector.chainInterface = chainInterface;

			return componentsInjector;
		}
	}

	public abstract class NeuraliumChainInstantiationFactoryGenerix<CHAIN_CREATION_FACTORY_IMPLEMENTATION> : ChainInstantiationFactory<CHAIN_CREATION_FACTORY_IMPLEMENTATION, INeuraliumBlockChainInterface, INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, INeuraliumChainComponentsInjection>, INeuraliumChainInstantiationFactory
		where CHAIN_CREATION_FACTORY_IMPLEMENTATION : class, INeuraliumChainInstantiationFactory, new() {
	}
}