using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {
	public interface INeuraliumChainOperatingRulesTransaction : IChainOperatingRulesTransaction, INeuraliumModerationTransaction {
		Amount SAFUDailyRatio { get; set; }
		Amount MinimumSAFUQuantity { get; set; }
		AdaptiveLong1_9 MaximumAmountDays { get; set; }

		Amount UBBAmount { get; set; }
		byte UBBBlockRate { get; set; }
	}

	public class NeuraliumChainOperatingRulesTransaction : ChainOperatingRulesTransaction, INeuraliumChainOperatingRulesTransaction {

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

		/// <summary>
		///     The amount to offer each registered account on the turn
		/// </summary>
		public Amount UBBAmount { get; set; } = new Amount();

		/// <summary>
		///     apply the universal bounty every X blocks
		/// </summary>
		public byte UBBBlockRate { get; set; }

		public override HashNodeList GetStructuresArray() {

			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.SAFUDailyRatio);
			nodeList.Add(this.MinimumSAFUQuantity);
			nodeList.Add(this.MaximumAmountDays);
			nodeList.Add(this.UBBAmount);
			nodeList.Add(this.UBBBlockRate);

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("SAFUDailyRatio", this.SAFUDailyRatio);
			jsonDeserializer.SetProperty("MinimumSAFUQuantity", this.MinimumSAFUQuantity);
			jsonDeserializer.SetProperty("MaximumAmountDays", this.MaximumAmountDays);
			jsonDeserializer.SetProperty("UBBAmount", this.UBBAmount);
			jsonDeserializer.SetProperty("UBBBlockRate", this.UBBBlockRate);
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (base.SetIdentity().Type.Value, 1, 0);
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			this.SAFUDailyRatio.Rehydrate(dataChannels.ContentsData);
			this.MinimumSAFUQuantity.Rehydrate(dataChannels.ContentsData);
			this.MaximumAmountDays.Rehydrate(dataChannels.ContentsData);
			this.UBBAmount.Rehydrate(dataChannels.ContentsData);
			this.UBBBlockRate = dataChannels.ContentsData.ReadByte();
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			this.SAFUDailyRatio.Dehydrate(dataChannels.ContentsData);
			this.MinimumSAFUQuantity.Dehydrate(dataChannels.ContentsData);
			this.MaximumAmountDays.Dehydrate(dataChannels.ContentsData);
			this.UBBAmount.Dehydrate(dataChannels.ContentsData);
			dataChannels.ContentsData.Write(this.UBBBlockRate);
		}
	}
}