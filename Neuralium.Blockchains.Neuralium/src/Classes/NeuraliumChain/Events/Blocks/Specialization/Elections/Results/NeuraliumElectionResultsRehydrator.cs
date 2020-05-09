using System;
using System.Collections.Generic;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Contexts;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results {
	public class NeuraliumElectionResultsRehydrator : ElectionResultsRehydrator {

		public override IIntermediaryElectionResults RehydrateIntermediateResults(IDataRehydrator rehydrator, Dictionary<int, TransactionId> transactionIndexesTree) {
			ComponentVersion<ElectionContextType> version = rehydrator.RehydrateRewind<ComponentVersion<ElectionContextType>>();

			IIntermediaryElectionResults result = null;

			if(version.Type == ElectionContextTypes.Instance.Active) {
				if(version == (1, 0)) {
					result = new NeuraliumActiveIntermediaryElectionResults();
				}
			}

			if(version.Type == ElectionContextTypes.Instance.Passive) {
				if(version == (1, 0)) {
					result = new NeuraliumPassiveIntermediaryElectionResults();
				}
			}

			if(result == null) {
				throw new ApplicationException("Invalid context type");
			}

			result.Rehydrate(rehydrator, transactionIndexesTree);

			return result;
		}

		public override IFinalElectionResults RehydrateFinalResults(IDataRehydrator rehydrator, Dictionary<int, TransactionId> transactionIndexesTree) {
			ComponentVersion<ElectionContextType> version = rehydrator.RehydrateRewind<ComponentVersion<ElectionContextType>>();

			IFinalElectionResults result = null;

			if(version.Type == ElectionContextTypes.Instance.Active) {
				if(version == (1, 0)) {
					result = new NeuraliumActiveFinalElectionResults();
				}
			}

			if(version.Type == ElectionContextTypes.Instance.Passive) {
				if(version == (1, 0)) {
					result = new NeuraliumPassiveFinalElectionResults();
				}
			}

			if(result == null) {
				throw new ApplicationException("Invalid context type");
			}

			result.Rehydrate(rehydrator, transactionIndexesTree);

			return result;
		}
	}
}