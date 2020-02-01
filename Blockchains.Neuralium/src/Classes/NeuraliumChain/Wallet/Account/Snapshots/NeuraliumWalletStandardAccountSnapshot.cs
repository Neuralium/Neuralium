using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using LiteDB;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots {

	public interface INeuraliumWalletStandardAccountSnapshot<ACCOUNT_ATTRIBUTE> : IWalletStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumWalletAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumStandardAccountSnapshot<ACCOUNT_ATTRIBUTE>
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute{
	}

	public interface INeuraliumWalletStandardAccountSnapshot : INeuraliumWalletStandardAccountSnapshot<NeuraliumAccountAttribute>, INeuraliumWalletAccountSnapshot {
	}

	public class NeuraliumWalletStandardAccountSnapshot : WalletStandardAccountSnapshot<NeuraliumAccountAttribute>, INeuraliumWalletStandardAccountSnapshot {
		public byte? FreezeDataVersion { get; set; }
		public byte[] FreezeData { get; set; }

		public Amount Balance { get; set; } = new Amount(); 
	}
}