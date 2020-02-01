using System;
using System.Collections.Generic;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Core.General.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumUtilityWalletProvider : IUtilityWalletProvider {
	}

	public interface INeuraliumReadonlyWalletProvider : IReadonlyWalletProvider {
		TotalAPI GetAccountBalance(bool includeReserved);
		TotalAPI GetAccountBalance(Guid accountUuid, bool includeReserved);
		TotalAPI GetAccountBalance(AccountId accountId, bool includeReserved);

		TimelineHeader GetTimelineHeader(Guid accountUuid);
		List<TimelineDay> GetTimelineSection(Guid accountUuid, DateTime firstday, int skip = 0, int take = 1);
	}

	public interface INeuraliumWalletProviderWrite : IWalletProviderWrite {
	}

	public interface INeuraliumWalletProviderProxy : IWalletProviderProxy, INeuraliumUtilityWalletProvider, INeuraliumReadonlyWalletProvider, INeuraliumWalletProviderWrite {
	}

	public class NeuraliumWalletProviderProxy : WalletProviderProxy<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumWalletProviderProxy {
		public NeuraliumWalletProviderProxy(INeuraliumCentralCoordinator centralCoordinator, INeuraliumWalletProvider walletProvider) : base(centralCoordinator, walletProvider) {

		}

		private INeuraliumWalletProvider WalletProvider => (INeuraliumWalletProvider) this.walletProvider;

		public TotalAPI GetAccountBalance(bool includeReserved) {
			return this.ScheduleKeyedRead(prov => this.WalletProvider.GetAccountBalance(includeReserved));
		}

		public TotalAPI GetAccountBalance(Guid accountUuid, bool includeReserved) {
			return this.ScheduleKeyedRead(prov => this.WalletProvider.GetAccountBalance(accountUuid, includeReserved));
		}

		public TotalAPI GetAccountBalance(AccountId accountId, bool includeReserved) {
			return this.ScheduleKeyedRead(prov => this.WalletProvider.GetAccountBalance(accountId, includeReserved));
		}

		public TimelineHeader GetTimelineHeader(Guid accountUuid) {
			return this.ScheduleKeyedRead(prov => this.WalletProvider.GetTimelineHeader(accountUuid));
		}

		public List<TimelineDay> GetTimelineSection(Guid accountUuid, DateTime firstday, int skip = 0, int take = 1) {
			return this.ScheduleKeyedRead(prov => this.WalletProvider.GetTimelineSection(accountUuid, firstday, skip, take));
		}

		public void PerformWalletTransaction(Action transactionAction) {
			throw new NotImplementedException();
		}
	}
}