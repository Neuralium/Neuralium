using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods {
	public interface IBountyAllocationMethod : IVersionable<BountyAllocationMethodType> {
	}

	/// <summary>
	///     By what method do we allocate the bounty
	/// </summary>
	public abstract class BountyAllocationMethod : Versionable<BountyAllocationMethodType>, IBountyAllocationMethod {
		
		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);
			
		}
	}
}