using System.Collections.Generic;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Core.General.Types.Specialized;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards {

	public interface INeuraliumAccountSnapshot : INeuraliumSnapshot, IAccountSnapshot {
		/// <summary>
		///     our account balance
		/// </summary>
		decimal Balance { get; set; }
	}

	public interface INeuraliumAccountSnapshot<ACCOUNT_ATTRIBUTE> : IAccountSnapshot<ACCOUNT_ATTRIBUTE>, INeuraliumAccountSnapshot
		where ACCOUNT_ATTRIBUTE : INeuraliumAccountAttribute {
	}
}