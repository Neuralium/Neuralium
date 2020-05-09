using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumSetAccountCorrelationTransaction : ISetAccountCorrelationTransaction, INeuraliumTransaction {
	}

	public class NeuraliumSetAccountCorrelationTransaction : SetAccountCorrelationTransaction, INeuraliumSetAccountCorrelationTransaction {
	}
}