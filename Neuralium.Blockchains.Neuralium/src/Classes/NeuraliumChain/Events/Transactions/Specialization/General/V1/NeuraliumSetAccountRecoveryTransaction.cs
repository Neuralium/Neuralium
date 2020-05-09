using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {
	public interface INeuraliumSetAccountRecoveryTransaction : ISetAccountRecoveryTransaction, INeuraliumTransaction {
	}

	public class NeuraliumSetAccountRecoveryTransaction : SetAccountRecoveryTransaction, INeuraliumSetAccountRecoveryTransaction {
	}
}