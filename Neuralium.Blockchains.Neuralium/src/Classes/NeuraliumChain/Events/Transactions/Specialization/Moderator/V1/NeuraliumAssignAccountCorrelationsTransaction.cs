using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {

	public interface INeuraliumAssignAccountCorrelationsTransaction : IAssignAccountCorrelationsTransaction, INeuraliumModerationTransaction {
	}

	public class NeuraliumAssignAccountCorrelationsTransaction : AssignAccountCorrelationsTransaction, INeuraliumAssignAccountCorrelationsTransaction {
	}
}