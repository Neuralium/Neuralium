using System.Collections.Generic;
using System.Collections.Immutable;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {

	public interface IDestroyNeuraliumsTransaction : INeuraliumModerationTransaction {
	}

	public class DestroyNeuraliumsTransaction : ModerationTransaction, IDestroyNeuraliumsTransaction {

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_MODERATOR_DESTROY_TOKENS, 1, 0);
		}
		
		public override ImmutableList<AccountId> TargetAccounts => new List<AccountId>().ToImmutableList();

	}
}