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
		Task UpgradeTimelineEntries(LockContext lockContext);

		Task InsertUBBTimelineEntry(decimal amount, LockContext lockContext);
		Task ConfirmLocalTimelineEntry(TransactionId transactionId, decimal? tip, BlockId blockId, LockContext lockContext);
		Task RemoveLocalTimelineEntry(TransactionId transactionId, LockContext lockContext);
		Task<int> GetDaysCount(LockContext lockContext);
		Task<DateTime> GetFirstDay(LockContext lockContext);
		Task<DateTime> GetLastDay(LockContext lockContext);
		Task<decimal?> GetLastTotal(LockContext lockContext);
	}

	public class NeuraliumWalletTimelineFileInfo : WalletFileInfo, INeuraliumWalletTimelineFileInfo {

		private readonly INeuraliumWalletAccount account;

		public NeuraliumWalletTimelineFileInfo(INeuraliumWalletAccount account, string filename, ChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
			this.account = account;
		}

		public async Task<decimal?> GetLastTotal(LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				return await this.RunDbOperation(async (dbdal, lc) => {
					return dbdal.Open(db => {
						if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && dbdal.Any<NeuraliumWalletTimelineDay>(db)) {
							int maxId = dbdal.All<NeuraliumWalletTimelineDay>(db).Max(dayEntry => dayEntry.Id);

							NeuraliumWalletTimelineDay timelineDay = dbdal.GetOne<NeuraliumWalletTimelineDay>(k => k.Id == maxId, db);

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
				await this.RunDbOperation(async (dbdal, lc) => {

					dbdal.Open(db => {
						
						DateTime day = DateTimeEx.CurrentTime.ToUniversalTime().Date;

						NeuraliumWalletTimelineDay dayEntry = null;

						// first, lets enter the day if required, otherwise update it
						if(!dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) || !dbdal.Exists<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db)) {
							dayEntry = new NeuraliumWalletTimelineDay();

							int newId = 0;

							if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && dbdal.Any<NeuraliumWalletTimelineDay>(db)) {
								newId = dbdal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Id);
							}

							dayEntry.Id = newId + 1;
							dayEntry.Timestamp = day;

							dayEntry.Total = amount;

							dbdal.Insert(dayEntry, k => k.Id, db);
						} else {
							dayEntry = dbdal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);

							dayEntry.Total += amount;
							
							dbdal.Update(dayEntry, db);
						}
						
						NeuraliumWalletTimeline entry = dbdal.GetOne<NeuraliumWalletTimeline>(k => k.CreditType == NeuraliumWalletTimeline.CreditTypes.UBB && k.DayId == dayEntry.Id, db);

						if(entry == null) {
							entry = new NeuraliumWalletTimeline();
							entry.DayId = dayEntry.Id;
							entry.Amount = amount;
							entry.Credit = amount;
							entry.CreditType = NeuraliumWalletTimeline.CreditTypes.UBB;
							entry.Timestamp = DateTimeEx.CurrentTime;
							entry.Confirmed = true;
							entry.TimestampTotal = dayEntry.Total;

							dbdal.Insert(entry, k => k.Id, db);
						} else {
							entry.Amount += amount;
							entry.Credit += amount;
							entry.TimestampTotal = dayEntry.Total;

							dbdal.Update(entry);
						}
					});

				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Special method to upgrade faulty wallets. 
		/// </summary>
		/// <remarks>this can be removed i the future when all wallets have been updated</remarks>
		/// <param name="lockContext"></param>
		/// <returns></returns>
		public async Task UpgradeTimelineEntries(LockContext lockContext)
		{
			//TODO: this could be removed in the future,
			// BUT users might still restore old-wallet from old-backup and thus need this.
			using (LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false))
			{
				await this.RunDbOperation(async (dbdal, lc) => {

					dbdal.Open(db => {

						if (dbdal.CollectionExists<NeuraliumWalletTimeline>(db))
						{
							var faulties = dbdal.Get<NeuraliumWalletTimeline>(e => e.CreditType == NeuraliumWalletTimeline.CreditTypes.Tranasaction).ToList();

							foreach (var entry in faulties)
							{

								entry.CreditType = NeuraliumWalletTimeline.CreditTypes.Transaction;
								dbdal.Update(entry, db);
							}
						}
					});

				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}

		public async Task InsertTimelineEntry(NeuraliumWalletTimeline entry, LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				await this.RunDbOperation(async (dbdal, lc) => {

					dbdal.Open(db => {

						if(entry.CreditType == NeuraliumWalletTimeline.CreditTypes.Election && dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && ((entry.Id != 0) || dbdal.Exists<NeuraliumWalletTimeline>(k => (k.Id == entry.Id) || (k.BlockId == entry.BlockId), db))) {
							return;
						}
						
						if(entry.CreditType != NeuraliumWalletTimeline.CreditTypes.Election && dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && ((entry.Id != 0) || dbdal.Exists<NeuraliumWalletTimeline>(k => (k.Id == entry.Id) || (k.TransactionId == entry.TransactionId), db))) {
							return;
						}

						DateTime day = entry.Timestamp.ToUniversalTime().Date;

						NeuraliumWalletTimelineDay dayEntry = null;

						// first, lets enter the day if required, otherwise update it
						if(!dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) || !dbdal.Exists<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db)) {
							dayEntry = new NeuraliumWalletTimelineDay();

							int newId = 0;

							if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && dbdal.Any<NeuraliumWalletTimelineDay>(db)) {
								newId = dbdal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Id);
							}

							dayEntry.Id = newId + 1;
							dayEntry.Timestamp = day;

							if(entry.Confirmed) {
								dayEntry.Total += (entry.Credit - entry.Debit);
							}

							dbdal.Insert(dayEntry, k => k.Id, db);
						} else {
							dayEntry = dbdal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);
							
							if(entry.Confirmed) {
								dayEntry.Total += (entry.Credit - entry.Debit);
							}
							
							dbdal.Update(dayEntry, db);
						}
						
						entry.DayId = dayEntry.Id;

						dbdal.Insert(entry, k => k.Id, db);

					});

				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}

		public async Task ConfirmLocalTimelineEntry(TransactionId transactionId, decimal? tip, BlockId blockId, LockContext lockContext) {
			await this.RunDbOperation(async (dbdal, lc) => {

				dbdal.Open(db => {
					if(dbdal.CollectionExists<NeuraliumWalletTimeline>(db)) {
						NeuraliumWalletTimeline entry = dbdal.GetOne<NeuraliumWalletTimeline>(k => k.Confirmed == false && k.TransactionId == transactionId.ToString(), db);

						if(entry != null && !entry.Confirmed) {

							// update to the latest total
							entry.Confirmed = true;
							entry.BlockId = blockId;

							if(tip.HasValue) {
								// adjust the tip!
								
								// undo the previous change
								entry.TimestampTotal += (entry.Debit - entry.Credit);
								
								entry.Debit -= entry.Tips;
								entry.Tips = tip.Value;
								entry.Debit += entry.Tips;
								
								entry.TimestampTotal += (entry.Credit - entry.Debit);
							}

							DateTime day = entry.Timestamp.ToUniversalTime().Date;
							var dayEntry = dbdal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day, db);

							if(dayEntry != null) {
								dayEntry.Total += (entry.Credit - entry.Debit);

								dbdal.Update(dayEntry, db);
							}


							dbdal.Update(entry, db);
						}
					}
				});
			}, lockContext).ConfigureAwait(false);

			await this.Save(lockContext).ConfigureAwait(false);
		}

		public async Task RemoveLocalTimelineEntry(TransactionId transactionId, LockContext lockContext) {
			await this.RunDbOperation(async (dbdal, lc) => {

				dbdal.Open(db => {
					decimal total = 0;
					int dayId = 0;

					if(dbdal.CollectionExists<NeuraliumWalletTimeline>(db)) {
						NeuraliumWalletTimeline entry = dbdal.GetOne<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString(), db);

						if(entry != null) {
							dayId = entry.DayId;
						}

						dbdal.Remove<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString(), db);
					}

					// if the day has no entries, we remove it.
					if(dayId != 0) {
						var day = dbdal.GetOne<NeuraliumWalletTimeline>(k => k.DayId == dayId, db);

						if(day == null) {
							dbdal.Remove<NeuraliumWalletTimelineDay>(k => k.Id == dayId, db);
						}
					}
				});
			}, lockContext).ConfigureAwait(false);

			await this.Save(lockContext).ConfigureAwait(false);
		}

		public Task<int> GetDaysCount(LockContext lockContext) {
			return this.RunQueryDbOperation(async (dbdal, lc) => {
				if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>()) {
					return dbdal.Count<NeuraliumWalletTimelineDay>();
				}

				return 0;
			}, lockContext);
		}

		public Task<DateTime> GetFirstDay(LockContext lockContext) {
			return this.RunQueryDbOperation(async (dbdal, lc) => {

				return dbdal.Open(db => {
					if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && dbdal.Any<NeuraliumWalletTimelineDay>(db)) {
						return dbdal.All<NeuraliumWalletTimelineDay>(db).Max(d => d.Timestamp);
					}

					return DateTimeEx.MinValue;
				});

			}, lockContext);
		}
		
		public Task<DateTime> GetLastDay(LockContext lockContext) {
			return this.RunQueryDbOperation(async (dbdal, lc) => {

				return dbdal.Open(db => {
					if(dbdal.CollectionExists<NeuraliumWalletTimelineDay>(db) && dbdal.Any<NeuraliumWalletTimelineDay>(db)) {
						return dbdal.All<NeuraliumWalletTimelineDay>(db).Min(d => d.Timestamp);
					}

					return DateTimeEx.MinValue;
				});

			}, lockContext);
		}

		protected override async Task CreateDbFile(IWalletDBDAL dbdal, LockContext lockContext) {
			dbdal.CreateDbFile<NeuraliumWalletTimeline, long>(i => i.Id);
			dbdal.CreateDbFile<NeuraliumWalletTimelineDay, int>(i => i.Id);

			// ensure extra indices
			dbdal.Open(db => {
				var collection = db.GetCollection<NeuraliumWalletTimeline>();
				collection.EnsureIndex(x => x.Timestamp);
				
				var collection2 = db.GetCollection<NeuraliumWalletTimelineDay>();
				collection2.EnsureIndex(x => x.Timestamp);
			});
			
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

						this.EncryptionInfo.EncryptionParameters = this.account.NeuraliumTimelineFileEncryptionParameters;
						this.EncryptionInfo.SecretHandler = () => this.account.KeyLogFileSecret;
					}
				}
			}
		}

		//
		// public bool TransactionExists(TransactionId transactionId) {
		//
		// 	return this.RunQueryDbOperation(dbdal => {
		//
		// 		if(!dbdal.CollectionExists<T>()) {
		// 			return false;
		// 		}
		//
		// 		return dbdal.Exists<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 	});
		// }
		//
		// public void RemoveTransaction(TransactionId transactionId) {
		// 	lock(this.locker) {
		// 		this.RunDbOperation(dbdal => {
		// 			if(dbdal.CollectionExists<T>()) {
		// 				dbdal.Remove<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 			}
		// 		});
		//
		// 		this.Save();
		// 	}
		// }
		//
		// public void UpdateTransactionStatus(TransactionId transactionId, WalletElectionsHistory.TransactionStatuses status) {
		// 	lock(this.locker) {
		// 		this.RunDbOperation(dbdal => {
		// 			if(dbdal.CollectionExists<T>()) {
		//
		// 				var entry = dbdal.GetOne<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		//
		// 				if(entry != null && entry.Local) {
		// 					entry.Status = (byte)status;
		// 					dbdal.Update(entry);
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
		// 		return this.RunQueryDbOperation(dbdal => {
		// 			if(dbdal.CollectionExists<T>()) {
		//
		// 				return dbdal.GetOne<T>(k => k.TransactionId.Equals(transactionId.ToString()));
		// 			}
		//
		// 			return null;
		// 		});
		// 	}
		// } 
	}
}