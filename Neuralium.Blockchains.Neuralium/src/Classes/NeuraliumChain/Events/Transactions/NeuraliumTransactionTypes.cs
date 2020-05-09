using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions {

	// 	=====================================
	// 	printing values for NeuraliumTransactionTypes
	// 	_____________________________________
	// 	GENESIS = 1;
	// SIMPLE_PRESENTATION = 2;
	// JOINT_PRESENTATION = 3;
	// KEY_CHANGE = 4;
	// ACCREDITATION_CERTIFICATE = 5;
	// SET_ACCOUNT_RECOVERY = 6;
	// MODERATION_KEY_CHANGE = 1007;
	// MODERATION_SECRET_KEY_CHANGE = 1008;
	// MODERATION_ACCOUNT_RESET = 1009;
	// MODERATION_ACCOUNT_RESET_WARNING = 1010;
	// MODERATION_RECLAIM_ACCOUNTS = 1011;
	// MODERATION_PRESENTATION = 1012;
	// MODERATION_OPERATING_RULES = 1013;
	// DEBUG = 1014;
	// NONE = 1015;
	// --------Children-----------
	// NEURALIUM_TRANSFER = 10001;
	// NEURALIUM_MULTI_TRANSFER = 10002;
	// NEURALIUM_MODERATOR_DESTROY_TOKENS = 10003;
	// NEURALIUM_REFILL_NEURLIUMS = 10004;
	// =====================================

	public class NeuraliumTransactionTypes : TransactionTypes {

		public readonly TransactionType NEURALIUM_FREEZE_SUSPICIOUSACCOUNTS;

		public readonly TransactionType NEURALIUM_MODERATOR_DESTROY_TOKENS;
		public readonly TransactionType NEURALIUM_MULTI_TRANSFER;

		public readonly TransactionType NEURALIUM_REFILL_NEURLIUMS;

		public readonly TransactionType NEURALIUM_SAFU_CONTRIBUTIONS;
		public readonly TransactionType NEURALIUM_SAFU_TRANSFER;
		public readonly TransactionType NEURALIUM_SAFU_UPDATE_RATIO;

		public readonly TransactionType NEURALIUM_THREE_WAY_GATED_TRANSFER_TRANSACTION;

		public readonly TransactionType NEURALIUM_TRANSFER;
		public readonly TransactionType NEURALIUM_UNFREEZE_SUSPICIOUSACCOUNTS;
		public readonly TransactionType NEURALIUM_UNWIND_STOLEN_SUSPICIOUSACCOUNTS;

		static NeuraliumTransactionTypes() {
		}

		protected NeuraliumTransactionTypes() {
			this.NEURALIUM_TRANSFER = this.CreateChildConstant();
			this.NEURALIUM_MULTI_TRANSFER = this.CreateChildConstant();
			this.NEURALIUM_MODERATOR_DESTROY_TOKENS = this.CreateChildConstant();

			this.NEURALIUM_FREEZE_SUSPICIOUSACCOUNTS = this.CreateChildConstant();
			this.NEURALIUM_UNFREEZE_SUSPICIOUSACCOUNTS = this.CreateChildConstant();
			this.NEURALIUM_UNWIND_STOLEN_SUSPICIOUSACCOUNTS = this.CreateChildConstant();

			this.NEURALIUM_SAFU_CONTRIBUTIONS = this.CreateChildConstant();
			this.NEURALIUM_SAFU_TRANSFER = this.CreateChildConstant();
			this.NEURALIUM_SAFU_UPDATE_RATIO = this.CreateChildConstant();

			this.NEURALIUM_THREE_WAY_GATED_TRANSFER_TRANSACTION = this.CreateChildConstant();

			//this.PrintValues(";");

#if TESTNET || DEVNET
			this.NEURALIUM_REFILL_NEURLIUMS = 30_000;
#endif
		}

		public static new NeuraliumTransactionTypes Instance { get; } = new NeuraliumTransactionTypes();
	}

}