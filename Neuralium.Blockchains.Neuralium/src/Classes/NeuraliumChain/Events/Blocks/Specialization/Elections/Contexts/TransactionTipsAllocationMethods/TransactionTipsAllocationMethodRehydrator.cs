using System;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods.V1;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Contexts.TransactionTipsAllocationMethods {
	public static class TransactionTipsAllocationMethodRehydrator {
		public static ITransactionTipsAllocationMethod Rehydrate(IDataRehydrator rehydrator) {

			ComponentVersion<TransactionTipsAllocationMethodType> version = rehydrator.RehydrateRewind<ComponentVersion<TransactionTipsAllocationMethodType>>();

			ITransactionTipsAllocationMethod allocationMethod = null;

			if(version.Type == TransactionTipsAllocationMethodTypes.Instance.LowestToHighest) {
				if(version == (1, 0)) {
					allocationMethod = new LowestToHighestTransactionTipsAllocationMethod();
				}
			}

			if(allocationMethod == null) {
				throw new ApplicationException("Invalid candidacy selector type");
			}

			allocationMethod.Rehydrate(rehydrator);

			return allocationMethod;
		}
	}
}