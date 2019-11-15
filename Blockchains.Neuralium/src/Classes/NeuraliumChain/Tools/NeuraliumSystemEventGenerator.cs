using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Tools {
	public class NeuraliumSystemEventGenerator : SystemEventGenerator {

		public static SystemEventGenerator NeuraliumBountyAllocated(long blockId, decimal bounty, decimal TransactionTip, AccountId delegateAccountId) {
			NeuraliumSystemEventGenerator generator = new NeuraliumSystemEventGenerator();

			generator.EventType = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningBountyAllocated;
			generator.Parameters = new object[] {blockId, bounty, TransactionTip, delegateAccountId?.ToString()};

			return generator;
		}
		
		public static SystemEventGenerator NeuraliumMiningPrimeElected(long blockId, decimal bounty, decimal TransactionTip, AccountId delegateAccountId) {
			NeuraliumSystemEventGenerator generator = new NeuraliumSystemEventGenerator();

			generator.EventType = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.NeuraliumMiningPrimeElected;
			generator.Parameters = new object[] {blockId, bounty, TransactionTip,  delegateAccountId?.ToString()};

			return generator;
		}

		public static SystemEventGenerator NeuraliumAccountTotalUpdated(long accountSequenceId, Enums.AccountTypes accountType, TotalAPI total) {
			NeuraliumSystemEventGenerator generator = new NeuraliumSystemEventGenerator();

			generator.EventType = NeuraliumBlockchainSystemEventTypes.NeuraliumInstance.AccountTotalUpdated;
			generator.Parameters = new object[] {new {AccountId = new AccountId(accountSequenceId, accountType).ToString(), Total = total}};

			return generator;
		}
	}
}