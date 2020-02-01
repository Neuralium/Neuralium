using System.Collections.Generic;
using System.Linq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Types {

	public class NeuraliumAccountAttributesTypes : AccountAttributesTypes {

		/// <summary>
		/// These are types of features that have an impact on an account balance
		/// </summary>
		public static List<AccountAttributeType> BalanceImactingFeatures => new [] {Instance.FREEZE, Instance.THREE_WAY_GATED_TRANSFER }.ToList();
		
		public readonly AccountAttributeType SAFU;
		public readonly AccountAttributeType FREEZE;
		
		static NeuraliumAccountAttributesTypes() {
			
			
		}

		protected NeuraliumAccountAttributesTypes() {

			//this.PrintValues(";");
			this.SAFU = this.CreateChildConstant();
			this.FREEZE = this.CreateChildConstant();

		}

		public static new NeuraliumAccountAttributesTypes Instance { get; } = new NeuraliumAccountAttributesTypes();
	}
}