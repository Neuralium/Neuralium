using System.Collections.Generic;
using System.Linq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Core.Serialization.OffsetCalculators;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1 {

	public interface INeuraliumFinalElectionResultsImplementation : IJsonSerializable, ITreeHashable {
		List<Amount> BountyAllocations { get; }
		Amount InfrastructureServiceFees { get; set; }
	}

	public class NeuraliumFinalElectionResultsImplementation : INeuraliumFinalElectionResultsImplementation {
		public List<Amount> BountyAllocations { get; } = new List<Amount>();
		public Amount InfrastructureServiceFees { get; set; }

		public virtual void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			jsonDeserializer.SetProperty("InfrastructureServiceFees", this.InfrastructureServiceFees?.Value);
		}

		public virtual HashNodeList GetStructuresArray() {

			HashNodeList hashNodeList = new HashNodeList();

			hashNodeList.Add(this.BountyAllocations);
			hashNodeList.Add(this.InfrastructureServiceFees);

			return hashNodeList;
		}

		public void Rehydrate(IDataRehydrator rehydrator, Dictionary<int, TransactionId> transactionIndexesTree) {

		}

		public void RehydrateHeader(IDataRehydrator rehydrator) {

			AdaptiveLong1_9 adaptiveLong = new AdaptiveLong1_9();

			// then the various bounties
			AmountSerializationOffsetsCalculator bountyCalculator = new AmountSerializationOffsetsCalculator();

			adaptiveLong.Rehydrate(rehydrator);
			ushort bountySetCount = (ushort) adaptiveLong.Value;

			this.BountyAllocations.Clear();

			if(bountySetCount > 0) {

				Amount bountyAmount = new Amount();

				for(ushort i = 0; i < bountySetCount; i++) {

					bountyAmount.Rehydrate(rehydrator);
					decimal offset = bountyAmount.Value;

					Amount bountyEntry = new Amount(bountyCalculator.RebuildValue(offset));

					this.BountyAllocations.Add(bountyEntry);

					bountyCalculator.AddLastOffset();
				}
			}

			// now retrieve the network service fees
			this.InfrastructureServiceFees = rehydrator.ReadRehydratable<Amount>();
		}

		public void RehydrateAccountEntry(AccountId accountId, INeuraliumElectedResults entry, IDataRehydrator rehydrator) {

			// now the bounty allocation

			AdaptiveLong1_9 bountyAllocationOffset = rehydrator.ReadRehydratable<AdaptiveLong1_9>();

			ushort index = (ushort) bountyAllocationOffset.Value;

			if(index != ushort.MaxValue) {
				// max value means there is no bounty associated
				entry.BountyShare = this.BountyAllocations[index];
			}
		}

		public void RehydrateDelegateAccountEntry(AccountId accountId, INeuraliumDelegateResults entry, IDataRehydrator rehydrator) {

			// now the bounty allocation

			AdaptiveLong1_9 bountyAllocationOffset = rehydrator.ReadRehydratable<AdaptiveLong1_9>();

			ushort index = (ushort) bountyAllocationOffset.Value;

			if(index != ushort.MaxValue) {
				// max value means there is no bounty associated
				entry.BountyShare = this.BountyAllocations[index];
			}
		}

		public IElectedResults CreateElectedResult(AccountId accountId) {
			return new NeuraliumElectedResults();
		}

		public IDelegateResults CreateDelegateResult() {
			return new NeuraliumDelegateResults();
		}

		public IElectedResults CreateElectedResult() {
			return new NeuraliumElectedResults();
		}
	}
}