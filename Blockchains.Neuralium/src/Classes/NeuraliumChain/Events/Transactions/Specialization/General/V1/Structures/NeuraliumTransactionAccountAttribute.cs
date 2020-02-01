using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1.Structures;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Structures {

	public interface INeuraliumTransactionAccountAttribute : ITransactionAccountAttribute {
	}

	public class NeuraliumTransactionAccountAttribute : TransactionAccountAttribute, INeuraliumTransactionAccountAttribute {
	}
}