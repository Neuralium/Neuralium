using Neuralia.Blockchains.Core.General.Types.Constants;
using Neuralia.Blockchains.Core.General.Types.Simple;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods {

	public class BountyAllocationMethodType : SimpleUShort<BountyAllocationMethodType> {

		public BountyAllocationMethodType() {
		}

		public BountyAllocationMethodType(byte value) : base(value) {
		}

		public static implicit operator BountyAllocationMethodType(byte d) {
			return new BountyAllocationMethodType(d);
		}

		public static bool operator ==(BountyAllocationMethodType a, BountyAllocationMethodType b) {
			if(ReferenceEquals(null, a)) {
				return ReferenceEquals(null, b);
			}

			return a.Equals(b);
		}

		public static bool operator !=(BountyAllocationMethodType a, BountyAllocationMethodType b) {
			return !(a == b);
		}
	}

	public sealed class BountyAllocationMethodTypes : UShortConstantSet<BountyAllocationMethodType> {

		public readonly BountyAllocationMethodType EqualSplit;

		static BountyAllocationMethodTypes() {
		}

		private BountyAllocationMethodTypes() : base((ushort) 50) {
			this.EqualSplit = this.CreateBaseConstant();
		}

		public static BountyAllocationMethodTypes Instance { get; } = new BountyAllocationMethodTypes();
	}
}