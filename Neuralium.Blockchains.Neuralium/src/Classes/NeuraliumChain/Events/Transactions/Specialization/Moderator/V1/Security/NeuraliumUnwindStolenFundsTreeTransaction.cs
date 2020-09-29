using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security {

	public interface INeuraliumUnwindStolenFundsTreeTransaction : INeuraliumModerationTransaction {
		ushort FreezeId { get; set; }
		Text EventDescription { get; set; }
		List<NeuraliumUnwindStolenFundsTreeTransaction.AccountUnwindImpact> AccountUnwindImpacts { get; }
		List<NeuraliumUnwindStolenFundsTreeTransaction.AccountRestoreImpact> AccountRestoreImpacts { get; }
	}

	/// <summary>
	///     This is a very special transaction that is meant to be user very rare if ever. The situation it would be used is in
	///     case of massive theft, MtGox or Binance style where many people lose massive amounts of money.
	///     In cases like this, we can freeze the stolen funds the thieves have taken and return it to their rightful owner.
	///     This transaction is meant to be a "Big Deal", and when used, mejor investogation will be performed,
	///     authorities may be contacted and serious proofs will be required to prove that stolen funds are legitimately used.
	///     This transaction si meant to make it safer for people to use this system by ensure that major theft can be stopped
	///     and potentially reversed.
	///     This transaction is not meant to be used at every whims every 5 minutes.
	/// </summary>
	public class NeuraliumUnwindStolenFundsTreeTransaction : ModerationTransaction, INeuraliumUnwindStolenFundsTreeTransaction {

		private AccountId[] accountIds;

		public ushort FreezeId { get; set; }
		public Text EventDescription { get; set; } = new Text();

		/// <summary>
		///     the ones that have funds they should not have
		/// </summary>
		public List<AccountUnwindImpact> AccountUnwindImpacts { get; } = new List<AccountUnwindImpact>();

		/// <summary>
		///     the ones that were wronged and have been robbed
		/// </summary>
		public List<AccountRestoreImpact> AccountRestoreImpacts { get; } = new List<AccountRestoreImpact>();

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			jsonDeserializer.SetProperty("FreezeId", this.FreezeId);
			jsonDeserializer.SetProperty("EventDescription", this.EventDescription);
			jsonDeserializer.SetArray("AccountUnwindImpacts", this.AccountUnwindImpacts);
			jsonDeserializer.SetArray("AccountRestoreImpacts", this.AccountRestoreImpacts);
		}

		public override HashNodeList GetStructuresArray(Enums.MutableStructureTypes types) {
			HashNodeList hasNodes = base.GetStructuresArray(types);

			hasNodes.Add(this.FreezeId);
			hasNodes.Add(this.EventDescription);
			hasNodes.Add(this.AccountUnwindImpacts);
			hasNodes.Add(this.AccountRestoreImpacts);

			return hasNodes;
		}

		public override Enums.TransactionTargetTypes TargetType => Enums.TransactionTargetTypes.Range;
		public override AccountId[] ImpactedAccounts =>this.TargetAccounts;
		public override AccountId[] TargetAccounts {
			get {
				if(this.accountIds == null) {

					List<AccountId> combinedAccounts = this.AccountRestoreImpacts.Select(a => a.AccountId).ToList();
					combinedAccounts.AddRange(this.AccountUnwindImpacts.Select(a => a.AccountId));

					this.accountIds = combinedAccounts.ToArray();
				}

				return this.accountIds;
			}
		}

		protected override void Sanitize() {
			base.Sanitize();

			foreach(AccountUnwindImpact entry in this.AccountUnwindImpacts) {
				entry.UnwoundAmount = NeuraliumUtilities.CapAndRound(entry.UnwoundAmount);
			}

			foreach(AccountRestoreImpact entry in this.AccountRestoreImpacts) {
				entry.RestoreAmount = NeuraliumUtilities.CapAndRound(entry.RestoreAmount);
			}
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_UNWIND_STOLEN_SUSPICIOUSACCOUNTS, 1, 0);
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			this.FreezeId = dataChannels.ContentsData.ReadUShort();
			this.EventDescription.Rehydrate(dataChannels.ContentsData);

			dataChannels.ContentsData.ReadRehydratableArray(this.AccountUnwindImpacts);
			dataChannels.ContentsData.ReadRehydratableArray(this.AccountRestoreImpacts);
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			dataChannels.ContentsData.Write(this.FreezeId);
			this.EventDescription.Dehydrate(dataChannels.ContentsData);

			dataChannels.ContentsData.Write(this.AccountUnwindImpacts);
			dataChannels.ContentsData.Write(this.AccountRestoreImpacts);
		}

		public class AccountUnwindImpact : ISerializableCombo {
			public AccountId AccountId { get; set; } = new AccountId();
			public Amount UnwoundAmount { get; set; } = new Amount();
			public Text Notes { get; set; } = new Text();

			public void Rehydrate(IDataRehydrator rehydrator) {

				this.AccountId.Rehydrate(rehydrator);
				this.UnwoundAmount.Rehydrate(rehydrator);
				this.Notes.Rehydrate(rehydrator);
			}

			public void Dehydrate(IDataDehydrator dehydrator) {
				this.AccountId.Dehydrate(dehydrator);
				this.UnwoundAmount.Dehydrate(dehydrator);
				this.Notes.Dehydrate(dehydrator);
			}

			public HashNodeList GetStructuresArray() {
				HashNodeList hasNodes = new HashNodeList();

				hasNodes.Add(this.AccountId);
				hasNodes.Add(this.UnwoundAmount);
				hasNodes.Add(this.Notes);

				return hasNodes;
			}

			public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
				jsonDeserializer.SetProperty("AccountId", this.AccountId);
				jsonDeserializer.SetProperty("UnwoundAmount", this.UnwoundAmount);
				jsonDeserializer.SetProperty("Notes", this.Notes);
			}
		}

		public class AccountRestoreImpact : ISerializableCombo {
			public AccountId AccountId { get; set; } = new AccountId();
			public Amount RestoreAmount { get; set; } = new Amount();
			public Text Notes { get; set; } = new Text();

			public void Rehydrate(IDataRehydrator rehydrator) {

				this.AccountId.Rehydrate(rehydrator);
				this.RestoreAmount.Rehydrate(rehydrator);
				this.Notes.Rehydrate(rehydrator);
			}

			public void Dehydrate(IDataDehydrator dehydrator) {
				this.AccountId.Dehydrate(dehydrator);
				this.RestoreAmount.Dehydrate(dehydrator);
				this.Notes.Dehydrate(dehydrator);
			}

			public HashNodeList GetStructuresArray() {
				HashNodeList hasNodes = new HashNodeList();

				hasNodes.Add(this.AccountId);
				hasNodes.Add(this.RestoreAmount);
				hasNodes.Add(this.Notes);

				return hasNodes;
			}

			public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
				jsonDeserializer.SetProperty("AccountId", this.AccountId);
				jsonDeserializer.SetProperty("RestoreAmount", this.RestoreAmount);
				jsonDeserializer.SetProperty("Notes", this.Notes);
			}
		}
	}
}