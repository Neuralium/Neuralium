using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {
	public interface INeuraliumPassiveFinalElectionResults : IPassiveFinalElectionResults, INeuraliumFinalElectionResults {
	}

	public class NeuraliumPassiveFinalElectionResults : PassiveFinalElectionResults, INeuraliumPassiveFinalElectionResults {
		private readonly NeuraliumFinalElectionResultsImplementation neuraliumPassiveFinalElectionResultsImplementation;

		public NeuraliumPassiveFinalElectionResults() {
			this.neuraliumPassiveFinalElectionResultsImplementation = new NeuraliumFinalElectionResultsImplementation();
		}

		public List<Amount> BountyAllocations => this.neuraliumPassiveFinalElectionResultsImplementation.BountyAllocations;

		public Amount InfrastructureServiceFees {
			get => this.neuraliumPassiveFinalElectionResultsImplementation.InfrastructureServiceFees;
			set => this.neuraliumPassiveFinalElectionResultsImplementation.InfrastructureServiceFees = value;
		}

		public override void Rehydrate(IDataRehydrator rehydrator, Dictionary<int, TransactionId> transactionIndexesTree) {

			base.Rehydrate(rehydrator, transactionIndexesTree);

			this.neuraliumPassiveFinalElectionResultsImplementation.Rehydrate(rehydrator, transactionIndexesTree);
		}

		public override IDelegateResults CreateDelegateResult() {
			return this.neuraliumPassiveFinalElectionResultsImplementation.CreateDelegateResult();
		}

		public override IElectedResults CreateElectedResult() {
			return this.neuraliumPassiveFinalElectionResultsImplementation.CreateElectedResult();
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			base.JsonDehydrate(jsonDeserializer);
			this.neuraliumPassiveFinalElectionResultsImplementation.JsonDehydrate(jsonDeserializer);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.neuraliumPassiveFinalElectionResultsImplementation);

			return nodeList;
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {

			this.neuraliumPassiveFinalElectionResultsImplementation.RehydrateHeader(rehydrator);

			base.RehydrateHeader(rehydrator);
		}

		protected override void RehydrateAccountEntry(AccountId accountId, IElectedResults entry, IDataRehydrator rehydrator) {

			this.neuraliumPassiveFinalElectionResultsImplementation.RehydrateAccountEntry(accountId, (INeuraliumElectedResults) entry, rehydrator);

			base.RehydrateAccountEntry(accountId, entry, rehydrator);
		}

		protected override void RehydrateDelegateAccountEntry(AccountId accountId, IDelegateResults entry, IDataRehydrator rehydrator) {

			this.neuraliumPassiveFinalElectionResultsImplementation.RehydrateDelegateAccountEntry(accountId, (INeuraliumDelegateResults) entry, rehydrator);

			base.RehydrateDelegateAccountEntry(accountId, entry, rehydrator);
		}

		protected override IElectedResults CreateElectedResult(AccountId accountId) {
			return this.neuraliumPassiveFinalElectionResultsImplementation.CreateElectedResult(accountId);
		}
	}
}