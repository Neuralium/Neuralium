using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Components.Blocks;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumUtilityWalletProvider : IUtilityWalletProvider {
	}

	public interface INeuraliumReadonlyWalletProvider : IReadonlyWalletProvider {
	}

	public interface INeuraliumWalletProviderWrite : IWalletProviderWrite {
	}

	public interface INeuraliumWalletProviderProxy : IWalletProviderProxy, INeuraliumUtilityWalletProvider, INeuraliumReadonlyWalletProvider, INeuraliumWalletProviderWrite, INeuraliumWalletProvider {
	}

	public class NeuraliumWalletProviderProxy : WalletProviderProxy<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumWalletProviderProxy {
		public NeuraliumWalletProviderProxy(INeuraliumCentralCoordinator centralCoordinator, INeuraliumWalletProvider walletProvider) : base(centralCoordinator, walletProvider) {

		}

		private INeuraliumWalletProvider WalletProvider => (INeuraliumWalletProvider) this.walletProvider;

		public Task<TotalAPI> GetAccountBalance(bool includeReserved, LockContext lockContext) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetAccountBalance(includeReserved, lc), lockContext);
		}

		public Task<TotalAPI> GetAccountBalance(string accountCode, bool includeReserved, LockContext lockContext) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetAccountBalance(accountCode, includeReserved, lc), lockContext);
		}

		public Task<TotalAPI> GetAccountBalance(AccountId accountId, bool includeReserved, LockContext lockContext) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetAccountBalance(accountId, includeReserved, lc), lockContext);
		}

		public Task<decimal> GetUsableAccountBalance(string accountCode, LockContext lockContext) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetUsableAccountBalance(accountCode, lc), lockContext);
		}
		
		public Task<TimelineHeader> GetTimelineHeader(string accountCode, LockContext lockContext) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetTimelineHeader(accountCode, lc), lockContext);
		}

		public Task<List<TimelineDay>> GetTimelineSection(string accountCode, DateTime firstday, LockContext lockContext, int skip = 0, int take = 1) {
			return this.ScheduleKeyedRead((prov, lc) => ((INeuraliumWalletProvider) prov).GetTimelineSection(accountCode, firstday, lc, skip, take), lockContext);
		}

		public Task ApplyUniversalBasicBounties(string accountCode, Amount bounty, BlockId blockId, LockContext lockContext) {
			return this.ScheduleTransaction((t, ct, lc) => {
				return ((INeuraliumWalletProvider) this.walletProvider).ApplyUniversalBasicBounties(accountCode, bounty, blockId, lc);

			}, lockContext, 20, lc => {
				// load wallet & key
				this.walletProvider.EnsureWalletIsLoaded();

				return Task.CompletedTask;
			});
		}
	}
}