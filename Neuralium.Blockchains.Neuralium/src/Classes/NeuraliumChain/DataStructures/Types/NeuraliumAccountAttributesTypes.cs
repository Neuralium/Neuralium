using System.Collections.Generic;
using System.Linq;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Types {

	public class NeuraliumAccountAttributesTypes : AccountAttributesTypes {
		public readonly AccountAttributeType FREEZE;

		public readonly AccountAttributeType SAFU;

		static NeuraliumAccountAttributesTypes() {

		}

		protected NeuraliumAccountAttributesTypes() {

			//this.PrintValues(";");
			this.SAFU = this.CreateChildConstant();
			this.FREEZE = this.CreateChildConstant();

		}

		/// <summary>
		///     These are types of features that have an impact on an account balance
		/// </summary>
		public static List<AccountAttributeType> BalanceImactingFeatures => new[] {Instance.FREEZE, Instance.THREE_WAY_GATED_TRANSFER}.ToList();

		public static new NeuraliumAccountAttributesTypes Instance { get; } = new NeuraliumAccountAttributesTypes();
	}
}