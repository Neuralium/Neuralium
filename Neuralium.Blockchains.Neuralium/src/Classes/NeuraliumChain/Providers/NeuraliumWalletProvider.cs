using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet.Extra;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation.V1;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet.Extra;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Components.Blocks;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Services;
using Neuralia.Blockchains.Tools;
using Neuralia.Blockchains.Tools.Locking;
using System.Text.RegularExpressions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Serialization.Exceptions;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Extensions;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.SAFU;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumWalletProvider : IWalletProvider {

		Task<decimal> GetUsableAccountBalance(string accountCode, LockContext lockContext);
		Task<TotalAPI> GetAccountBalance(bool includeReserved, LockContext lockContext);
		Task<TotalAPI> GetAccountBalance(AccountId accountId, bool includeReserved, LockContext lockContext);
		Task<TotalAPI> GetAccountBalance(string accountCode, bool includeReserved, LockContext lockContext);
		Task<TimelineHeader> GetTimelineHeader(string accountCode, LockContext lockContext);
		Task<TimelineDay> GetTimelineSection(string accountCode, DateTime day, LockContext lockContext);

		Task ApplyUniversalBasicBounties(string accountCode, Amount bounty, BlockId blockId, LockContext lockContext);
	}

	public interface INeuraliumWalletProviderInternal : INeuraliumWalletProvider, IWalletProviderInternal {
	}

	public class NeuraliumWalletProvider : WalletProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumWalletProviderInternal {

		public NeuraliumWalletProvider(INeuraliumCentralCoordinator centralCoordinator) : base(GlobalsService.TOKEN_CHAIN_NAME, centralCoordinator) {

		}

		public new INeuraliumWalletSerialisationFal SerialisationFal => (INeuraliumWalletSerialisationFal) base.SerialisationFal;

		protected override ICardUtils CardUtils => NeuraliumCardsUtils.Instance;

		public override async Task<IWalletElectionsHistory> InsertElectionsHistoryEntry(SynthesizedBlock.SynthesizedElectionResult electionResult, SynthesizedBlock synthesizedBlock, AccountId electedAccountId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletElectionsHistory historyEntry = await base.InsertElectionsHistoryEntry(electionResult, synthesizedBlock, electedAccountId, lockContext).ConfigureAwait(false);

			// now let's add a neuralium timeline entry
			if(historyEntry is INeuraliumWalletElectionsHistory neuraliumWalletElectionsHistory && electionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult) {

				IWalletAccount account = (await this.WalletFileInfo.WalletBase(lockContext).ConfigureAwait(false)).Accounts.Values.SingleOrDefault(a => a.GetAccountId() == electedAccountId);

				if(account == null) {
					throw new ApplicationException("Invalid account");
				}

				if(this.WalletFileInfo.Accounts[account.AccountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
					NeuraliumWalletTimeline neuraliumWalletTimeline = new NeuraliumWalletTimeline();
					INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

					neuraliumWalletTimeline.BlockId = electionResult.BlockId;

					neuraliumWalletTimeline.Timestamp = neuraliumSynthesizedElectionResult.Timestamp.ToUniversalTime();
					neuraliumWalletTimeline.Amount = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].bountyShare;
					neuraliumWalletTimeline.Tips = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].tips;

					neuraliumWalletTimeline.RecipientAccountIds = electedAccountId.ToString();
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Credit;
					neuraliumWalletTimeline.CreditType = NeuraliumWalletTimeline.CreditTypes.Election;

					neuraliumWalletTimeline.Credit = neuraliumWalletTimeline.Amount + neuraliumWalletTimeline.Tips;

					neuraliumWalletTimeline.TimestampTotal = (await this.GetAccountBalance(electedAccountId, false, lockContext).ConfigureAwait(false)).Total + neuraliumWalletTimeline.Credit;

					neuraliumWalletTimeline.Confirmed = true;

					await neuraliumWalletTimelineFileInfo.InsertTimelineEntry(neuraliumWalletTimeline, lockContext).ConfigureAwait(false);

					this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());

					await this.SendTotal(account, lockContext).ConfigureAwait(false);
				}
			}

			return historyEntry;
		}

		protected override async Task UpdateLocalTransactionHistoryEntry(IWalletTransactionHistoryFileInfo transactionHistoryFileInfo, ITransaction transaction, TransactionId transactionId, WalletTransactionHistory.TransactionStatuses status, LockContext lockContext) {
			await base.UpdateLocalTransactionHistoryEntry(transactionHistoryFileInfo, transaction, transactionId, status, lockContext).ConfigureAwait(false);

			// sometimes tip transactions had a tip, but the tip was removed if not caught by a miner. in this case, we update our history to reflect this.
			if(status == WalletTransactionHistory.TransactionStatuses.Confirmed && transaction is ITipTransaction tipTransaction && transactionHistoryFileInfo is INeuraliumWalletTransactionHistoryFileInfo neuraliumWalletTransactionHistoryFileInfo) {
				await neuraliumWalletTransactionHistoryFileInfo.UpdateTransactionTip(transactionId, tipTransaction.Tip.Value, lockContext).ConfigureAwait(false);
			}

			if(transactionHistoryFileInfo is INeuraliumWalletTransactionHistoryFileInfo neuraliumWalletTransactionHistoryFile) {
				await this.SendTotal(transactionId.Account, lockContext).ConfigureAwait(false);
			}
		}

		public override async Task<IWalletTransactionHistoryFileInfo> UpdateLocalTransactionHistoryEntry(ITransaction transaction, TransactionId transactionId, WalletTransactionHistory.TransactionStatuses status, BlockId blockId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletTransactionHistoryFileInfo historyEntry = await base.UpdateLocalTransactionHistoryEntry(transaction, transactionId, status, blockId, lockContext).ConfigureAwait(false);

			IUserWallet walletbase = await this.WalletFileInfo.WalletBase(lockContext).ConfigureAwait(false);
			IWalletAccount account = walletbase.Accounts.Values.SingleOrDefault(a => (a.GetAccountId() == transactionId.Account) || (a.PresentationTransactionId == transactionId));

			if(account == null) {
				throw new ApplicationException("Invalid account");
			}

			if(historyEntry is INeuraliumWalletTransactionHistoryFileInfo neuraliumWalletTransactionHistoryFileInfo && this.WalletFileInfo.Accounts[account.AccountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

				if(status == WalletTransactionHistory.TransactionStatuses.Confirmed) {

					decimal? tip = null;

					if(transaction is ITipTransaction tipTransaction) {
						tip = tipTransaction.Tip;
					}

					// now let's add a neuralium timeline entry
					await neuraliumWalletTimelineFileInfo.ConfirmLocalTimelineEntry(transactionId, tip, blockId, lockContext).ConfigureAwait(false);

				} else if(status == WalletTransactionHistory.TransactionStatuses.Rejected) {
					// now let's add a neuralium timeline entry
					await neuraliumWalletTimelineFileInfo.RemoveLocalTimelineEntry(transactionId, lockContext).ConfigureAwait(false);
				}

				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());
				AccountId targetAccountId = transactionId.Account;

				TotalAPI total = await this.GetAccountBalance(targetAccountId, true, lockContext).ConfigureAwait(false);
				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(targetAccountId.SequenceId, targetAccountId.AccountType, total));
			}

			return historyEntry;
		}

		protected override async Task InsertedTransactionHistoryEntry(IWalletTransactionHistory walletAccountTransactionHistory, IWalletAccount account, ITransaction transaction, BlockId blockId, LockContext lockContext) {
			await base.InsertedTransactionHistoryEntry(walletAccountTransactionHistory, account, transaction, blockId, lockContext).ConfigureAwait(false);

			if(walletAccountTransactionHistory is INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory) {

				await this.InsertNeuraliumTransactionTimelineEntry(transaction, neuraliumWalletTransactionHistory,account, blockId, lockContext).ConfigureAwait(false);

				await this.SendTotal(account, lockContext).ConfigureAwait(false);
			}
		}

		public override async Task<List<WalletTransactionHistoryHeaderAPI>> APIQueryWalletTransactionHistory(string accountCode, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(!this.IsWalletLoaded) {
				throw new ApplicationException("Wallet is not loaded");
			}

			if(string.IsNullOrWhiteSpace(accountCode)) {
				accountCode = await this.GetAccountCode(lockContext).ConfigureAwait(false);
			}

			if(!await this.WalletFileInfo.Accounts[accountCode].WalletTransactionHistoryInfo.CollectionExists<NeuraliumWalletTransactionHistory>(lockContext).ConfigureAwait(false)) {
				return new List<WalletTransactionHistoryHeaderAPI>();
			}

			//TODO: merge correctly with base version of this method
			NeuraliumWalletTransactionHistoryHeaderAPI[] results = await this.WalletFileInfo.Accounts[accountCode].WalletTransactionHistoryInfo.RunQuery<NeuraliumWalletTransactionHistoryHeaderAPI, NeuraliumWalletTransactionHistory>(caches => caches.Select(t => {

				TransactionId transactionId = new TransactionId(t.TransactionId);
				ComponentVersion<TransactionType> version = new ComponentVersion<TransactionType>(t.Version);

				return new NeuraliumWalletTransactionHistoryHeaderAPI {
					TransactionId = t.TransactionId, Sender = transactionId.Account.ToString(), Timestamp = TimeService.FormatDateTimeStandardUtc(t.Timestamp), Status = (byte) t.Status,
					Version = new VersionAPI {TransactionType = version.Type.Value.Value, Major = version.Major.Value, Minor = version.Minor}, Recipient = t.Recipient, Local = t.Local, Amount = t.Amount,
					Tip = t.Tip, Note = t.Note
				};
			}).OrderByDescending(t => t.Timestamp).ToList(), lockContext).ConfigureAwait(false);

			return results.Cast<WalletTransactionHistoryHeaderAPI>().ToList();

		}

		public override async Task<WalletTransactionHistoryDetailsAPI> APIQueryWalletTransactionHistoryDetails(string accountCode, string transactionId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(!this.IsWalletLoaded) {
				throw new ApplicationException("Wallet is not loaded");
			}

			if(string.IsNullOrWhiteSpace(accountCode)) {
				accountCode = await this.GetAccountCode(lockContext).ConfigureAwait(false);
			}

			if(!await this.WalletFileInfo.Accounts[accountCode].WalletTransactionHistoryInfo.CollectionExists<NeuraliumWalletTransactionHistory>(lockContext).ConfigureAwait(false)) {
				return new WalletTransactionHistoryDetailsAPI();
			}

			NeuraliumWalletTransactionHistoryDetailsAPI[] results = await this.WalletFileInfo.Accounts[accountCode].WalletTransactionHistoryInfo.RunQuery<NeuraliumWalletTransactionHistoryDetailsAPI, NeuraliumWalletTransactionHistory>(caches => caches.Where(t => t.TransactionId == transactionId).Select(t => {

				ComponentVersion<TransactionType> version = new ComponentVersion<TransactionType>(t.Version);

				return new NeuraliumWalletTransactionHistoryDetailsAPI {
					TransactionId = t.TransactionId, Sender = new TransactionId(t.TransactionId).Account.ToString(), Timestamp = TimeService.FormatDateTimeStandardUtc(t.Timestamp), Status = (byte) t.Status,
					Version = new VersionAPI {TransactionType = version.Type.Value.Value, Major = version.Major.Value, Minor = version.Minor}, Recipient = t.Recipient, Contents = t.Contents, Local = t.Local,
					Amount = t.Amount, Tip = t.Tip, Note = t.Note
				};
			}).ToList(), lockContext).ConfigureAwait(false);

			return results.SingleOrDefault();
		}

		public async Task SendTotal(IWalletAccount account, LockContext lockContext) {
			TotalAPI total = await this.GetAccountBalance(account, true, lockContext).ConfigureAwait(false);
			var accountId = account.GetAccountId();
			this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(accountId.SequenceId, accountId.AccountType, total));
		}

		public async Task SendTotal(AccountId accountId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletAccount account = await this.GetWalletAccount(accountId, lockContext).ConfigureAwait(false);

			await this.SendTotal(account, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(bool includeReserved, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletAccount account = await this.GetActiveAccount(lockContext).ConfigureAwait(false);

			return await this.GetAccountBalance(account, includeReserved, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(AccountId accountId, bool includeReserved, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletAccount account = await this.GetWalletAccount(accountId, lockContext).ConfigureAwait(false);

			return await this.GetAccountBalance(account, includeReserved, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(string accountCode, bool includeReserved, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(!this.WalletFileInfo.Accounts.ContainsKey(accountCode)) {
				return new TotalAPI();
			}

			IWalletAccount account = await this.GetWalletAccount(accountCode, lockContext).ConfigureAwait(false);

			return await this.GetAccountBalance(account, includeReserved, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(IWalletAccount account, bool includeReserved, LockContext lockContext) {

			TotalAPI result = new TotalAPI();

			if(account == null) {
				return result;
			}

			IWalletAccountSnapshot accountBase = await this.GetStandardAccountSnapshot(account.GetAccountId(), lockContext).ConfigureAwait(false);

			if(accountBase is INeuraliumWalletAccountSnapshot walletAccountSnapshot) {
				result.Total = walletAccountSnapshot.Balance;
			}

			if(includeReserved) {
				var accountHistoryBase = this.WalletFileInfo.Accounts[account.AccountCode].WalletTransactionHistoryInfo;

				if(accountHistoryBase is NeuraliumWalletTransactionHistoryFileInfo neuraliumWalletTransactionHistoryFileInfo) {
					(decimal debit, decimal credit, decimal tip) results = await neuraliumWalletTransactionHistoryFileInfo.GetPendingTransactionAmounts(lockContext).ConfigureAwait(false);

					result.ReservedDebit = results.debit + results.tip;
					result.ReservedCredit = results.credit;
				}

				if(accountBase != null) {
					foreach(IAccountAttribute entry in accountBase.AppliedAttributesBase) {
						result.Frozen += NeuraliumSnapshotUtilities.GetImpactAmount(entry);
					}
				}
			}

			return result;
		}

		public async Task<decimal> GetUsableAccountBalance(string accountCode, LockContext lockContext) {
			var balance = await this.GetAccountBalance(accountCode, true, lockContext).ConfigureAwait(false);

			return balance.Total - (balance.Frozen + balance.ReservedDebit);
		}

		protected override (MiningStatisticSessionAPI session, MiningStatisticAggregateAPI aggregate) CreateMiningStatisticSet() {
			return (new NeuraliumMiningStatisticSessionApi(), new NeuraliumMiningStatisticAggregateApi());
		}

		protected override void PrepareMiningStatisticAggregateSet(MiningStatisticAggregateAPI miningStatisticAggregateSet, WalletElectionsMiningAggregateStatistics aggregateStatisticsEntry) {
			base.PrepareMiningStatisticAggregateSet(miningStatisticAggregateSet, aggregateStatisticsEntry);

			if(miningStatisticAggregateSet is NeuraliumMiningStatisticAggregateApi neuraliumMiningStatistic && aggregateStatisticsEntry is NeuraliumWalletElectionsMiningAggregateStatistics neuraliumWalletElectionsMiningStatistics) {
				neuraliumMiningStatistic.AverageBountyPerBlock = 0;
				neuraliumMiningStatistic.TotalBounties = 0;
				neuraliumMiningStatistic.TotalTips = 0;

				if(neuraliumWalletElectionsMiningStatistics.BlocksElected != 0) {

					neuraliumMiningStatistic.TotalBounties = (double) neuraliumWalletElectionsMiningStatistics.TotalBounty;
					neuraliumMiningStatistic.TotalTips = (double) neuraliumWalletElectionsMiningStatistics.TotalTips;

					neuraliumMiningStatistic.AverageBountyPerBlock = (double) (neuraliumWalletElectionsMiningStatistics.TotalBounty / neuraliumWalletElectionsMiningStatistics.BlocksElected);
				}
			}
		}

		protected override void PrepareMiningStatisticSessionSet(MiningStatisticSessionAPI miningStatisticSessionSet, WalletElectionsMiningSessionStatistics sessionStatisticsEntry) {
			base.PrepareMiningStatisticSessionSet(miningStatisticSessionSet, sessionStatisticsEntry);

			if(miningStatisticSessionSet is NeuraliumMiningStatisticSessionApi neuraliumMiningStatistic && sessionStatisticsEntry is NeuraliumWalletElectionsMiningSessionStatistics neuraliumWalletElectionsMiningStatistics) {
				neuraliumMiningStatistic.AverageBountyPerBlock = 0;
				neuraliumMiningStatistic.TotalBounties = 0;
				neuraliumMiningStatistic.TotalTips = 0;

				if(neuraliumWalletElectionsMiningStatistics.BlocksElected != 0) {

					neuraliumMiningStatistic.TotalBounties = (double) neuraliumWalletElectionsMiningStatistics.TotalBounty;
					neuraliumMiningStatistic.TotalTips = (double) neuraliumWalletElectionsMiningStatistics.TotalTips;

					neuraliumMiningStatistic.AverageBountyPerBlock = (double) (neuraliumWalletElectionsMiningStatistics.TotalBounty / neuraliumWalletElectionsMiningStatistics.BlocksElected);
				}
			}
		}

		public override async Task<Dictionary<AccountId, int>> ClearTimedOutTransactions(LockContext lockContext) {
			Dictionary<AccountId, int> totals = await base.ClearTimedOutTransactions(lockContext).ConfigureAwait(false);

			foreach(KeyValuePair<AccountId, int> nonZero in totals.Where(e => e.Value != 0)) {

				// alert of the new total, since it changed
				TotalAPI total = await this.GetAccountBalance(nonZero.Key, true, lockContext).ConfigureAwait(false);
				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(nonZero.Key.SequenceId, nonZero.Key.AccountType, total));
			}

			return totals;
		}

		private async Task InsertNeuraliumTransactionTimelineEntry(ITransaction transaction, INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory, IWalletAccount account, BlockId blockId, LockContext lockContext) {
			if((neuraliumWalletTransactionHistory.Amount == 0) && (neuraliumWalletTransactionHistory.Tip == 0)) {
				// this transaction is most probably not a token influencing transaction. let's ignore 0 values
				return;
			}
			
			bool isLocal = neuraliumWalletTransactionHistory.Local;
			AccountId accountId = account.GetAccountId();

			if(this.WalletFileInfo.Accounts[account.AccountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				NeuraliumWalletTimeline neuraliumWalletTimeline = new NeuraliumWalletTimeline();
				INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

				neuraliumWalletTimeline.Timestamp = this.serviceSet.TimeService.GetTimestampDateTime(transaction.TransactionId.Timestamp.Value, this.centralCoordinator.ChainComponentProvider.ChainStateProvider.ChainInception);
				neuraliumWalletTimeline.Amount = neuraliumWalletTransactionHistory.Amount;

				neuraliumWalletTimeline.TransactionId = transaction.TransactionId.ToString();

				var sender = transaction.TransactionId.Account;
				neuraliumWalletTimeline.SenderAccountId = sender;
				neuraliumWalletTimeline.CreditType = NeuraliumWalletTimeline.CreditTypes.Transaction;

				if(isLocal) {

					// we only take 
					neuraliumWalletTimeline.Tips = neuraliumWalletTransactionHistory.Tip;
					neuraliumWalletTimeline.Debit = neuraliumWalletTimeline.Tips;

					// in most cases, transactions we make wil be debits
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Debit;
					neuraliumWalletTimeline.Confirmed = false;

					neuraliumWalletTimeline.RecipientAccountIds = (transaction.TargetAccountsSerialized.Contains(",", StringComparison.InvariantCulture) ? Regex.Replace(transaction.TargetAccountsSerialized, $@"({sender})|(,{sender})", string.Empty) : transaction.TargetAccountsSerialized).Truncate(100, true);
				} else {
					neuraliumWalletTimeline.BlockId = blockId;
					neuraliumWalletTimeline.Confirmed = true;
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Credit;

					// since this is not our transaction, we already know we are targetted, and we dont want to waste space with the others
					neuraliumWalletTimeline.RecipientAccountIds = null;
				}
				
				void ApplyDebitCredit(decimal amount, bool localDebit) {
					if(isLocal) {
						if(localDebit) {
							neuraliumWalletTimeline.Debit += amount;
						} else {
							neuraliumWalletTimeline.Credit += amount;
						}
					} else {
						if(localDebit) {
							neuraliumWalletTimeline.Credit += amount;
						} else {
							neuraliumWalletTimeline.Debit += amount;
						}
					}
				}
				if(transaction is INeuraliumTransferTransaction neuraliumTransferTransaction) {

					neuraliumWalletTransactionHistory.Amount = neuraliumTransferTransaction.Amount;
					ApplyDebitCredit(neuraliumTransferTransaction.Amount, true);
				} else if(transaction is INeuraliumMultiTransferTransaction neuraliumMultiTransferTransaction) {

					if(isLocal) {
						neuraliumWalletTransactionHistory.Amount = neuraliumMultiTransferTransaction.Amount;
						ApplyDebitCredit(neuraliumWalletTransactionHistory.Amount, true);
					} else {
						var impact = neuraliumMultiTransferTransaction.Recipients.SingleOrDefault(r => r.Recipient == accountId);

						if(impact != null) {
							neuraliumWalletTransactionHistory.Amount =  impact.Amount;
							
							ApplyDebitCredit(neuraliumWalletTransactionHistory.Amount, false);
						}
					}
				}
				else if(transaction is INeuraliumThreeWayGatedTransferTransaction neuraliumThreeWayGatedTransferTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);

					//TODO: implement this method for ThreeWayGatedTransferTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumGatedJudgementTransaction neuraliumGatedJudgementTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumSAFUContributionTransaction neuraliumSafuContributionTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumSAFUTransferTransaction neuraliumSafuTransferTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumFreezeSuspiciousFundsTransaction neuraliumFreezeSuspiciousFundsTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumUnfreezeClearedFundsTransaction neuraliumUnfreezeClearedFundsTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumUnwindStolenFundsTreeTransaction neuraliumUnwindStolenFundsTreeTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
#if DEVENET || TESTNET
				else if(transaction is INeuraliumRefillNeuraliumsTransaction neuraliumsTransaction) {
					if(isLocal) {
						neuraliumWalletTransactionHistory.Amount = 1000;
						ApplyDebitCredit(neuraliumWalletTransactionHistory.Amount, false);
					}
				}
#endif
				else {
					//TODO: add any other transactions here
				}

				decimal? lastTotal = await neuraliumWalletTimelineFileInfo.GetLastTotal(lockContext).ConfigureAwait(false);

				bool adjust = true;
				if(lastTotal.HasValue) {
					neuraliumWalletTimeline.TimestampTotal = lastTotal.Value;
					// this total is already adjusted
				} else {
					// we dont have anything in the timeline, so lets take it from the wallet
					neuraliumWalletTimeline.TimestampTotal = (await this.GetAccountBalance(account.GetAccountId(), false, lockContext).ConfigureAwait(false)).Total;

					// we dont always adjust,  if it is not our own, the total was already adjusted
					adjust = isLocal;
				}

				if(adjust) {
					neuraliumWalletTimeline.TimestampTotal += (neuraliumWalletTimeline.Credit - neuraliumWalletTimeline.Debit);
				}

				await neuraliumWalletTimelineFileInfo.InsertTimelineEntry(neuraliumWalletTimeline, lockContext).ConfigureAwait(false);

				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());

			}
		}

		protected override IAccountFileInfo CreateNewAccountFileInfo(AccountPassphraseDetails accountSecurityDetails) {
			return new NeuraliumAccountFileInfo(accountSecurityDetails);
		}

		protected override void FillWalletElectionsHistoryEntry(IWalletElectionsHistory walletElectionsHistory, SynthesizedBlock.SynthesizedElectionResult electionResult, AccountId electedAccountId) {

			if(walletElectionsHistory is INeuraliumWalletElectionsHistory neuraliumWalletElectionsHistory && electionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult) {

				neuraliumWalletElectionsHistory.Bounty = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].bountyShare;
				neuraliumWalletElectionsHistory.Tips = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].tips;
			}
		}

		protected override async Task FillWalletTransactionHistoryEntry(IWalletTransactionHistory walletAccountTransactionHistory, ITransaction transaction, IWalletAccount account, bool local, string note, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			await base.FillWalletTransactionHistoryEntry(walletAccountTransactionHistory, transaction, account, local, note, lockContext).ConfigureAwait(false);

			AccountId accountId = account.GetAccountId();
			if(walletAccountTransactionHistory is INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory) {
				
				//here we record the impact amount. + value increases our amount. - reduces
				if(transaction is INeuraliumTransferTransaction neuraliumTransferTransaction) {

					neuraliumWalletTransactionHistory.Amount = neuraliumTransferTransaction.Amount;
					neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Debit;
				} else if(transaction is INeuraliumMultiTransferTransaction neuraliumMultiTransferTransaction) {

					if(walletAccountTransactionHistory.Local) {
						neuraliumWalletTransactionHistory.Amount = neuraliumMultiTransferTransaction.Amount;
					} else {
						var impact = neuraliumMultiTransferTransaction.Recipients.SingleOrDefault(r => r.Recipient == accountId);

						if(impact != null) {
							neuraliumWalletTransactionHistory.Amount =  impact.Amount;
						}
					}

					neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Debit;
					neuraliumWalletTransactionHistory.Recipient = string.Join(",", neuraliumMultiTransferTransaction.Recipients.Select(a => a.Recipient).OrderBy(a => a.ToLongRepresentation()));
				}
				else if(transaction is INeuraliumThreeWayGatedTransferTransaction neuraliumThreeWayGatedTransferTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for ThreeWayGatedTransferTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumGatedJudgementTransaction neuraliumGatedJudgementTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumSAFUContributionTransaction neuraliumSafuContributionTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumSAFUTransferTransaction neuraliumSafuTransferTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumFreezeSuspiciousFundsTransaction neuraliumFreezeSuspiciousFundsTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumUnfreezeClearedFundsTransaction neuraliumUnfreezeClearedFundsTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
				else if(transaction is INeuraliumUnwindStolenFundsTreeTransaction neuraliumUnwindStolenFundsTreeTransaction) {
					throw new UnrecognizedElementException(this.CentralCoordinator.ChainId, this.CentralCoordinator.ChainName);
					//TODO: implement this method for GatedJudgementTransaction
					// if(walletAccountTransactionHistory.Local) {
					// 	neuraliumWalletTransactionHistory.Amount = neuraliumGatedJudgementTransaction.;
					// 	neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					// }
				} 
#if DEVENET || TESTNET
				else if(transaction is INeuraliumRefillNeuraliumsTransaction neuraliumsTransaction) {
					if(walletAccountTransactionHistory.Local) {
						neuraliumWalletTransactionHistory.Amount = 1000;
						neuraliumWalletTransactionHistory.BookkeepingType = Enums.BookkeepingTypes.Credit;
					}
				}
#endif
				else {
					//TODO: add any other transactions here
				}

				if(walletAccountTransactionHistory.Local && transaction is ITipTransaction tipTransaction) {
					neuraliumWalletTransactionHistory.Tip = tipTransaction.Tip;
				}
			}
		}

		protected override void FillStandardAccountSnapshot(IWalletAccount account, IWalletStandardAccountSnapshot accountSnapshot) {
			base.FillStandardAccountSnapshot(account, accountSnapshot);

			// anything else?
		}

		protected override void FillJointAccountSnapshot(IWalletAccount account, IWalletJointAccountSnapshot accountSnapshot) {
			base.FillJointAccountSnapshot(account, accountSnapshot);

			// anything else?
		}

		protected override async Task PrepareAccountInfos(IAccountFileInfo accountFileInfo, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			await base.PrepareAccountInfos(accountFileInfo, lockContext).ConfigureAwait(false);

			if(accountFileInfo is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				await neuraliumAccountFileInfo.WalletTimelineFileInfo.CreateEmptyFile(lockContext).ConfigureAwait(false);
			}

		}

		protected override void CreateNewAccountInfoContents(IAccountFileInfo accountFileInfo, IWalletAccount account) {
			this.EnsureWalletIsLoaded();

			if(account is INeuraliumWalletAccount neuraliumWalletAccount) {
				base.CreateNewAccountInfoContents(accountFileInfo, account);

				if(accountFileInfo is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
					neuraliumAccountFileInfo.WalletTimelineFileInfo = this.SerialisationFal.CreateNeuraliumWalletTimelineFileInfo(neuraliumWalletAccount, this.WalletFileInfo.WalletSecurityDetails);
				}
			} else {
				throw new ArgumentException(nameof(account), $"Field should be of type {nameof(INeuraliumAccountFileInfo)}");
			}
		}

	#region external API methods

		//TODO: this can be removed when wallets have all been updated!
		public async Task<TimelineHeader> GetTimelineHeader(string accountCode, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(string.IsNullOrWhiteSpace(accountCode)) {
				accountCode = await this.GetAccountCode(lockContext).ConfigureAwait(false);
			}

			TimelineHeader timelineHeader = new TimelineHeader();
			
			if(this.WalletFileInfo.Accounts[accountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				
				var days = (await neuraliumAccountFileInfo.WalletTimelineFileInfo.RunQuery<NeuraliumWalletTimelineDay, NeuraliumWalletTimelineDay>(d => d, lockContext).ConfigureAwait(false)).Select(d => d.Timestamp.ToUniversalTime()).OrderByDescending(d => d).Select(d => DateTime.SpecifyKind(d, DateTimeKind.Local).Date).ToArray();

				if(days.Any()) {
					timelineHeader.FirstDay = TimeService.FormatDateTimeStandardLocal(days.Max(d => d));
					timelineHeader.LastDay = TimeService.FormatDateTimeStandardLocal(days.Min(d => d));

					foreach(var year in days.GroupBy(d => d.Year).OrderByDescending(y => y.Key)) {

						Dictionary<int, int[]> monthEntries = new Dictionary<int, int[]>();

						foreach(var month in year.GroupBy(d => d.Month).OrderByDescending(m => m.Key)) {

							monthEntries[month.Key] = month.Select(m => m.Day).OrderByDescending(d => d).ToArray();
						}

						timelineHeader.Days[year.Key] = monthEntries;
					}
				}
			}

			return timelineHeader;

		}

		/// <summary>
		///     Creates a timeline where the days and their entries are adapted to the timezone of the provided firstday.
		/// </summary>
		/// <param name="accountCode"></param>
		/// <param name="firstday"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns>A list of TimelineDay in descending order of days.</returns>
		public async Task<TimelineDay> GetTimelineSection(string accountCode, DateTime day, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(string.IsNullOrWhiteSpace(accountCode)) {
				accountCode = await this.GetAccountCode(lockContext).ConfigureAwait(false);
			}

			// ensure to get a UTC version
			var adjustedDay = DateTime.SpecifyKind(day, DateTimeKind.Utc).Date;

			// //Get the end of first day and the start of last day (first day is the most recent one, last day is the oldest)
			DateTime startOfDay = DateTime.SpecifyKind(adjustedDay, DateTimeKind.Local).Date;
			DateTime endOfDay = startOfDay.AddDays(1).Date.AddSeconds(-1);

			// ensure UTC
			endOfDay = endOfDay.ToUniversalTime();
			startOfDay = startOfDay.ToUniversalTime();

			var result = new TimelineDay();

			// we send the date so that the baseline midnight is seen in local time
			result.Day = TimeService.FormatDateTimeStandardLocal(DateTime.SpecifyKind(adjustedDay, DateTimeKind.Local).Date);

			if(this.WalletFileInfo.Accounts[accountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {

				// 	//Get the entries between the end of first day and the start of last day. We compare everything in UTC.
				NeuraliumWalletTimeline[] dayEntries = await neuraliumAccountFileInfo.WalletTimelineFileInfo.RunQuery<NeuraliumWalletTimeline, NeuraliumWalletTimeline>(d => d.Where(e => e.Timestamp.ToUniversalTime() >= startOfDay && e.Timestamp.ToUniversalTime() <= endOfDay).ToArray(), lockContext).ConfigureAwait(false);

				if(dayEntries.Any()) {
					result.Entries.AddRange(dayEntries.OrderByDescending(p => p.Timestamp).Select(p => new TimelineDay.TimelineEntry {
						Timestamp = TimeService.FormatDateTimeStandardLocal(p.Timestamp), SenderAccountId = p.SenderAccountId?.ToString() ?? "", RecipientAccountIds = p.RecipientAccountIds, Amount = p.Amount,
						Tips = p.Tips, Total = p.Amount + p.Tips, TimestampTotal = p.TimestampTotal, //This is a snapshot of the account at that moment in time.
						ElectedBlockId = p.BlockId, Direction = (byte) p.Direction, CreditType = (byte) p.CreditType, Confirmed = p.Confirmed, TransactionId = p.TransactionId ?? ""
					}).OrderByDescending(p => p.Timestamp));

					//We re-adjust the total of the day now that it is in local
					result.EndingTotal = result.Entries.First().TimestampTotal;
					result.EntriesTotal = dayEntries.Sum(e => e.Credit - e.Debit);
				} else {
					// ok, try to take the latest total from the last day closest to this one
					var days = (await neuraliumAccountFileInfo.WalletTimelineFileInfo.RunQuery<NeuraliumWalletTimelineDay, NeuraliumWalletTimelineDay>(d => d.Where(e => e.Timestamp.ToUniversalTime() <= endOfDay).OrderByDescending(e => e.Timestamp).Take(1).ToArray(), lockContext).ConfigureAwait(false));

					if(days.Any()) {
						result.EndingTotal = days.Single().Total;
					}
				}
			}

			return result;
		}

		/// <summary>
		///     here we apply the UBB to any applicable accounts that we have
		/// </summary>
		/// <param name="accountCode"></param>
		/// <param name="bounty"></param>
		/// <param name="blockId"></param>
		/// <param name="lockContext"></param>
		/// <returns></returns>
		public async Task ApplyUniversalBasicBounties(string accountCode, Amount bounty, BlockId blockId, LockContext lockContext) {

			IWalletAccount account = await this.GetWalletAccount(accountCode, lockContext).ConfigureAwait(false);

			if(account?.VerificationLevel == Enums.AccountVerificationTypes.KYC) {

				if(this.WalletFileInfo.Accounts[account.AccountCode] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {

					IWalletAccountSnapshot snapshot = await (neuraliumAccountFileInfo?.WalletSnapshotInfo.WalletAccountSnapshot(lockContext)).ConfigureAwait(false);

					if(snapshot == null) {
						throw new ApplicationException("Account snapshot does not exist");
					}

					if(snapshot is INeuraliumWalletAccountSnapshot neuraliumWalletAccountSnapshot) {
						neuraliumWalletAccountSnapshot.Balance += bounty;
					}

					// and now the timeline
					INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

					await neuraliumWalletTimelineFileInfo.InsertUBBTimelineEntry(bounty, lockContext).ConfigureAwait(false);

					this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());
				}
			}
		}

	#endregion

	#region wallet manager

		public override async Task<SynthesizedBlock> ConvertApiSynthesizedBlock(SynthesizedBlockAPI synthesizedBlockApi, LockContext lockContext) {
			SynthesizedBlock synthesizedBlock = await base.ConvertApiSynthesizedBlock(synthesizedBlockApi, lockContext).ConfigureAwait(false);

			AccountId accountId = synthesizedBlockApi.AccountId != null ? new AccountId(synthesizedBlockApi.AccountId) : new AccountId(synthesizedBlockApi.AccountCode);

			if(synthesizedBlockApi is NeuraliumSynthesizedBlockApi neuraliumSynthesizedBlockApi && synthesizedBlock is NeuraliumSynthesizedBlock neuraliumSynthesizedBlock) {

				foreach(NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedElectionResultAPI electionResult in neuraliumSynthesizedBlockApi.FinalElectionResults) {

					NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult = new NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult();

					neuraliumSynthesizedElectionResult.BlockId = synthesizedBlockApi.BlockId;
					neuraliumSynthesizedElectionResult.Timestamp = DateTime.ParseExact(synthesizedBlockApi.Timestamp, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToUniversalTime();

					AccountId delegateAccountId = null;

					if(!string.IsNullOrWhiteSpace(electionResult.DelegateAccountId)) {
						delegateAccountId = new AccountId(electionResult.DelegateAccountId);
					}

					neuraliumSynthesizedElectionResult.ElectedAccounts.Add(accountId, (accountId, delegateAccountId, (Enums.MiningTiers) electionResult.ElectedTier, electionResult.SelectedTransactions));
					neuraliumSynthesizedElectionResult.BlockId = synthesizedBlockApi.BlockId - electionResult.Offset;
					neuraliumSynthesizedElectionResult.ElectedGains.Add(accountId, (electionResult.BountyShare, electionResult.Tips));

					neuraliumSynthesizedBlock.FinalElectionResults.Add(neuraliumSynthesizedElectionResult);
				}
			}

			return synthesizedBlock;
		}

		protected override SynthesizedBlock CreateSynthesizedBlockFromApi(SynthesizedBlockAPI synthesizedBlockApi) {
			return new NeuraliumSynthesizedBlock();
		}

		public override SynthesizedBlockAPI DeserializeSynthesizedBlockAPI(string synthesizedBlock) {
			return JsonSerializer.Deserialize<NeuraliumSynthesizedBlockApi>(synthesizedBlock);
		}

	#endregion

	}
}