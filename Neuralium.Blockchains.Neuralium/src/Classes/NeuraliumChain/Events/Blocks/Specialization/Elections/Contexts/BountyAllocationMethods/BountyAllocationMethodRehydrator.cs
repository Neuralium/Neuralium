using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods.V1;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.BountyAllocationMethods {
	public static class BountyAllocationMethodRehydrator {
		public static IBountyAllocationMethod Rehydrate(IDataRehydrator rehydrator) {

			ComponentVersion<BountyAllocationMethodType> version = rehydrator.RehydrateRewind<ComponentVersion<BountyAllocationMethodType>>();

			IBountyAllocationMethod bountyAllocationMethod = null;

			if(version.Type == BountyAllocationMethodTypes.Instance.EqualSplit) {
				if(version == (1, 0)) {
					bountyAllocationMethod = new EqualSplitBountyAllocationMethod();
				}
			}

			if(bountyAllocationMethod == null) {
				throw new ApplicationException("Invalid candidacy selector type");
			}

			bountyAllocationMethod.Rehydrate(rehydrator);

			return bountyAllocationMethod;
		}
	}
}