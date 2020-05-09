using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet {

	public interface INeuraliumUserWallet : IUserWallet {
	}

	public class NeuraliumUserWallet : UserWallet, INeuraliumUserWallet {
		static NeuraliumUserWallet() {
			BsonMapper.Global.Entity<NeuraliumUserWallet>().Id(x => x.Id);
		}
	}

}