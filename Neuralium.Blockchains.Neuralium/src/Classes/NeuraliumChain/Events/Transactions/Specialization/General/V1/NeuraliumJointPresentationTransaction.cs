using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1.Structures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1.Structures;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {
	public interface INeuraliumJointPresentationTransaction : IJointPresentationTransaction, INeuraliumTransaction {
	}

	public class NeuraliumJointPresentationTransaction : JointPresentationTransaction, INeuraliumJointPresentationTransaction {
		protected override ITransactionAccountAttribute CreateTransactionAccountFeature() {
			return new NeuraliumTransactionAccountAttribute();
		}

		protected override ITransactionJointAccountMember CreateTransactionJointAccountMember() {
			return new NeuraliumTransactionJointAccountMember();
		}
	}
}