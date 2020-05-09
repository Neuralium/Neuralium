using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security {

	public interface INeuraliumUnfreezeClearedFundsTransaction : INeuraliumModerationTransaction {
		ushort FreezeId { get; set; }
		Text Reason { get; set; }
		List<NeuraliumUnfreezeClearedFundsTransaction.AccountUnfreeze> Accounts { get; }
	}

	public class NeuraliumUnfreezeClearedFundsTransaction : ModerationTransaction, INeuraliumUnfreezeClearedFundsTransaction {

		public ushort FreezeId { get; set; }
		public Text Reason { get; set; } = new Text();

		public List<AccountUnfreeze> Accounts { get; } = new List<AccountUnfreeze>();

		public override HashNodeList GetStructuresArray() {
			HashNodeList hasNodes = new HashNodeList();

			hasNodes.Add(this.FreezeId);
			hasNodes.Add(this.Reason);
			hasNodes.Add(this.Accounts);

			return hasNodes;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {

			jsonDeserializer.SetProperty("FreezeId", this.FreezeId);
			jsonDeserializer.SetProperty("Reason", this.Reason);
			jsonDeserializer.SetArray("Accounts", this.Accounts);
		}

		public override ImmutableList<AccountId> TargetAccounts => this.Accounts.Select(e => e.AccountId).ToImmutableList();

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			this.FreezeId = dataChannels.ContentsData.ReadUShort();
			this.Reason.Rehydrate(dataChannels.ContentsData);
			dataChannels.ContentsData.ReadRehydratableArray(this.Accounts);
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			dataChannels.ContentsData.Write(this.FreezeId);
			this.Reason.Dehydrate(dataChannels.ContentsData);
			dataChannels.ContentsData.Write(this.Accounts);
		}

		protected override void Sanitize() {
			base.Sanitize();
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_UNFREEZE_SUSPICIOUSACCOUNTS, 1, 0);
		}

		public class AccountUnfreeze : ISerializableCombo {
			public AccountId AccountId { get; set; } = new AccountId();
			public Text Notes { get; set; } = new Text();

			public void Rehydrate(IDataRehydrator rehydrator) {

				this.AccountId.Rehydrate(rehydrator);
				this.Notes.Rehydrate(rehydrator);
			}

			public void Dehydrate(IDataDehydrator dehydrator) {
				this.AccountId.Dehydrate(dehydrator);
				this.Notes.Dehydrate(dehydrator);
			}

			public HashNodeList GetStructuresArray() {
				HashNodeList hasNodes = new HashNodeList();

				hasNodes.Add(this.AccountId);
				hasNodes.Add(this.Notes);

				return hasNodes;
			}

			public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
				jsonDeserializer.SetProperty("AccountId", this.AccountId);
				jsonDeserializer.SetProperty("Notes", this.Notes);
			}
		}
	}
}