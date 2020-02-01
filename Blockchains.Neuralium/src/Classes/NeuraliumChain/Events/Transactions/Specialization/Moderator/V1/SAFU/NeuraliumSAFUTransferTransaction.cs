using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Common.Classes.Tools.Serialization;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.SAFU {

	public interface INeuraliumSAFUTransferTransaction : INeuraliumModerationTransaction {
		Text Note { get; set; }
		List<RecipientSet> Recipients { get; }
	}

	public class NeuraliumSAFUTransferTransaction : ModerationTransaction, INeuraliumSAFUTransferTransaction {

		public Text Note { get; set; } = new Text();
		public List<RecipientSet> Recipients { get; } = new List<RecipientSet>();

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.Note);

			nodeList.Add(this.Recipients.Count);

			foreach(RecipientSet entry in this.Recipients) {
				nodeList.Add(entry.Recipient);
				nodeList.Add(entry.Amount);
			}

			return nodeList;
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("Note", this.Note);

			jsonDeserializer.SetArray("Recipients", this.Recipients, (ds, e) => {
				ds.WriteObject((s) => {
					s.SetProperty("Recipient", e.Recipient.ToString());
					s.SetProperty("Amount", e.Amount.Value);
				});
			});
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_SAFU_TRANSFER, 1, 0);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);

			this.Note.Rehydrate(rehydrator);

			this.Recipients.Clear();

			var parameters = new AccountIdGroupSerializer.AccountIdGroupSerializerRehydrateParameters<AccountId> {
				RehydrateExtraData = (accountId, offset, index, dh) => {

					RecipientSet entry = new RecipientSet();

					entry.Recipient = accountId;
					entry.Amount.Rehydrate(rehydrator);

					this.Recipients.Add(entry);
				}
			};

			AccountIdGroupSerializer.Rehydrate(rehydrator, true, parameters);
		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);

			this.Note.Dehydrate(dehydrator);

			var parameters = new AccountIdGroupSerializer.AccountIdGroupSerializerDehydrateParameters<RecipientSet, AccountId> {
				DehydrateExtraData = (entry, AccountId, offset, index, dh) => {

					entry.Amount.Dehydrate(dehydrator);
				}
			};

			AccountIdGroupSerializer.Dehydrate(this.Recipients.ToDictionary(e => e.Recipient, e => e), dehydrator, true, parameters);
		}

		public override ImmutableList<AccountId> TargetAccounts => this.Recipients.Select(e => e.Recipient).ToImmutableList();
	}
}