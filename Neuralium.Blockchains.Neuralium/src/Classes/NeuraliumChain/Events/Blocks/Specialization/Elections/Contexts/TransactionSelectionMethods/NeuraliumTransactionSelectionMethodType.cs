using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionSelectionMethods {

	public class NeuraliumTransactionSelectionMethodTypes : TransactionSelectionMethodTypes {

		public readonly TransactionSelectionMethodType HighestTips;

		protected NeuraliumTransactionSelectionMethodTypes() {
			this.HighestTips = this.CreateChildConstant();
		}

		public static new NeuraliumTransactionSelectionMethodTypes Instance { get; } = new NeuraliumTransactionSelectionMethodTypes();

		public static bool operator ==(NeuraliumTransactionSelectionMethodTypes a, NeuraliumTransactionSelectionMethodTypes b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(NeuraliumTransactionSelectionMethodTypes a, NeuraliumTransactionSelectionMethodTypes b) {
			return !(a == b);
		}
	}
}