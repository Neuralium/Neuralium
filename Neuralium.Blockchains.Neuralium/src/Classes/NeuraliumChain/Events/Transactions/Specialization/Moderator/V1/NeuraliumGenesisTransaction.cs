using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Core.General.Versions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1 {
	public interface INeuraliumGenesisModeratorAccountPresentationTransaction : IGenesisModeratorAccountPresentationTransaction, INeuraliumModerationTransaction {

		//		AccountId DestructionAccountId { get; set; }
		//		
		//		SecretCryptographicKey DestructionAccountTransactionCryptographicKey { get; }
		//		SecretCryptographicKey DestructionAccountTransactionChangeCryptographicKey { get; }
		//		
		//		bool IsDestructionAccountTransactionKeyLoaded { get; }
		//		
		//		bool IsDestructionAccountGeneralChangeKeyLoaded { get; }
		//		
		//		AccountId ServiceFeesAccountId { get; set; }
		//		
		//		SecretCryptographicKey ServiceFeesAccountTransactionCryptographicKey { get; }
		//		SecretCryptographicKey ServiceFeesAccountTransactionChangeCryptographicKey { get; }
		//		
		//		bool IsServiceFeesAccountTransactionKeyLoaded { get; }
		//		
		//		bool IsServiceFeesAccountGeneralChangeKeyLoaded { get; }
	}

	public class NeuraliumGenesisModeratorAccountPresentationTransaction : GenesisModeratorAccountPresentationTransaction, INeuraliumGenesisModeratorAccountPresentationTransaction {

		protected override ComponentVersion<TransactionType> SetIdentity() {
			return (base.SetIdentity().Type.Value, 1, 0);
		}
	}
}