using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Elections.Processors.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Elections.Processors.V1 {

	/// <summary>
	///     The main class that will perform the mining processing
	/// </summary>
	public class NeuraliumElectionProcessor : ElectionProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumElectionProcessor {
		public NeuraliumElectionProcessor(INeuraliumCentralCoordinator centralCoordinator, IEventPoolProvider chainEventPoolProvider) : base(centralCoordinator, chainEventPoolProvider) {

		}

		protected override ElectedCandidateResultDistillate CreateElectedCandidateResult() {
			return new NeuraliumElectedCandidateResultDistillate();
		}

		protected override TransactionSelectionMethodType GetTransactionSelectionMethodType() {

			switch(this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration.TransactionSelectionStrategy) {
				case NeuraliumBlockChainConfigurations.NeuraliumTransactionSelectionStrategies.Tips:
					return NeuraliumTransactionSelectionMethodTypes.Instance.HighestTips;
			}

			this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration.TransactionSelectionStrategy = this.centralCoordinator.ChainComponentProvider.ChainConfigurationProvider.NeuraliumBlockChainConfiguration.TransactionSelectionStrategy;

			return base.GetTransactionSelectionMethodType();
		}
	}
}