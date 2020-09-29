using System.Collections.Immutable;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.Cryptography.Keys;
using Neuralia.Blockchains.Core.General.Types;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {
	public class NeuraliumModeratorKeyChangeTransaction : ModeratorKeyChangeTransaction, INeuraliumModerationTransaction {

		public NeuraliumModeratorKeyChangeTransaction() {
			// rehydration only
		}

		public NeuraliumModeratorKeyChangeTransaction(ICryptographicKey cryptographicKey) : base(cryptographicKey) {
		}

		public NeuraliumModeratorKeyChangeTransaction(byte ordinalId, CryptographicKeyType keyType) : base(ordinalId, keyType) {
		}

		public override Enums.TransactionTargetTypes TargetType => Enums.TransactionTargetTypes.All;
		public override AccountId[] ImpactedAccounts =>this.TargetAccounts;
		public override AccountId[] TargetAccounts => this.GetSenderList();
	}
}