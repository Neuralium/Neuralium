using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {

	public interface INeuraliumAccountResetWarningTransaction : IAccountResetWarningTransaction, INeuraliumModerationTransaction {
	}

	public class NeuraliumAccountResetWarningTransaction : AccountResetWarningTransaction, INeuraliumAccountResetWarningTransaction {
	}
}