using System;
using LiteDB;
using Neuralia.Blockchains.Core.General.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account {

	public interface INeuraliumWalletTimelineDay {
		[BsonId]
		int Id { get; set; }

		DateTime Timestamp { get; set; }

		decimal Total { get; set; }
	}

	public interface INeuraliumWalletTimeline {
		[BsonId]
		long Id { get; set; }

		int DayId { get; set; }
		string TransactionId { get; set; }

		DateTime Timestamp { get; set; }

		AccountId SenderAccountId { get; set; }
		string RecipientAccountIds { get; set; }

		decimal Amount { get; set; }
		decimal Tips { get; set; }

		decimal Total { get; set; }
		bool Confirmed { get; set; }

		NeuraliumWalletTimeline.MoneratyTransactionTypes Direction { get; set; }
		NeuraliumWalletTimeline.CreditTypes CreditType { get; set; }
	}

	/// <summary>
	///     A useful timeline view of all wallet neuralium events
	/// </summary>
	public class NeuraliumWalletTimeline : INeuraliumWalletTimeline {
		public enum CreditTypes : byte {
			None = 0,
			Tranasaction = 1,
			Election = 2
		}

		public enum MoneratyTransactionTypes : byte {
			Debit = 1,
			Credit = 2
		}

		[BsonId]
		public long Id { get; set; }

		public int DayId { get; set; }
		public string TransactionId { get; set; }
		public DateTime Timestamp { get; set; }
		public AccountId SenderAccountId { get; set; }
		public string RecipientAccountIds { get; set; }

		public decimal Amount { get; set; }
		public decimal Tips { get; set; }
		public decimal Total { get; set; }
		public bool Confirmed { get; set; }
		public MoneratyTransactionTypes Direction { get; set; } = MoneratyTransactionTypes.Debit;
		public CreditTypes CreditType { get; set; } = CreditTypes.None;
	}

	public class NeuraliumWalletTimelineDay : INeuraliumWalletTimelineDay {

		[BsonId]
		public int Id { get; set; }

		public DateTime Timestamp { get; set; }
		public decimal Total { get; set; }
	}
}