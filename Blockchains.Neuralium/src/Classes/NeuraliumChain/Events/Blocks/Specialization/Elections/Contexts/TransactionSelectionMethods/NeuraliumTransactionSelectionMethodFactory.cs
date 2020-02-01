using Blockchains.Neuralium.Classes.Configuration;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Configuration;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods {
	public class NeuraliumTransactionSelectionMethodFactory : TransactionSelectionMethodFactory {

		public override ITransactionSelectionMethod CreateTransactionSelectionMethod(TransactionSelectionMethodType type, long blockId, BlockElectionDistillate blockElectionDistillate, IChainStateProvider chainStateProvider, IWalletProvider walletProvider, BlockChainConfigurations blockChainConfigurations, BlockchainServiceSet serviceSet) {

			if(blockElectionDistillate.ElectionContext is INeuraliumElectionContext neuraliumElectionContext) {

				NodeShareType nodeShareType = blockChainConfigurations.BlockSavingMode;
				if(type == TransactionSelectionMethodTypes.Instance.Automatic) {
					// ok, this one is meant to be automatic. we wlil try to find the best method
					//TODO: make this more elaborate. Try to response to the various cues we can use like the declared bounty allocator
					if(neuraliumElectionContext.BountyAllocationMethod.Version.Type.Value == BountyAllocationMethodTypes.Instance.EqualSplit) {
						type = NeuraliumTransactionSelectionMethodTypes.Instance.HighestTips;
					}

					// the default automatic best choice
					type = NeuraliumTransactionSelectionMethodTypes.Instance.HighestTips;
				}

				if(type == NeuraliumTransactionSelectionMethodTypes.Instance.HighestTips) {

					// ok, nothing special here, lets just maximize profits by choosing the highest paying transactions
					return new NeuraliumHighestTipTransactionSelectionMethod(blockId, neuraliumElectionContext.TransactionTipsAllocationMethod, chainStateProvider, (INeuraliumWalletProviderProxy) walletProvider, blockElectionDistillate.ElectionContext, ((NeuraliumBlockChainConfigurations) blockChainConfigurations).HighestTipTransactionSelectionStrategySettings, serviceSet.BlockchainTimeService, nodeShareType);

				}
			}

			return base.CreateTransactionSelectionMethod(type, blockId, blockElectionDistillate, chainStateProvider, walletProvider, blockChainConfigurations, serviceSet);
		}
	}
}