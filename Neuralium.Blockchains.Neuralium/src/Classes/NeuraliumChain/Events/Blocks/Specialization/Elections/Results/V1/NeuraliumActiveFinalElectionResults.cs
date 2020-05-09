using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {
	public interface INeuraliumActiveFinalElectionResults : IActiveFinalElectionResults, INeuraliumFinalElectionResults {
	}

	public class NeuraliumActiveFinalElectionResults : ActiveFinalElectionResults, INeuraliumActiveFinalElectionResults {
		private readonly NeuraliumFinalElectionResultsImplementation neuraliumActiveFinalElectionResultsImplementation;

		public NeuraliumActiveFinalElectionResults() {
			this.neuraliumActiveFinalElectionResultsImplementation = new NeuraliumFinalElectionResultsImplementation();
		}

		public List<Amount> BountyAllocations => this.neuraliumActiveFinalElectionResultsImplementation.BountyAllocations;

		public Amount InfrastructureServiceFees {
			get => this.neuraliumActiveFinalElectionResultsImplementation.InfrastructureServiceFees;
			set => this.neuraliumActiveFinalElectionResultsImplementation.InfrastructureServiceFees = value;
		}

		public override void Rehydrate(IDataRehydrator rehydrator, Dictionary<int, TransactionId> transactionIndexesTree) {

			base.Rehydrate(rehydrator, transactionIndexesTree);

			this.neuraliumActiveFinalElectionResultsImplementation.Rehydrate(rehydrator, transactionIndexesTree);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.neuraliumActiveFinalElectionResultsImplementation);

			return nodeList;
		}

		public override IDelegateResults CreateDelegateResult() {
			return this.neuraliumActiveFinalElectionResultsImplementation.CreateDelegateResult();
		}

		public override IElectedResults CreateElectedResult() {
			return this.neuraliumActiveFinalElectionResultsImplementation.CreateElectedResult();
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			base.JsonDehydrate(jsonDeserializer);
			this.neuraliumActiveFinalElectionResultsImplementation.JsonDehydrate(jsonDeserializer);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {

			this.neuraliumActiveFinalElectionResultsImplementation.RehydrateHeader(rehydrator);

			base.RehydrateHeader(rehydrator);
		}

		protected override void RehydrateAccountEntry(AccountId accountId, IElectedResults entry, IDataRehydrator rehydrator) {

			this.neuraliumActiveFinalElectionResultsImplementation.RehydrateAccountEntry(accountId, (INeuraliumElectedResults) entry, rehydrator);

			base.RehydrateAccountEntry(accountId, entry, rehydrator);
		}

		protected override void RehydrateDelegateAccountEntry(AccountId accountId, IDelegateResults entry, IDataRehydrator rehydrator) {

			this.neuraliumActiveFinalElectionResultsImplementation.RehydrateDelegateAccountEntry(accountId, (INeuraliumDelegateResults) entry, rehydrator);

			base.RehydrateDelegateAccountEntry(accountId, entry, rehydrator);
		}

		protected override IElectedResults CreateElectedResult(AccountId accountId) {
			return this.neuraliumActiveFinalElectionResultsImplementation.CreateElectedResult(accountId);
		}
	}
}