using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {
	public interface INeuraliumGenesisAccountPresentationTransaction : IGenesisAccountPresentationTransaction, INeuraliumModerationTransaction {
	}

	public class NeuraliumGenesisAccountPresentationTransaction : GenesisAccountPresentationTransaction, INeuraliumGenesisAccountPresentationTransaction {

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TransactionTypes.Instance.MODERATION_PRESENTATION, 1, 0);
		}
	}
}