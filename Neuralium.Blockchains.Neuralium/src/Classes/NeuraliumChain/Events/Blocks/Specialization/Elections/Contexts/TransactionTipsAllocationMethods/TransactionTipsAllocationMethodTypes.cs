using Neuralia.Blockchains.Core.General.Types.Constants;
using Neuralia.Blockchains.Core.General.Types.Simple;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods {

	public class TransactionTipsAllocationMethodType : SimpleUShort<TransactionTipsAllocationMethodType> {

		public TransactionTipsAllocationMethodType() {
		}

		public TransactionTipsAllocationMethodType(byte value) : base(value) {
		}

		public static implicit operator TransactionTipsAllocationMethodType(byte d) {
			return new TransactionTipsAllocationMethodType(d);
		}

		public static bool operator ==(TransactionTipsAllocationMethodType a, TransactionTipsAllocationMethodType b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(TransactionTipsAllocationMethodType a, TransactionTipsAllocationMethodType b) {
			return !(a == b);
		}
	}

	public sealed class TransactionTipsAllocationMethodTypes : UShortConstantSet<TransactionTipsAllocationMethodType> {

		public readonly TransactionTipsAllocationMethodType LowestToHighest;

		static TransactionTipsAllocationMethodTypes() {
		}

		private TransactionTipsAllocationMethodTypes() : base((ushort) 50) {
			this.LowestToHighest = this.CreateBaseConstant();
		}

		public static TransactionTipsAllocationMethodTypes Instance { get; } = new TransactionTipsAllocationMethodTypes();
	}
}