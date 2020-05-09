using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots {

	public interface INeuraliumWalletStandardAccountSnapshot<ACCOUNT_ATTRIBUTE> : IWalletStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumWalletAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute {
	}

	public interface INeuraliumWalletStandardAccountSnapshot : INeuraliumWalletStandardAccountSnapshot<NeuraliumAccountAttribute>, INeuraliumWalletAccountSnapshot {
	}

	public class NeuraliumWalletStandardAccountSnapshot : WalletStandardAccountSnapshot<NeuraliumAccountAttribute>, INeuraliumWalletStandardAccountSnapshot {
		public byte? FreezeDataVersion { get; set; }
		public byte[] FreezeData { get; set; }

		public decimal Balance { get; set; } = new Amount();
	}
}