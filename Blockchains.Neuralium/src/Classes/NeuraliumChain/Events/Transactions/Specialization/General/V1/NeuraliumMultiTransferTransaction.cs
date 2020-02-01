using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Tags;
using Neuralia.Blockchains.Common.Classes.Tools.Serialization;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1 {

	public interface INeuraliumMultiTransferTransaction : INeuraliumTipingTransaction, IRateLimitedTransaction {
		decimal Total { get; }

		List<RecipientSet> Recipients { get; }
	}

	public class NeuraliumMultiTransferTransaction : NeuraliumTipingTransaction, INeuraliumMultiTransferTransaction {

		public decimal Total => this.Recipients.Sum(e => e.Amount.Value);

		public List<RecipientSet> Recipients { get; } = new List<RecipientSet>();

		public override HashNodeList GetStructuresArray() {
			HashNodeList nodeList = base.GetStructuresArray();

			nodeList.Add(this.Total);

			nodeList.Add(this.Recipients.Count);

			foreach(RecipientSet entry in this.Recipients) {
				nodeList.Add(entry.Recipient);
				nodeList.Add(entry.Amount);
			}

			return nodeList;
		}

		protected override void Sanitize() {
			base.Sanitize();

			var clone = this.Recipients.ToArray();
			this.Recipients.Clear();
			foreach(RecipientSet entry in clone) {

				this.Recipients.Add(new RecipientSet(){ Recipient = entry.Recipient, Amount = NeuraliumUtilities.CapAndRound(entry.Amount)});
			}
		}

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			//
			jsonDeserializer.SetProperty("Total", this.Total);

			jsonDeserializer.SetArray("Recipients", this.Recipients, (ds, e) => {

				ds.WriteObject((s) => {
					s.SetProperty("Recipient", e.Recipient.ToString());
					s.SetProperty("Amount", e.Amount.Value);
				});
			});
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (TOKEN_MULTI_TRANSFER: NeuraliumTransactionTypes.Instance.NEURALIUM_MULTI_TRANSFER, 1, 0);
		}

		protected override void RehydrateHeader(IDataRehydrator rehydrator) {
			base.RehydrateHeader(rehydrator);
			
			this.Recipients.Clear();

			var parameters = new AccountIdGroupSerializer.AccountIdGroupSerializerRehydrateParameters<AccountId> {
				RehydrateExtraData = (accountId, offset, index, dh) => {

					RecipientSet entry = new RecipientSet(accountId);
					
					entry.Amount.Rehydrate(rehydrator);

					this.Recipients.Add(entry);
				}
			};

			AccountIdGroupSerializer.Rehydrate(rehydrator, true, parameters);
		}

		protected override void DehydrateHeader(IDataDehydrator dehydrator) {
			base.DehydrateHeader(dehydrator);
			
			var parameters = new AccountIdGroupSerializer.AccountIdGroupSerializerDehydrateParameters<RecipientSet, AccountId> {
				DehydrateExtraData = (entry, AccountId, offset, index, dh) => {

					entry.Amount.Dehydrate(dehydrator);
				}
			};

			AccountIdGroupSerializer.Dehydrate(this.Recipients.ToDictionary(e => e.Recipient, e => e), dehydrator, true, parameters);
		}

		public override ImmutableList<AccountId> TargetAccounts => this.Recipients.Select(r => r.Recipient).ToImmutableList();
	}
}