using System;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Components.Blocks;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.DataAccess.Dal;
using Neuralia.Blockchains.Tools;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {
	public interface INeuraliumWalletTimelineFileInfo : IWalletFileInfo {
		Task InsertTimelineEntry(NeuraliumWalletTimeline entry, LockContext lockContext);

		Task InsertUBBTimelineEntry(decimal amount, LockContext lockContext);
		Task ConfirmLocalTimelineEntry(TransactionId transactionId, decimal? tip, BlockId blockId, LockContext lockContext);
		Task RemoveLocalTimelineEntry(TransactionId transactionId, LockContext lockContext);
		Task<int> GetDaysCount(LockContext lockContext);
		Task<DateTime> GetFirstDay(LockContext lockContext);
		Task<decimal?> GetLastTotal(LockContext lockContext);
	}

	public class NeuraliumWalletTimelineFileInfo : WalletFileInfo, INeuraliumWalletTimelineFileInfo {

		private readonly IWalletAccount account;

		public NeuraliumWalletTimelineFileInfo(IWalletAccount account, string filename, ChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
			this.account = account;
		}

		public async Task<decimal?> GetLastTotal(LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				return await this.RunDbOperation(async (litedbDal, lc) => {
					return litedbDal.Open(db => {
						if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && litedbDal.Any<NeuraliumWalletTimelineDay>(db)) {
							int maxId = litedbDal.All<NeuraliumWalletTimelineDay>(db).Max(dayEntry => dayEntry.Id);

							NeuraliumWalletTimelineDay timelineDay = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Id == maxId, db);

							if(timelineDay != null) {
								return timelineDay.Total;
							}

						}

						return (decimal?) null;
					});
				}, handle).ConfigureAwait(false);
			}
		}

		public async Task InsertUBBTimelineEntry(decimal amount, LockContext lockContext) {
			
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				await this.RunDbOperation(async (litedbDal, lc) => {

					litedbDal.Open(db => {
						
						DateTime day = DateTimeEx.CurrentTime.ToUniversalTime().Date;

						NeuraliumWalletTimelineDay dayEntry = null;

						// first, lets enter the day if required, otherwise update it
						if(!litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) || !litedbDal.Exists<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db)) {
							dayEntry = new NeuraliumWalletTimelineDay();

							int newId = 0;

							if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && litedbDal.Any<NeuraliumWalletTimelineDay>(db)) {
								newId = litedbDal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Id);
							}

							dayEntry.Id = newId + 1;
							dayEntry.Timestamp = day;

							dayEntry.Total = amount;

							litedbDal.Insert(dayEntry, k => k.Id, db);
						} else {
							dayEntry = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);

							dayEntry.Total += amount;
							
							litedbDal.Update(dayEntry, db);
						}
						
						NeuraliumWalletTimeline entry = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.CreditType == NeuraliumWalletTimeline.CreditTypes.UBB && k.DayId == dayEntry.Id, db);

						if(entry != null) {
							entry = new NeuraliumWalletTimeline();
							entry.DayId = dayEntry.Id;
							entry.Amount = amount;
							entry.Timestamp = DateTimeEx.CurrentTime;

							litedbDal.Insert(entry, k => k.Id, db);
						} else {
							entry.Amount += amount;

							litedbDal.Update(entry);
						}
					});

				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}

		public async Task InsertTimelineEntry(NeuraliumWalletTimeline entry, LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				await this.RunDbOperation(async (litedbDal, lc) => {

					litedbDal.Open(db => {

						if(entry.CreditType == NeuraliumWalletTimeline.CreditTypes.Election && litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && ((entry.Id != 0) || litedbDal.Exists<NeuraliumWalletTimeline>(k => (k.Id == entry.Id) || (k.BlockId == entry.BlockId), db))) {
							return;
						}
						
						if(entry.CreditType != NeuraliumWalletTimeline.CreditTypes.Election && litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && ((entry.Id != 0) || litedbDal.Exists<NeuraliumWalletTimeline>(k => (k.Id == entry.Id) || (k.TransactionId == entry.TransactionId), db))) {
							return;
						}

						DateTime day = entry.Timestamp.ToUniversalTime().Date;

						NeuraliumWalletTimelineDay dayEntry = null;

						// first, lets enter the day if required, otherwise update it
						if(!litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) || !litedbDal.Exists<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db)) {
							dayEntry = new NeuraliumWalletTimelineDay();

							int newId = 0;

							if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && litedbDal.Any<NeuraliumWalletTimelineDay>(db)) {
								newId = litedbDal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Id);
							}

							dayEntry.Id = newId + 1;
							dayEntry.Timestamp = day;

							if(entry.Confirmed) {
								dayEntry.Total += (entry.Credit - entry.Debit);
							}

							litedbDal.Insert(dayEntry, k => k.Id, db);
						} else {
							dayEntry = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);
							
							if(entry.Confirmed) {
								dayEntry.Total += (entry.Credit - entry.Debit);
							}
							
							litedbDal.Update(dayEntry, db);
						}
						
						entry.DayId = dayEntry.Id;

						litedbDal.Insert(entry, k => k.Id, db);

					});

				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}

		public async Task ConfirmLocalTimelineEntry(TransactionId transactionId, decimal? tip, BlockId blockId, LockContext lockContext) {
			await this.RunDbOperation(async (litedbDal, lc) => {

				litedbDal.Open(db => {
					if(litedbDal.CollectionExists<NeuraliumWalletTimeline>(db)) {
						NeuraliumWalletTimeline entry = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString(), db);

						if(entry != null) {

							// update to the latest total
							entry.Confirmed = true;
							entry.BlockId = blockId;

							if(tip.HasValue) {
								// adjust the tip!
								entry.Debit -= entry.Tips;
								entry.Tips = tip.Value;
								entry.Debit += entry.Tips;
							}
							
							
							DateTime day = entry.Timestamp.ToUniversalTime().Date;
							var dayEntry = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);

							if(dayEntry != null) {
								dayEntry.Total += (entry.Credit - entry.Debit);

								litedbDal.Update(dayEntry, db);
							}


							litedbDal.Update(entry, db);
						}
					}
				});
			}, lockContext).ConfigureAwait(false);

			await this.Save(lockContext).ConfigureAwait(false);
		}

		public async Task RemoveLocalTimelineEntry(TransactionId transactionId, LockContext lockContext) {
			await this.RunDbOperation(async (litedbDal, lc) => {

				litedbDal.Open(db => {
					decimal total = 0;
					int dayId = 0;

					if(litedbDal.CollectionExists<NeuraliumWalletTimeline>(db)) {
						NeuraliumWalletTimeline entry = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString(), db);

						if(entry != null) {
							dayId = entry.DayId;
						}

						litedbDal.Remove<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString(), db);
					}

					// if the day has no entries, we remove it.
					if(dayId != 0) {
						var day = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.DayId == dayId, db);

						if(day == null) {
							litedbDal.Remove<NeuraliumWalletTimelineDay>(k => k.Id == dayId, db);
						}
					}
				});
			}, lockContext).ConfigureAwait(false);

			await this.Save(lockContext).ConfigureAwait(false);
		}

		public Task<int> GetDaysCount(LockContext lockContext) {
			return this.RunQueryDbOperation(async (litedbDal, lc) => {
				if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>()) {
					return litedbDal.Count<NeuraliumWalletTimelineDay>();
				}

				return 0;
			}, lockContext);
		}

		public Task<DateTime> GetFirstDay(LockContext lockContext) {
			return this.RunQueryDbOperation(async (litedbDal, lc) => {

				return litedbDal.Open(db => {
					if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>(db) && litedbDal.Any<NeuraliumWalletTimelineDay>(db)) {
						return litedbDal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Timestamp);
					}

					return DateTimeEx.MinValue;
				});

			}, lockContext);
		}

		protected override Task CreateDbFile(LiteDBDAL litedbDal, LockContext lockContext) {
			litedbDal.CreateDbFile<NeuraliumWalletTimeline, long>(i => i.Id);
			litedbDal.CreateDbFile<NeuraliumWalletTimelineDay, int>(i => i.Id);

			return Task.CompletedTask;
		}

		protected override Task PrepareEncryptionInfo(LockContext lockContext) {
			return this.CreateSecurityDetails(lockContext);
		}

		protected override async Task CreateSecurityDetails(LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				if(this.EncryptionInfo == null) {
					this.EncryptionInfo = new EncryptionInfo();

					this.EncryptionInfo.Encrypt = this.WalletSecurityDetails.EncryptWallet;

					if(this.EncryptionInfo.Encrypt) {

						this.EncryptionInfo.EncryptionParameters = this.account.KeyLogFileEncryptionParameters;
						this.EncryptionInfo.Secret = () => this.account.KeyLogFileSecret;
					}
				}
			}
		}

		//
		// public bool TransactionExists(TransactionId transactionId) {
		//
		// 	return this.RunQueryDbOperation(litedbDal => {
		//
		// 		if(!litedbDal.CollectionExists<T>()) {
		// 			return false;
		// 		}
		//
		// 		return litedbDal.Exists<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 	});
		// }
		//
		// public void RemoveTransaction(TransactionId transactionId) {
		// 	lock(this.locker) {
		// 		this.RunDbOperation(litedbDal => {
		// 			if(litedbDal.CollectionExists<T>()) {
		// 				litedbDal.Remove<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 			}
		// 		});
		//
		// 		this.Save();
		// 	}
		// }
		//
		// public void UpdateTransactionStatus(TransactionId transactionId, WalletElectionsHistory.TransactionStatuses status) {
		// 	lock(this.locker) {
		// 		this.RunDbOperation(litedbDal => {
		// 			if(litedbDal.CollectionExists<T>()) {
		//
		// 				var entry = litedbDal.GetOne<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		//
		// 				if(entry != null && entry.Local) {
		// 					entry.Status = (byte)status;
		// 					litedbDal.Update(entry);
		// 				}
		// 			}
		// 		});
		//
		// 		this.Save();
		// 	}
		// } 
		//
		// public IWalletElectionsHistory GetTransactionBase(TransactionId transactionId) {
		// 	return this.GetTransaction(transactionId);
		// }
		//
		// public T GetTransaction(TransactionId transactionId) {
		// 	lock(this.locker) {
		// 		return this.RunQueryDbOperation(litedbDal => {
		// 			if(litedbDal.CollectionExists<T>()) {
		//
		// 				return litedbDal.GetOne<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 			}
		//
		// 			return null;
		// 		});
		// 	}
		// } 
	}
}