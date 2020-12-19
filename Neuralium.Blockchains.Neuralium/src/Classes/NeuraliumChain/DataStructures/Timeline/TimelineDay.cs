using System.Collections.Generic;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline {

	public class TimelineDay {

		public List<TimelineEntry> Entries { get; set; } = new List<TimelineEntry>();

		public string Day { get; set; }
		public decimal EndingTotal { get; set; }
		public decimal EntriesTotal { get; set; }

		public class TimelineEntry {

			public string Timestamp { get; set; }

			public string TransactionId { get; set; }
			public string SenderAccountId { get; set; }
			public string RecipientAccountIds { get; set; }
			public decimal Amount { get; set; }
			public decimal Tips { get; set; }

			public decimal TimestampTotal { get; set; }
			public decimal Total { get; set; }

			public long? ElectedBlockId { get; set; }

			public byte Direction { get; set; }
			public byte CreditType { get; set; }
			public bool Confirmed { get; set; }
		}
	}
}