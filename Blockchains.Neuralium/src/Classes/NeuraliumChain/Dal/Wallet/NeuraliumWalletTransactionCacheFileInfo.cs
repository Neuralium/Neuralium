using System;
using System.Linq;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes.Configuration;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Cryptography.Passphrases;
using Neuralia.Blockchains.Tools.Locking;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Wallet {

	public class NeuraliumWalletTransactionCacheFileInfo : WalletTransactionCacheFileInfo<NeuraliumWalletTransactionCache> {

		public NeuraliumWalletTransactionCacheFileInfo(IWalletAccount account, string filename, NeuraliumBlockChainConfigurations chainConfiguration, BlockchainServiceSet serviceSet, IWalletSerialisationFal serialisationFal, WalletPassphraseDetails walletSecurityDetails) : base(account, filename, chainConfiguration, serviceSet, serialisationFal, walletSecurityDetails) {
		}

		/// <summary>
		///     take the sum of all amounts dn tips currently locked in unconfirmed transactions in our cache
		/// </summary>
		/// <returns></returns>
		public async Task<(decimal debit, decimal credit, decimal tip)> GetTransactionAmounts(LockContext lockContext) {
			using var handle = await this.locker.LockAsync(lockContext).ConfigureAwait(false);
				bool collectionExists = false;

				var debits = await RunQueryDbOperation(async (litedbDal, lc) => {
					collectionExists = litedbDal.CollectionExists<NeuraliumWalletTransactionCache>();

					if(collectionExists) {
						return litedbDal.Get<NeuraliumWalletTransactionCache, Tuple<decimal, decimal>>(t => t.MoneratyTransactionType == NeuraliumWalletTransactionCache.MoneratyTransactionTypes.Debit, t => new Tuple<decimal, decimal>(t.Amount, t.Tip));
					}

					return default;
				}, handle).ConfigureAwait(false);

				var credits = await RunQueryDbOperation(async (litedbDal, lc) => {
					if(collectionExists) {
						return litedbDal.Get<NeuraliumWalletTransactionCache, Tuple<decimal, decimal>>(t => t.MoneratyTransactionType == NeuraliumWalletTransactionCache.MoneratyTransactionTypes.Credit, t => new Tuple<decimal, decimal>(t.Amount, t.Tip));
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
		}
}