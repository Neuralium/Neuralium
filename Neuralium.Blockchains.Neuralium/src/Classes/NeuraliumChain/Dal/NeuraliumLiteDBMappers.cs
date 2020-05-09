using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using LiteDB;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal {
	public static class NeuraliumLiteDBMappers {

		/// <summary>
		///     Register extra mapping types that are important
		/// </summary>
		public static void RegisterBasics() {

			// litedb does not map these unsigned types by default. so lets add them
			RegisterWalletSnaphostTypes();
		}

		public static void RegisterWalletSnaphostTypes() {

			BsonMapper.Global.Entity<NeuraliumWalletStandardAccountSnapshot>().Id(x => x.AccountId);

			BsonMapper.Global.Entity<NeuraliumWalletJointAccountSnapshot>().Id(x => x.AccountId);
		}
	}
}