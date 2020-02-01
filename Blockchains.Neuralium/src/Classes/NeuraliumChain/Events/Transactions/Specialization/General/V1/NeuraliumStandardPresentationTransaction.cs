using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Structures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1.Structures;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumStandardPresentationTransaction : IStandardPresentationTransaction, INeuraliumTransaction {
	}

	public class NeuraliumStandardPresentationTransaction : StandardPresentationTransaction, INeuraliumStandardPresentationTransaction {

		protected override ITransactionAccountAttribute CreateTransactionAccountFeature() {
			return new NeuraliumTransactionAccountAttribute();
		}
	}
}