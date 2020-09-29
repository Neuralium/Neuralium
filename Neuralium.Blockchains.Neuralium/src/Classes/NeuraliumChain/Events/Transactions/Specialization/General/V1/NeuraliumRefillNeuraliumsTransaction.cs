using System.Collections.Immutable;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumRefillNeuraliumsTransaction : INeuraliumTransaction {
	}

#if TESTNET || DEVNET
	public class NeuraliumRefillNeuraliumsTransaction : Transaction, INeuraliumRefillNeuraliumsTransaction {

		public override Enums.TransactionTargetTypes TargetType => Enums.TransactionTargetTypes.Range;
		public override AccountId[] ImpactedAccounts =>this.TargetAccounts;
		public override AccountId[] TargetAccounts => this.GetSenderList();

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TOKEN_REFILL_NEURLIUMS: NeuraliumTransactionTypes.Instance.NEURALIUM_REFILL_NEURLIUMS, 1, 0);
		}
	}
#endif
}