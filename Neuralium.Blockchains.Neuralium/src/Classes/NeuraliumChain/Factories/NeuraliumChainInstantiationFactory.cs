using System;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Serialization;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Managers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Factories;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Messages;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Workflows.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Common.Classes.Services;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories {
	public interface INeuraliumChainInstantiationFactory : IChainInstantiationFactory {
	}

	/// <summary>
	///     this base class is used to simplify the partial class wrappers. this way we dont have to carry around all those
	///     generics
	/// </summary>
	public class NeuraliumChainInstantiationFactory : NeuraliumChainInstantiationFactoryGenerix<NeuraliumChainInstantiationFactory> {

		public TimeSpan? TaskCheckSpan { get; set; }

		public override async Task<INeuraliumBlockChainInterface> CreateNewChain(IServiceProvider serviceProvider, LockContext lockContext, ChainRuntimeConfiguration chainRuntimeConfiguration = null, FileSystemWrapper fileSystem = null) {

			chainRuntimeConfiguration ??= new ChainRuntimeConfiguration();
			fileSystem ??= FileSystemWrapper.CreatePhysical();

			DIService.Instance.AddServiceProvider(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium, serviceProvider);
			BlockchainServiceSet serviceSet = new BlockchainServiceSet(NeuraliumBlockchainTypes.NeuraliumInstance.Neuralium);

			INeuraliumCentralCoordinator centralCoordinator = this.CreateCentralCoordinator(serviceSet, chainRuntimeConfiguration, fileSystem);

			NeuraliumBlockChainInterface chainInterface = new NeuraliumBlockChainInterface(centralCoordinator, this.TaskCheckSpan);

			await centralCoordinator.InitializeContents(this.CreateChainComponents(centralCoordinator, chainInterface), lockContext).ConfigureAwait(false);

			return chainInterface;
		}

		protected override INeuraliumCentralCoordinator CreateCentralCoordinator(BlockchainServiceSet serviceSet, ChainRuntimeConfiguration chainRuntimeConfiguration, FileSystemWrapper fileSystem) {
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

			INeuraliumBlockchainProvider blockchainProvider = new NeuraliumBlockchainProvider(centralCoordinator);

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

			INeuraliumAppointmentsProvider appointmentsProvider = new NeuraliumAppointmentsProvider(centralCoordinator);
			// build the final component
			NeuraliumChainComponentsInjection componentsInjector = new NeuraliumChainComponentsInjection();

			componentsInjector.ChainComponentProvider = new NeuraliumChainComponentProvider(walletProviderProxy, assemblyProvider, chainFactoryProvider, blockchainProvider, chainStateProvider, chainConfigurationProvider, chainValidationProvider, chainMiningProvider, chainDataLoadProvider, accreditationCertificateProvider, accountSnapshotsProvider, chainNetworkingProvider, interpretationProvider, appointmentsProvider);

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