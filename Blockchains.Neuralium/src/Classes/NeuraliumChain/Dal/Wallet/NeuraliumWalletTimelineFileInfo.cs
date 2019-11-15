using System;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.DataAccess.Dal;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {
	public interface INeuraliumWalletTimelineFileInfo : IWalletFileInfo {
		void InsertTimelineEntry(NeuraliumWalletTimeline entry);
		void ConfirmLocalTimelineEntry(TransactionId transactionId);
		void RemoveLocalTimelineEntry(TransactionId transactionId);
		int GetDaysCount();
		DateTime GetFirstDay();
		decimal? GetLastTotal();
	}

	public class NeuraliumWalletTimelineFileInfo : WalletFileInfo, INeuraliumWalletTimelineFileInfo {

		private readonly IWalletAccount account;

		public NeuraliumWalletTimelineFileInfo(IWalletAccount account, string filename, ChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
			this.account = account;
		}

		public decimal? GetLastTotal() {
			lock(this.locker) {
				return this.RunDbOperation(litedbDal => {
					
					if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>() && litedbDal.Any<NeuraliumWalletTimelineDay>()) {
						int maxId = litedbDal.All<NeuraliumWalletTimelineDay>().Max(dayEntry => dayEntry.Id);

						var timelineDay = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Id == maxId);

						if(timelineDay != null) {
							return timelineDay.Total;
						}

					}

					return (decimal?)null;
				});
			}
		}
		
		public void InsertTimelineEntry(NeuraliumWalletTimeline entry) {
			lock(this.locker) {
				this.RunDbOperation(litedbDal => {
					
					if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>() && (entry.Id != 0) && litedbDal.Exists<NeuraliumWalletTimeline>(k => k.Id == entry.Id || k.TransactionId == entry.TransactionId)) {
						return;
					}

					DateTime day = entry.Timestamp.ToUniversalTime().Date;

					NeuraliumWalletTimelineDay dayEntry = null;

					// first, lets enter the day if required, otherwise update it
					if(!litedbDal.CollectionExists<NeuraliumWalletTimelineDay>() || !litedbDal.Exists<NeuraliumWalletTimelineDay>(k => k.Timestamp == day)) {
						dayEntry = new NeuraliumWalletTimelineDay();

						int newId = 0;

						if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>() && litedbDal.Any<NeuraliumWalletTimelineDay>()) {
							newId = litedbDal.All<NeuraliumWalletTimelineDay>().Max(d => d.Id);
						}

						dayEntry.Id = newId + 1;
						dayEntry.Timestamp = day;
						dayEntry.Total = entry.Total;

						litedbDal.Insert(dayEntry, k => k.Id);
					}

					dayEntry = litedbDal.GetOne<NeuraliumWalletTimelineDay>(k => k.Timestamp == day);

					// update to the latest total
					dayEntry.Total = entry.Total;

					litedbDal.Update(dayEntry);

					entry.DayId = dayEntry.Id;

					litedbDal.Insert(entry, k => k.Id);
				});

				this.Save();
			}
		}

		public void ConfirmLocalTimelineEntry(TransactionId transactionId) {
			this.RunDbOperation(litedbDal => {

				if(litedbDal.CollectionExists<NeuraliumWalletTimeline>()) {
					NeuraliumWalletTimeline entry = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString());

					if(entry != null) {
						// update to the latest total
						entry.Confirmed = true;

						litedbDal.Update(entry);
					}
				}
			});

			this.Save();
		}

		public void RemoveLocalTimelineEntry(TransactionId transactionId) {
			this.RunDbOperation(litedbDal => {

				int dayId = 0;

				if(litedbDal.CollectionExists<NeuraliumWalletTimeline>()) {
					NeuraliumWalletTimeline entry = litedbDal.GetOne<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString());

					if(entry != null) {
						dayId = entry.DayId;
					}

					litedbDal.Remove<NeuraliumWalletTimeline>(k => k.TransactionId == transactionId.ToString());
				}

				// if the day has no entries, we remove it.
				if(dayId != 0) {
					if(litedbDal.Any<NeuraliumWalletTimeline>(k => k.DayId == dayId) == false) {
						litedbDal.Remove<NeuraliumWalletTimelineDay>(k => k.Id == dayId);
					}
				}
			});

			this.Save();
		}

		public int GetDaysCount() {
			return this.RunQueryDbOperation(litedbDal => {
				if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>()) {
					return litedbDal.Count<NeuraliumWalletTimelineDay>();
				}

				return 0;
			});
		}

		public DateTime GetFirstDay() {
			return this.RunQueryDbOperation(litedbDal => {
				if(litedbDal.CollectionExists<NeuraliumWalletTimelineDay>()) {
					return litedbDal.All<NeuraliumWalletTimelineDay>().Max(d => d.Timestamp);
				}

				return DateTime.MinValue;
			});
		}

		protected override void CreateDbFile(LiteDBDAL litedbDal) {
			litedbDal.CreateDbFile<NeuraliumWalletTimeline, long>(i => i.Id);
			litedbDal.CreateDbFile<NeuraliumWalletTimelineDay, int>(i => i.Id);
		}

		protected override void PrepareEncryptionInfo() {
			this.CreateSecurityDetails();
		}

		protected override void CreateSecurityDetails() {
			lock(this.locker) {
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

		protected override void UpdateDbEntry() {
			// do nothing, we dont udpate

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