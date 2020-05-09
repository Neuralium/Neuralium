using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using MoreLinq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Cryptography.Trees;
using Neuralia.Blockchains.Core.General;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Dynamic;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Serialization;
using Neuralia.Blockchains.Tools.Serialization;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security {

	public interface INeuraliumFreezeSuspiciousFundsTransaction : INeuraliumModerationTransaction {
		List<NeuraliumFreezeSuspiciousFundsTransaction.TransactionFlowNode> SuspectTransactionFreezeTree { get; }
		ushort FreezeId { get; set; }
		Text EventDescription { get; set; }
		DateTime AllegedTheftTimestamp { get; set; }
		(Dictionary<AccountId, decimal> victims, Dictionary<AccountId, decimal> holders) GetFlatImpactTree();
	}

	public class NeuraliumFreezeSuspiciousFundsTransaction : ModerationTransaction, INeuraliumFreezeSuspiciousFundsTransaction {

		private ImmutableList<AccountId> accountIds;

		public ushort FreezeId { get; set; }
		public Text EventDescription { get; set; } = new Text();
		public DateTime AllegedTheftTimestamp { get; set; }

		public override void JsonDehydrate(JsonDeserializer jsonDeserializer) {
			base.JsonDehydrate(jsonDeserializer);

			jsonDeserializer.SetProperty("FreezeId", this.FreezeId);
			jsonDeserializer.SetProperty("EventDescription", this.EventDescription);
			jsonDeserializer.SetProperty("AllegedTheftTimestamp", this.AllegedTheftTimestamp);
			jsonDeserializer.SetArray("SuspectTransactionFreezeTree", this.SuspectTransactionFreezeTree);
		}

		public override HashNodeList GetStructuresArray() {
			HashNodeList hasNodes = base.GetStructuresArray();

			hasNodes.Add(this.SuspectTransactionFreezeTree);

			return hasNodes;
		}

		public override ImmutableList<AccountId> TargetAccounts {
			get {
				if(this.accountIds == null) {
					static void GetAaccounts(IEnumerable<TransactionFlowNode> flows, ISet<AccountId> accountIdsSet) {

						foreach(TransactionFlowNode flow in flows) {

							if(!accountIdsSet.Contains(flow.TransactionId.Account)) {
								accountIdsSet.Add(flow.TransactionId.Account);
							}

							foreach(AccountId accountId in flow.Amounts.Select(e => e.Key)) {
								if(!accountIdsSet.Contains(accountId)) {
									accountIdsSet.Add(accountId);
								}
							}

							GetAaccounts(flow.OutgoingSuspectTransactions, accountIdsSet);
						}
					}

					HashSet<AccountId> accountIdsSet = new HashSet<AccountId>();
					GetAaccounts(this.SuspectTransactionFreezeTree, accountIdsSet);

					this.accountIds = accountIdsSet.ToImmutableList();
				}

				return this.accountIds;
			}
		}

		public List<TransactionFlowNode> SuspectTransactionFreezeTree { get; } = new List<TransactionFlowNode>();

		public (Dictionary<AccountId, decimal> victims, Dictionary<AccountId, decimal> holders) GetFlatImpactTree() {
			Dictionary<AccountId, decimal> impacts = new Dictionary<AccountId, decimal>();

			this.ParseTreeNode(this.SuspectTransactionFreezeTree, impacts);

			// now separate the victims form the holders
			List<AccountId> victimsAccountIds = this.SuspectTransactionFreezeTree.Select(e => e.TransactionId.Account).Distinct().ToList();

			Dictionary<AccountId, decimal> victims = new Dictionary<AccountId, decimal>();

			foreach(AccountId victimAccountId in victimsAccountIds) {
				victims.Add(victimAccountId, Math.Abs(impacts[victimAccountId]));
				impacts.Remove(victimAccountId);
			}

			return (victims, impacts.ToDictionary(e => e.Key, e => Math.Abs(Math.Max(e.Value, 0))));
		}

		protected override void RehydrateContents(ChannelsEntries<IDataRehydrator> dataChannels, ITransactionRehydrationFactory rehydrationFactory) {
			base.RehydrateContents(dataChannels, rehydrationFactory);

			this.FreezeId = dataChannels.ContentsData.ReadUShort();
			this.EventDescription.Rehydrate(dataChannels.ContentsData);
			this.AllegedTheftTimestamp = dataChannels.ContentsData.ReadDateTime();

			dataChannels.ContentsData.ReadRehydratableArray(this.SuspectTransactionFreezeTree);
		}

		protected override void DehydrateContents(ChannelsEntries<IDataDehydrator> dataChannels) {
			base.DehydrateContents(dataChannels);

			dataChannels.ContentsData.Write(this.FreezeId);
			this.EventDescription.Dehydrate(dataChannels.ContentsData);
			dataChannels.ContentsData.Write(this.AllegedTheftTimestamp);

			dataChannels.ContentsData.Write(this.SuspectTransactionFreezeTree);
		}

		protected override void Sanitize() {
			base.Sanitize();

			void Clean(List<TransactionFlowNode> nodes) {
				foreach(TransactionFlowNode node in nodes) {

					Dictionary<AccountId, Amount> temp = node.Amounts.ToDictionary();
					node.Amounts.Clear();

					foreach(KeyValuePair<AccountId, Amount> outgoing in temp) {
						node.Amounts.Add(outgoing.Key, NeuraliumUtilities.CapAndRound(outgoing.Value));
					}

					Clean(node.OutgoingSuspectTransactions);
				}
			}

			Clean(this.SuspectTransactionFreezeTree);

		}

		private void ParseTreeNode(List<TransactionFlowNode> nodes, Dictionary<AccountId, decimal> impacts) {

			foreach(TransactionFlowNode node in nodes) {

				if(!impacts.ContainsKey(node.TransactionId.Account)) {
					impacts.Add(node.TransactionId.Account, 0);
				}

				decimal total = 0;

				foreach(KeyValuePair<AccountId, Amount> outgoing in node.Amounts) {
					if(!impacts.ContainsKey(outgoing.Key)) {
						impacts.Add(outgoing.Key, 0);
					}

					impacts[outgoing.Key] += outgoing.Value;
					total += outgoing.Value;
				}

				impacts[node.TransactionId.Account] -= total;

				this.ParseTreeNode(node.OutgoingSuspectTransactions, impacts);
			}
		}

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (NeuraliumTransactionTypes.Instance.NEURALIUM_FREEZE_SUSPICIOUSACCOUNTS, 1, 0);
		}

		public class TransactionFlowNode : ISerializableCombo {
			public TransactionId TransactionId { get; set; } = new TransactionId();
			public Dictionary<AccountId, Amount> Amounts { get; } = new Dictionary<AccountId, Amount>();

			public List<TransactionFlowNode> OutgoingSuspectTransactions { get; } = new List<TransactionFlowNode>();

			public void Rehydrate(IDataRehydrator rehydrator) {

				this.TransactionId.Rehydrate(rehydrator);

				AdaptiveLong1_9 value = new AdaptiveLong1_9();
				value.Rehydrate(rehydrator);
				int count = (int) value.Value;

				this.Amounts.Clear();

				for(int i = 0; i < count; i++) {

					AccountId accountId = new AccountId();
					accountId.Rehydrate(rehydrator);

					Amount amount = new Amount();
					amount.Rehydrate(rehydrator);

					this.Amounts.Add(accountId, amount);
				}

				rehydrator.ReadRehydratableArray(this.OutgoingSuspectTransactions);
			}

			public void Dehydrate(IDataDehydrator dehydrator) {
				this.TransactionId.Dehydrate(dehydrator);

				AdaptiveLong1_9 value = new AdaptiveLong1_9();
				value.Value = this.Amounts.Count;
				value.Dehydrate(dehydrator);

				foreach(KeyValuePair<AccountId, Amount> entry in this.Amounts) {
					entry.Key.Dehydrate(dehydrator);
					entry.Value.Dehydrate(dehydrator);
				}

				dehydrator.Write(this.OutgoingSuspectTransactions);
			}

			public HashNodeList GetStructuresArray() {
				HashNodeList hasNodes = new HashNodeList();

				hasNodes.Add(this.TransactionId);
				hasNodes.Add(this.Amounts.Count);

				foreach(KeyValuePair<AccountId, Amount> entry in this.Amounts) {
					hasNodes.Add(entry.Key);
					hasNodes.Add(entry.Value);
				}

				hasNodes.Add(this.OutgoingSuspectTransactions);

				return hasNodes;
			}

			public void JsonDehydrate(JsonDeserializer jsonDeserializer) {
				jsonDeserializer.SetProperty("TransactionId", this.TransactionId);

				jsonDeserializer.SetArray("Amount", this.Amounts, (js, e) => {
					js.WriteObject(s => {
						(AccountId key, Amount value) = e;
						s.SetProperty("AccountId", key);
						s.SetProperty("Amount", value);
					});
				});

				jsonDeserializer.SetArray("OutgoingSuspectTransactions", this.OutgoingSuspectTransactions);
			}
		}
	}
}