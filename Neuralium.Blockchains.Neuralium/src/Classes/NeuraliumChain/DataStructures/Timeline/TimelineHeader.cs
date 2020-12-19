using System.Collections.Generic;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Timeline {

	public class TimelineHeader {
		public string FirstDay { get; set; }
		public string LastDay { get; set; }
		
		public Dictionary<int, Dictionary<int, int[]>> Days { get; set; } = new Dictionary<int, Dictionary<int, int[]>>();
	}
}