using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumRefillNeuraliumsTransaction : INeuraliumTransaction {
	}

#if TESTNET || DEVNET
	public class NeuraliumRefillNeuraliumsTransaction : Transaction, INeuraliumRefillNeuraliumsTransaction {

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TOKEN_REFILL_NEURLIUMS: NeuraliumTransactionTypes.Instance.NEURALIUM_REFILL_NEURLIUMS, 1, 0);
		}
		
		public override ImmutableList<AccountId> TargetAccounts => new [] {this.TransactionId.Account}.ToImmutableList();

	}
#endif
}