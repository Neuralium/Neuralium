using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {

	public interface INeuraliumWalletElectionsHistory : IWalletElectionsHistory {

		[BsonId]
		new long BlockId { get; set; }

		decimal Bounty { get; set; }

		decimal Tips { get; set; }
	}

	public class NeuraliumWalletElectionsHistory : WalletElectionsHistory, INeuraliumWalletElectionsHistory {

		[BsonId]
		public new long BlockId { get; set; }

		public decimal Bounty { get; set; }
		public decimal Tips { get; set; }
	}
	public interface INeuraliumWalletElectionsMiningSessionStatistics : IWalletElectionsMiningSessionStatistics {

		decimal TotalBounty { get; set; }
		decimal TotalTips { get; set; }
	}
	
	public class NeuraliumWalletElectionsMiningSessionStatistics : WalletElectionsMiningSessionStatistics, INeuraliumWalletElectionsMiningSessionStatistics {

		static NeuraliumWalletElectionsMiningSessionStatistics() {
			BsonMapper.Global.Entity<NeuraliumWalletElectionsMiningSessionStatistics>().Id(x => x.Id);
		}
		
		public decimal TotalBounty { get; set; }
		public decimal TotalTips { get; set; }
	}
	
	
	public interface INeuraliumWalletElectionsMiningAggregateStatistics : IWalletElectionsMiningAggregateStatistics {

		decimal TotalBounty { get; set; }
		decimal TotalTips { get; set; }
	}
	
	public class NeuraliumWalletElectionsMiningAggregateStatistics : WalletElectionsMiningAggregateStatistics, INeuraliumWalletElectionsMiningAggregateStatistics {

		static NeuraliumWalletElectionsMiningAggregateStatistics() {
			BsonMapper.Global.Entity<NeuraliumWalletElectionsMiningAggregateStatistics>().Id(x => x.Id);
		}
		
		public decimal TotalBounty { get; set; }
		public decimal TotalTips { get; set; }
	}
}