using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.Configuration;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Core.DataAccess.Dal;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {

	public interface INeuraliumWalletTransactionHistoryFileInfo : IWalletTransactionHistoryFileInfo {
		Task UpdateTransactionTip(TransactionId transactionId, decimal tip, LockContext lockContext);

	}

	public class NeuraliumWalletTransactionHistoryFileInfo : WalletTransactionHistoryFileInfo<NeuraliumWalletTransactionHistory>, INeuraliumWalletTransactionHistoryFileInfo {

		public NeuraliumWalletTransactionHistoryFileInfo(IWalletAccount account, string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(account, filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}

		public async Task UpdateTransactionTip(TransactionId transactionId, decimal tip, LockContext lockContext) {
			using(LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false)) {
				await this.RunDbOperation((dbdal, lc) => {
					if(dbdal.CollectionExists<NeuraliumWalletTransactionHistory>()) {

						NeuraliumWalletTransactionHistory entry = dbdal.GetOne<NeuraliumWalletTransactionHistory>(k => k.TransactionId == transactionId.ToString());

						if((entry != null) && entry.Local && entry.Tip != tip) {
							entry.Tip = tip;
							dbdal.Update(entry);
						}
					}

					return Task.CompletedTask;
				}, handle).ConfigureAwait(false);

				await this.Save(handle).ConfigureAwait(false);
			}
		}
		
		/// <summary>
		///     take the sum of all amounts and tips currently locked in unconfirmed transactions in our cache
		/// </summary>
		/// <returns></returns>
		public async Task<(decimal debit, decimal credit, decimal tip)> GetPendingTransactionAmounts(LockContext lockContext) {
			using LockHandle handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false);

			(List<(decimal amount, decimal tip)> debits, List<(decimal amount, decimal tip)> credits) = await this.RunQueryDbOperation(async (dbdal, lc) => {

				if(dbdal.CollectionExists<NeuraliumWalletTransactionHistory>()) {
					var pendingtransactions = dbdal.Get<NeuraliumWalletTransactionHistory, (decimal amount, decimal tip, Enums.BookkeepingTypes bookkeepingType)>(t => t.Status == WalletTransactionHistory.TransactionStatuses.Dispatched && t.Local, t => (t.Amount, t.Tip, t.BookkeepingType));

					var dbdebits = pendingtransactions.Where(t => t.bookkeepingType == Enums.BookkeepingTypes.Debit).Select(t => (t.amount, t.tip)).ToList();
					var dbcredits = pendingtransactions.Where(t => t.bookkeepingType == Enums.BookkeepingTypes.Credit).Select(t => (t.amount, t.tip)).ToList();

					return (dbdebits, dbcredits);
				}

				return default;
			}, handle).ConfigureAwait(false);
			
			decimal debit = 0;
			decimal credit = 0;
			decimal tip = 0;

			if(debits?.Any() ?? false) {

				debit = debits.Sum(e => e.Item1);
				tip = debits.Sum(e => e.Item2);
			}

			if(credits?.Any() ?? false) {

				credit = credits.Sum(e => e.Item1);
				tip += credits.Sum(e => e.Item2);
			}

			return (debit, credit, tip);
		}

		protected override async Task CreateDbFile(IWalletDBDAL dbdal, LockContext lockContext) {
			await base.CreateDbFile(dbdal, lockContext).ConfigureAwait(false);
			
			// ensure extra indices
			dbdal.Open(db => {
				var collection = db.GetCollection<NeuraliumWalletTransactionHistory>();
				collection.EnsureIndex(x => x.Timestamp);
			});
		}
	}
}