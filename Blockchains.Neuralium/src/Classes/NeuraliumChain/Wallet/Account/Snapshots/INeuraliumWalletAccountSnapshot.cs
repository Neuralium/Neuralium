using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots {
	public interface INeuraliumWalletAccountSnapshot<ACCOUNT_ATTRIBUTE> : IWalletAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshot<ACCOUNT_ATTRIBUTE>
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute{
	}

	public interface INeuraliumWalletAccountSnapshot : INeuraliumWalletAccountSnapshot<NeuraliumAccountAttribute> {
	}
}