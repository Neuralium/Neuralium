using System.Collections.Generic;
using System.Collections.Immutable;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.SAFU {
	public interface INeuraliumDailySAFURatioTransaction : INeuraliumModerationTransaction {
		Amount SAFUDailyRatio { get; set; }
		Amount MinimumSAFUQuantity { get; set; }
		AdaptiveLong1_9 MaximumAmountDays { get; set; }
	}

	public class NeuraliumDailySAFURatioTransaction : ModerationTransaction, INeuraliumDailySAFURatioTransaction {

		/// <summary>
		///     this is the amount of neuraliums that must be paid to be part of the SAFU for 1 day and be able to protect 1
		///     neuralium.
		/// </summary>
		public Amount SAFUDailyRatio { get; set; } = new Amount();

		/// <summary>
		///     The minimum amount of Tokens that need to be included
		/// </summary>
		public Amount MinimumSAFUQuantity { get; set; } = new Amount();

		/// <summary>
		///     The maximum amount of days that can be used
		/// </summary>
		public AdaptiveLong1_9 MaximumAmountDays { get; set; } = new AdaptiveLong1_9();

		public override HashNodeList GetStructuresArray() {

			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.SAFUDailyRatio);
			nodeList.Add(this.MinimumSAFUQuantity);
			nodeList.Add(this.MaximumAmountDays);

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("SAFUDailyRatio", this.SAFUDailyRatio);
			jsonDeserializer.SetProperty("MinimumSAFUQuantity", this.MinimumSAFUQuantity);
			jsonDeserializer.SetProperty("MaximumAmountDays", this.MaximumAmountDays);
		}

		public override ImmutableList<AccountId> TargetAccounts => new List<AccountId>().ToImmutableList();

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_UPDATE_RATIO, 1, 0);
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			this.SAFUDailyRatio.Rehydrate(dataChannels.ContentsData);
			this.MinimumSAFUQuantity.Rehydrate(dataChannels.ContentsData);
			this.MaximumAmountDays.Rehydrate(dataChannels.ContentsData);
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			this.SAFUDailyRatio.Dehydrate(dataChannels.ContentsData);
			this.MinimumSAFUQuantity.Dehydrate(dataChannels.ContentsData);
			this.MaximumAmountDays.Dehydrate(dataChannels.ContentsData);
		}
	}
}