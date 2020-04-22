using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet.Extra;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet.Extra;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.ExternalAPI.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Envelopes;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Services;
using System.Text.Json;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Identifiers;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Locking;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumWalletProvider : IWalletProvider {

		Task<TotalAPI> GetAccountBalance(bool includeReserved, LockContext lockContext);
		Task<TotalAPI> GetAccountBalance(AccountId accountId, bool includeReserved, LockContext lockContext);
		Task<TotalAPI> GetAccountBalance(Guid accountUuid, bool includeReserved, LockContext lockContext);
		Task<TimelineHeader> GetTimelineHeader(Guid accountUuid, LockContext lockContext);
		Task<List<TimelineDay>> GetTimelineSection(Guid accountUuid, DateTime firstday, LockContext lockContext, int skip = 0, int take = 1);
		
		Task ApplyUniversalBasicBounties(Guid accountUuid, Amount bounty, LockContext lockContext);

	}

	public interface INeuraliumWalletProviderInternal : INeuraliumWalletProvider, IWalletProviderInternal {
	}

	public class NeuraliumWalletProvider : WalletProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumWalletProviderInternal {

		public NeuraliumWalletProvider(INeuraliumCentralCoordinator centralCoordinator) : base(GlobalsService.TOKEN_CHAIN_NAME, centralCoordinator) {

		}

		public new INeuraliumWalletSerialisationFal SerialisationFal => (INeuraliumWalletSerialisationFal) base.SerialisationFal;

		protected override ICardUtils CardUtils => NeuraliumCardsUtils.Instance;

		public override async Task<IWalletElectionsHistory> InsertElectionsHistoryEntry(SynthesizedBlock.SynthesizedElectionResult electionResult, AccountId electedAccountId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletElectionsHistory historyEntry = await base.InsertElectionsHistoryEntry(electionResult, electedAccountId, lockContext).ConfigureAwait(false);

			// now let's add a neuralium timeline entry
			if(historyEntry is INeuraliumWalletElectionsHistory neuraliumWalletElectionsHistory && electionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult) {

				IWalletAccount account = (await WalletFileInfo.WalletBase(lockContext).ConfigureAwait(false)).Accounts.Values.SingleOrDefault(a => a.GetAccountId() == electedAccountId);

				if(account == null) {
					throw new ApplicationException("Invalid account");
				}

				if(this.WalletFileInfo.Accounts[account.AccountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
					NeuraliumWalletTimeline neuraliumWalletTimeline = new NeuraliumWalletTimeline();
					INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

					neuraliumWalletTimeline.ElectedBlockId = electionResult.BlockId;

					neuraliumWalletTimeline.Timestamp = neuraliumSynthesizedElectionResult.Timestamp.ToUniversalTime();
					neuraliumWalletTimeline.Amount = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].bountyShare;
					neuraliumWalletTimeline.Tips = neuraliumSynthesizedElectionResult.ElectedGains[electedAccountId].tips;

					neuraliumWalletTimeline.RecipientAccountIds = electedAccountId.ToString();
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Credit;
					neuraliumWalletTimeline.CreditType = NeuraliumWalletTimeline.CreditTypes.Election;

					neuraliumWalletTimeline.Total = (await GetAccountBalance(electedAccountId, false, lockContext).ConfigureAwait(false)).Total + neuraliumWalletTimeline.Amount + neuraliumWalletTimeline.Tips;

					await neuraliumWalletTimelineFileInfo.InsertTimelineEntry(neuraliumWalletTimeline, lockContext).ConfigureAwait(false);
					
					this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());
				}
			}

			return historyEntry;
		}

		public override async Task<IWalletTransactionHistoryFileInfo> UpdateLocalTransactionHistoryEntry(TransactionId transactionId, WalletTransactionHistory.TransactionStatuses status, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			IWalletTransactionHistoryFileInfo historyEntry = await base.UpdateLocalTransactionHistoryEntry(transactionId, status, lockContext).ConfigureAwait(false);

			var walletbase = await this.WalletFileInfo.WalletBase(lockContext).ConfigureAwait(false);
			IWalletAccount account = walletbase.Accounts.Values.SingleOrDefault(a => (a.GetAccountId() == transactionId.Account) || (a.PresentationTransactionId == transactionId));

			if(account == null) {
				throw new ApplicationException("Invalid account");
			}

			if(historyEntry is INeuraliumWalletTransactionHistoryFileInfo neuraliumWalletTransactionHistoryFileInfo && this.WalletFileInfo.Accounts[account.AccountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

				if(status == WalletTransactionHistory.TransactionStatuses.Confirmed) {
					// now let's add a neuralium timeline entry
					await neuraliumWalletTimelineFileInfo.ConfirmLocalTimelineEntry(transactionId, lockContext).ConfigureAwait(false);

				} else if(status == WalletTransactionHistory.TransactionStatuses.Rejected) {
                    // now let's add a neuralium timeline entry
                    await neuraliumWalletTimelineFileInfo.RemoveLocalTimelineEntry(transactionId, lockContext).ConfigureAwait(false);
				}
				
				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumNeuraliumTimelineUpdated());
			}

			return historyEntry;
		}

		public override async Task<List<IWalletTransactionHistory>> InsertTransactionHistoryEntry(ITransaction transaction, string note, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			List<IWalletTransactionHistory> historyEntries = await base.InsertTransactionHistoryEntry(transaction, note, lockContext).ConfigureAwait(false);

			foreach(var historyEntry in historyEntries) {
				if(historyEntry is INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory) {

					await InsertNeuraliumTransactionTimelineEntry(transaction, neuraliumWalletTransactionHistory, lockContext).ConfigureAwait(false);
				}
			}

			return historyEntries;
		}

		public override async Task<List<WalletTransactionHistoryHeaderAPI>> APIQueryWalletTransactionHistory(Guid accountUuid, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(!this.IsWalletLoaded) {
				throw new ApplicationException("Wallet is not loaded");
			}

			if(accountUuid == Guid.Empty) {
				accountUuid = await GetAccountUuid(lockContext).ConfigureAwait(false);
			}

			if (!await WalletFileInfo.Accounts[accountUuid].WalletTransactionHistoryInfo.CollectionExists<NeuraliumWalletTransactionHistory>(lockContext).ConfigureAwait(false))
            {
				return new List<WalletTransactionHistoryHeaderAPI>();
			}

			//TODO: merge correctly with base version of this method
			var results = await WalletFileInfo.Accounts[accountUuid].WalletTransactionHistoryInfo.RunQuery<NeuraliumWalletTransactionHistoryHeaderAPI, NeuraliumWalletTransactionHistory>(caches => caches.Select(t => {

				TransactionId transactionId = new TransactionId(t.TransactionId);
				var version = new ComponentVersion<TransactionType>(t.Version);

				return new NeuraliumWalletTransactionHistoryHeaderAPI {
					TransactionId = t.TransactionId, Sender = transactionId.Account.ToString(), Timestamp = TimeService.FormatDateTimeStandardUtc(t.Timestamp), Status = t.Status,
					Version = new VersionAPI{TransactionType = version.Type.Value.Value, Major = version.Major.Value, Minor = version.Minor.Value}, Recipient = t.Recipient, Local = t.Local, Amount = t.Amount,
					Tip = t.Tip, Note = t.Note
				};
			}).OrderByDescending(t => t.Timestamp).ToList(), lockContext).ConfigureAwait(false);

			return results.Cast<WalletTransactionHistoryHeaderAPI>().ToList();

		}

		public override async Task<WalletTransactionHistoryDetailsAPI> APIQueryWalletTransactionHistoryDetails(Guid accountUuid, string transactionId, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(!this.IsWalletLoaded) {
				throw new ApplicationException("Wallet is not loaded");
			}

			if(accountUuid == Guid.Empty) {
				accountUuid = await GetAccountUuid(lockContext).ConfigureAwait(false);
			}

			if (!await WalletFileInfo.Accounts[accountUuid].WalletTransactionHistoryInfo.CollectionExists<NeuraliumWalletTransactionHistory>(lockContext).ConfigureAwait(false))
            {
				return new WalletTransactionHistoryDetailsAPI();
			}

			
			var results = await WalletFileInfo.Accounts[accountUuid].WalletTransactionHistoryInfo.RunQuery<NeuraliumWalletTransactionHistoryDetailsAPI, NeuraliumWalletTransactionHistory>(caches => caches.Where(t => t.TransactionId == transactionId).Select(t => {

				var version = new ComponentVersion<TransactionType>(t.Version);

				return new NeuraliumWalletTransactionHistoryDetailsAPI {
					TransactionId = t.TransactionId, Sender = new TransactionId(t.TransactionId).Account.ToString(), Timestamp = TimeService.FormatDateTimeStandardUtc(t.Timestamp), Status = t.Status,
					Version = new VersionAPI{TransactionType = version.Type.Value.Value, Major = version.Major.Value, Minor = version.Minor.Value}, Recipient = t.Recipient, Contents = t.Contents, Local = t.Local,
					Amount = t.Amount, Tip = t.Tip, Note = t.Note
				};
			}).ToList(), lockContext).ConfigureAwait(false);

			return results.SingleOrDefault();

		}

		public async Task<TotalAPI> GetAccountBalance(bool includeReserved, LockContext lockContext) {
			return await GetAccountBalance((await GetActiveAccount(lockContext).ConfigureAwait(false)).AccountUuid, includeReserved, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(AccountId accountId, bool includeReserved, LockContext lockContext) {

			return await GetAccountBalance((await GetWalletAccount(accountId, lockContext).ConfigureAwait(false)).AccountUuid, includeReserved, lockContext).ConfigureAwait(false);
		}

		public async Task<TotalAPI> GetAccountBalance(Guid accountUuid, bool includeReserved, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			TotalAPI result = new TotalAPI();

			if(!this.WalletFileInfo.Accounts.ContainsKey(accountUuid)) {
				return result;
			}
			var account = await GetWalletAccount(accountUuid, lockContext).ConfigureAwait(false);

			IWalletAccountSnapshot accountBase = await this.GetStandardAccountSnapshot(account.GetAccountId(), lockContext).ConfigureAwait(false);

			if(accountBase is INeuraliumWalletAccountSnapshot walletAccountSnapshot) {
				result.Total = walletAccountSnapshot.Balance;
			}

			if(includeReserved) {
				IWalletTransactionCacheFileInfo accountCacheBase = this.WalletFileInfo.Accounts[accountUuid].WalletTransactionCacheInfo;

				if(accountCacheBase is NeuraliumWalletTransactionCacheFileInfo neuraliumWalletTransactionCacheFileInfo) {
					(decimal debit, decimal credit, decimal tip) results = await neuraliumWalletTransactionCacheFileInfo.GetTransactionAmounts(lockContext).ConfigureAwait(false);

					result.ReservedDebit = results.debit + results.tip;
					result.ReservedCredit = results.credit;
				}

				if(accountBase != null) {
					foreach(var entry in accountBase.AppliedAttributesBase) {
						result.Frozen += NeuraliumSnapshotUtilities.GetImpactAmount(entry);
					}
				}
			}

			return result;
		}

		public override async Task<Dictionary<AccountId, int>> ClearTimedOutTransactions(LockContext lockContext) {
			Dictionary<AccountId, int> totals = await base.ClearTimedOutTransactions(lockContext).ConfigureAwait(false);

			foreach(var nonZero in totals.Where(e => e.Value != 0)) {
				
				// alert of the new total, since it changed
				TotalAPI total = await GetAccountBalance(nonZero.Key, true, lockContext).ConfigureAwait(false);
				this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(nonZero.Key.SequenceId, nonZero.Key.AccountType, total));
			}

			return totals;
		}
		
		public override async Task InsertLocalTransactionCacheEntry(ITransactionEnvelope transactionEnvelope, LockContext lockContext) {
			await base.InsertLocalTransactionCacheEntry(transactionEnvelope, lockContext).ConfigureAwait(false);

			AccountId targetAccountId = transactionEnvelope.Contents.Uuid.Account;

			TotalAPI total = await GetAccountBalance(targetAccountId, true, lockContext).ConfigureAwait(false);
			this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(targetAccountId.SequenceId, targetAccountId.AccountType, total));
		}

		public override async Task UpdateLocalTransactionCacheEntry(TransactionId transactionId, WalletTransactionCache.TransactionStatuses status, long gossipMessageHash, LockContext lockContext) {
			await base.UpdateLocalTransactionCacheEntry(transactionId, status, gossipMessageHash, lockContext).ConfigureAwait(false);

			AccountId targetAccountId = transactionId.Account;

			TotalAPI total = await GetAccountBalance(targetAccountId, true, lockContext).ConfigureAwait(false);
			this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(targetAccountId.SequenceId, targetAccountId.AccountType, total));
		}

		public override async Task RemoveLocalTransactionCacheEntry(TransactionId transactionId, LockContext lockContext) {
			await base.RemoveLocalTransactionCacheEntry(transactionId, lockContext).ConfigureAwait(false);
			
			TotalAPI total = await GetAccountBalance(transactionId.Account, true, lockContext).ConfigureAwait(false);
			this.centralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(transactionId.Account.SequenceId, transactionId.Account.AccountType, total));
		}
		
		private async Task InsertNeuraliumTransactionTimelineEntry(ITransaction transaction, INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory, LockContext lockContext) {
			if((neuraliumWalletTransactionHistory.Amount == 0) && (neuraliumWalletTransactionHistory.Tip == 0)) {
				// this transaction is most probably not a token influencing transaction. let's ignore 0 values
				return;
			}

			// this is an incomming transaction, now let's add a neuralium timeline entry
			(IWalletAccount sendingAccount, var recipientAccounts) = await this.GetImpactedLocalAccounts(transaction, lockContext).ConfigureAwait(false);

			IWalletAccount account = neuraliumWalletTransactionHistory.Local ? sendingAccount : recipientAccounts.Single(e => e.GetAccountId().ToString() == neuraliumWalletTransactionHistory.Recipient);
				
			if(account == null) {
				throw new ApplicationException("Invalid account");
			}

			
			if(this.WalletFileInfo.Accounts[account.AccountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				NeuraliumWalletTimeline neuraliumWalletTimeline = new NeuraliumWalletTimeline();
				INeuraliumWalletTimelineFileInfo neuraliumWalletTimelineFileInfo = neuraliumAccountFileInfo.WalletTimelineFileInfo;

				neuraliumWalletTimeline.Timestamp = this.serviceSet.TimeService.GetTimestampDateTime(transaction.TransactionId.Timestamp.Value, this.centralCoordinator.ChainComponentProvider.ChainStateProvider.ChainInception);
				neuraliumWalletTimeline.Amount = neuraliumWalletTransactionHistory.Amount;
				neuraliumWalletTimeline.Tips = 0;

				neuraliumWalletTimeline.TransactionId = transaction.TransactionId.ToString();
				
				decimal total = 0;
				decimal? lastTotal = await neuraliumWalletTimelineFileInfo.GetLastTotal(lockContext).ConfigureAwait(false);

				if(lastTotal.HasValue) {
					total = lastTotal.Value;
				} else {
					// we dont have anything in the timeline, so lets take it from the wallet
					total = (await this.GetAccountBalance(account.GetAccountId(), false, lockContext).ConfigureAwait(false)).Total;
				}

				neuraliumWalletTimeline.SenderAccountId = transaction.TransactionId.Account;
				neuraliumWalletTimeline.RecipientAccountIds = transaction.TargetAccountsSerialized;
				
				if(neuraliumWalletTransactionHistory.Local) {

					
					
					// in most cases, transactions we make wil be debits
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Debit;
					neuraliumWalletTimeline.Total = total - neuraliumWalletTimeline.Amount;
					neuraliumWalletTimeline.Tips = neuraliumWalletTransactionHistory.Tip;

#if TESTNET || DEVNET
					if(transaction is INeuraliumRefillNeuraliumsTransaction) {
						neuraliumWalletTimeline.Total = total + neuraliumWalletTimeline.Amount;
						neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Credit;
						neuraliumWalletTimeline.CreditType = NeuraliumWalletTimeline.CreditTypes.Tranasaction;
					}
#endif

					neuraliumWalletTimeline.Total -= neuraliumWalletTimeline.Tips;
					neuraliumWalletTimeline.Confirmed = false;

				} else {
					
					neuraliumWalletTimeline.Total = total + neuraliumWalletTimeline.Amount;

					neuraliumWalletTimeline.Confirmed = true;
					neuraliumWalletTimeline.Direction = NeuraliumWalletTimeline.MoneratyTransactionTypes.Credit;
					neuraliumWalletTimeline.CreditType = NeuraliumWalletTimeline.CreditTypes.Tranasaction;
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

		protected override async Task FillWalletTransactionHistoryEntry(IWalletTransactionHistory walletAccountTransactionHistory, ITransaction transaction, bool local, string note, LockContext lockContext) {
			this.EnsureWalletIsLoaded();
			await base.FillWalletTransactionHistoryEntry(walletAccountTransactionHistory, transaction, local, note, lockContext).ConfigureAwait(false);

			if(walletAccountTransactionHistory is INeuraliumWalletTransactionHistory neuraliumWalletTransactionHistory) {

				//here we record the impact amount. + value increases our amount. - reduces
				if(transaction is INeuraliumTransferTransaction neuraliumTransferTransaction) {

					neuraliumWalletTransactionHistory.Amount = neuraliumTransferTransaction.Amount;
					neuraliumWalletTransactionHistory.MoneratyTransactionType = NeuraliumWalletTransactionHistory.MoneratyTransactionTypes.Debit;
				} else if(transaction is INeuraliumMultiTransferTransaction neuraliumMultiTransferTransaction) {

					if(walletAccountTransactionHistory.Local) {
						neuraliumWalletTransactionHistory.Amount = neuraliumMultiTransferTransaction.Total;

					} else {
						(IWalletAccount sendingAccount, var recipientAccounts) = await GetImpactedLocalAccounts(transaction, lockContext).ConfigureAwait(false);
						neuraliumWalletTransactionHistory.Amount = neuraliumMultiTransferTransaction.Recipients.SingleOrDefault(r => r.Recipient == recipientAccounts.Single().GetAccountId()).Amount;
					}

					neuraliumWalletTransactionHistory.MoneratyTransactionType = NeuraliumWalletTransactionHistory.MoneratyTransactionTypes.Debit;
					neuraliumWalletTransactionHistory.Recipient = string.Join(",", neuraliumMultiTransferTransaction.Recipients.Select(a => a.Recipient).OrderBy(a => a.ToLongRepresentation()));
				} else if(transaction is INeuraliumRefillNeuraliumsTransaction neuraliumsTransaction) {
					if(walletAccountTransactionHistory.Local) {
						neuraliumWalletTransactionHistory.Amount = 1000;
						neuraliumWalletTransactionHistory.MoneratyTransactionType = NeuraliumWalletTransactionHistory.MoneratyTransactionTypes.Credit;
					}
				}

				if(transaction is ITipTransaction tipTransaction) {
					neuraliumWalletTransactionHistory.Tip = tipTransaction.Tip;
				}
			}
		}

		protected override void FillWalletTransactionCacheEntry(IWalletTransactionCache walletAccountTransactionCache, ITransactionEnvelope transactionEnvelope, AccountId targetAccountId) {
			this.EnsureWalletIsLoaded();
			base.FillWalletTransactionCacheEntry(walletAccountTransactionCache, transactionEnvelope, targetAccountId);

			ITransaction transaction = transactionEnvelope.Contents.RehydratedTransaction;

			if(walletAccountTransactionCache is INeuraliumWalletTransactionCache neuraliumWalletTransactionCache) {

				bool ours = transaction.TransactionId.Account == targetAccountId;

				//here we record the impact amount. + value increases our amount. - reduces
				if(transaction is INeuraliumTransferTransaction neuraliumTransferTransaction) {
					neuraliumWalletTransactionCache.Amount = neuraliumTransferTransaction.Amount;
					neuraliumWalletTransactionCache.MoneratyTransactionType = NeuraliumWalletTransactionCache.MoneratyTransactionTypes.Debit;
				} else if(transaction is INeuraliumMultiTransferTransaction neuraliumMultiTransferTransaction) {

					neuraliumWalletTransactionCache.Amount = neuraliumMultiTransferTransaction.Total;
					neuraliumWalletTransactionCache.MoneratyTransactionType = NeuraliumWalletTransactionCache.MoneratyTransactionTypes.Debit;
				} else if(transaction is INeuraliumRefillNeuraliumsTransaction neuraliumsTransaction) {

					neuraliumWalletTransactionCache.Amount = 1000;
					neuraliumWalletTransactionCache.MoneratyTransactionType = NeuraliumWalletTransactionCache.MoneratyTransactionTypes.Credit;
				}

				if(transaction is ITipTransaction tipTransaction) {
					neuraliumWalletTransactionCache.Tip = tipTransaction.Tip;
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

			base.CreateNewAccountInfoContents(accountFileInfo, account);

			if(accountFileInfo is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
				neuraliumAccountFileInfo.WalletTimelineFileInfo = this.SerialisationFal.CreateNeuraliumWalletTimelineFileInfo(account, this.WalletFileInfo.WalletSecurityDetails);
			}
		}

	#region external API methods

		public async Task<TimelineHeader> GetTimelineHeader(Guid accountUuid, LockContext lockContext) {
			this.EnsureWalletIsLoaded();

			if(accountUuid == Guid.Empty) {
				accountUuid = await GetAccountUuid(lockContext).ConfigureAwait(false);
			}

			TimelineHeader timelineHeader = new TimelineHeader();

			//TODO: merge correctly with base version of this method
			if(this.WalletFileInfo.Accounts[accountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {

				timelineHeader.FirstDay = TimeService.FormatDateTimeStandardUtc(await neuraliumAccountFileInfo.WalletTimelineFileInfo.GetFirstDay(lockContext).ConfigureAwait(false));
				timelineHeader.NumberOfDays = await neuraliumAccountFileInfo.WalletTimelineFileInfo.GetDaysCount(lockContext).ConfigureAwait(false);
			}

			return timelineHeader;

		}

		/// <summary>
		/// Creates a timeline where the days and their entries are adapted to the timezone of the provided firstday.
		/// </summary>
		/// <param name="accountUuid"></param>
		/// <param name="firstday"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns>A list of TimelineDay in descending order of days.</returns>
		public async Task<List<TimelineDay>> GetTimelineSection(Guid accountUuid, DateTime firstday, LockContext lockContext, int skip = 0, int take = 1) {
			this.EnsureWalletIsLoaded();

			if (skip < 0)
				throw new ArgumentOutOfRangeException(nameof(skip), "Must be bigger than or equal to 0.");
			if (take < 1)
				throw new ArgumentOutOfRangeException(nameof(take), "Must be bigger than or equal to 1.");

			if(accountUuid == Guid.Empty) {
				accountUuid = await GetAccountUuid(lockContext).ConfigureAwait(false);
			}

			var results = new List<TimelineDay>();

			//Get the end of first day and the start of last day (first day is the most recent one, last day is the oldest)
			var endOfFirstDay = firstday.AddDays(1 - skip).Date.AddSeconds(-1);
			var startOfLastDay = endOfFirstDay.AddDays(-take + 1).Date;

			//Convert to UTC
			endOfFirstDay = endOfFirstDay.ToUniversalTime();
			startOfLastDay = startOfLastDay.ToUniversalTime();

			if(this.WalletFileInfo.Accounts[accountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {

				//Get the entries between the end of first day and the start of last day. We compare everything in UTC.
				var dayEntries = await neuraliumAccountFileInfo.WalletTimelineFileInfo.RunQuery<NeuraliumWalletTimeline, NeuraliumWalletTimeline>(d => d.Where(e => endOfFirstDay >= e.Timestamp.ToUniversalTime()
				                                                                                                                                                    && e.Timestamp.ToUniversalTime() >= startOfLastDay).ToList(), lockContext).ConfigureAwait(false);

				//Because the entries are stored as a UTC day, an entry might not belong in the local day. This is why we compare the local datetimes.
				//We then create the TimelineDay required depending on our entries.
				int id = 0;
				var groupedEntries = dayEntries.GroupBy(e => e.Timestamp.ToLocalTime().Date).OrderByDescending(p => p.Key);
				foreach (var group in groupedEntries)
				{
					if (results.All(p => DateTime.ParseExact((string) p.Day, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime() != @group.Key))
						results.Add(new TimelineDay()
						{
							Day = TimeService.FormatDateTimeStandardUtc(group.Key),
							EndingTotal =  group.OrderByDescending(p => p.Timestamp)
												.First().Total,
							Id = ++id
						});
				}

				//We add the entries in their respective TimelineDay. We remove any TimelineDay that has no entry.
				foreach (TimelineDay day in results.ToList()) {
					var entries = groupedEntries.Where(e =>
						e.Key == DateTime.ParseExact(day.Day, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).ToLocalTime());

					if (!entries.Any())
						results.Remove(day);
					else
						day.Entries.AddRange(entries.OrderByDescending(p => p.Key).SelectMany(p => p).Select(p => new TimelineDay.TimelineEntry
						{
							Timestamp = TimeService.FormatDateTimeStandardUtc(p.Timestamp),
							SenderAccountId = p.SenderAccountId?.ToString() ?? "",
							RecipientAccountIds = p.RecipientAccountIds,
							Amount = p.Amount,
							Tips = p.Tips,
							Total = p.Total,
							ElectedBlockId = p.ElectedBlockId,
							Direction = (byte)p.Direction,
							CreditType = (byte)p.CreditType,
							Confirmed = p.Confirmed,
							DayId = p.DayId,
							TransactionId = p.TransactionId ?? ""
						}).OrderByDescending(p => p.Timestamp));
				}

				//Sort the Timeline in descending order of days.
				results.Sort((x, y) =>
						DateTime.ParseExact(y.Day, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
								.CompareTo(
									DateTime.ParseExact(x.Day, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal)
									)
						);
			}

			return results;

		}
		
		/// <summary>
		/// here we apply the UBB to any applicable accounts that we have
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="bounty"></param>
		/// <returns></returns>
		public async Task ApplyUniversalBasicBounties(Guid accountUuid, Amount bounty, LockContext lockContext){

			var account = await GetWalletAccount(accountUuid, lockContext).ConfigureAwait(false);
			if (account?.Correlated??false){
					
				if(this.WalletFileInfo.Accounts[account.AccountUuid] is INeuraliumAccountFileInfo neuraliumAccountFileInfo) {
					
					var snapshot = await (neuraliumAccountFileInfo?.WalletSnapshotInfo.WalletAccountSnapshot(lockContext)).ConfigureAwait(false);
					
					if(snapshot == null) {
						throw new ApplicationException("Account snapshot does not exist");
					}

					if(snapshot is INeuraliumWalletAccountSnapshot neuraliumWalletAccountSnapshot) {
						neuraliumWalletAccountSnapshot.Balance += bounty;
					}
				}
			}
		}

		#endregion

	#region wallet manager

		public override async Task<SynthesizedBlock> ConvertApiSynthesizedBlock(SynthesizedBlockAPI synthesizedBlockApi, LockContext lockContext) {
			SynthesizedBlock synthesizedBlock = await base.ConvertApiSynthesizedBlock(synthesizedBlockApi, lockContext).ConfigureAwait(false);

			AccountId accountId = synthesizedBlockApi.AccountId != null ? new AccountId(synthesizedBlockApi.AccountId) : new AccountId(synthesizedBlockApi.AccountHash);

			if(synthesizedBlockApi is NeuraliumSynthesizedBlockApi neuraliumSynthesizedBlockApi && synthesizedBlock is NeuraliumSynthesizedBlock neuraliumSynthesizedBlock) {

				foreach(NeuraliumSynthesizedBlockApi.NeuraliumSynthesizedElectionResultAPI electionResult in neuraliumSynthesizedBlockApi.FinalElectionResults) {

					NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult = new NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult();

					neuraliumSynthesizedElectionResult.BlockId = synthesizedBlockApi.BlockId;
					neuraliumSynthesizedElectionResult.Timestamp = DateTime.ParseExact(synthesizedBlockApi.Timestamp, "o", CultureInfo.InvariantCulture,  DateTimeStyles.AdjustToUniversal).ToUniversalTime();

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