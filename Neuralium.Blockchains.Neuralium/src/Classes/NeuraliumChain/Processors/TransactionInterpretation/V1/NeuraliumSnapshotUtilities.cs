using System;
using System.Collections.Generic;
using System.Linq;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Types;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Tools.Data;
using Neuralia.Blockchains.Tools.Data.Arrays;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation.V1 {
	public static class NeuraliumSnapshotUtilities {

		/// <summary>
		///     this method will make sure to make all required adjustments to give the actual usable balance from an account
		/// </summary>
		/// <param name="snapshot"></param>
		/// <returns></returns>
		public static decimal GetUsableBalance(INeuraliumAccountSnapshot snapshot, DateTime transactionTime) {

			decimal frozen = 0;

			List<IAccountAttribute> features = snapshot.AppliedAttributesBase.Where(e => NeuraliumAccountAttributesTypes.BalanceImactingFeatures.Contains(e.AttributeType) && (!e.Start.HasValue || (e.Start.Value >= transactionTime)) && (!e.Expiration.HasValue || (e.Expiration.Value > transactionTime))).ToList();

			// if there are any freezes, they must be considered and removed from the balance
			if(features.Any()) {

				frozen = features.Sum(GetImpactAmount);
			}

			return snapshot.Balance - frozen;
		}

		/// <summary>
		///     get the token impact of a certain feature
		/// </summary>
		/// <param name="accountAttribute"></param>
		/// <returns></returns>
		public static decimal GetImpactAmount(IAccountAttribute accountAttribute) {

			if(accountAttribute.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE) {
				return GetFreezeImpactAmount(accountAttribute);
			}

			if(accountAttribute.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER) {
				return GetThreeWayGatedTransferImpactAmount(accountAttribute);
			}

			return 0;
		}

		public static decimal GetFreezeImpactAmount(IAccountAttribute accountAttribute) {

			SafeArrayHandle context = SafeArrayHandle.Wrap(accountAttribute.Context);

			if(!context.IsCleared) {

				NeuraliumFreezeAttributeContext ctx = new NeuraliumFreezeAttributeContext();
				ctx.Rehydrate(context);

				return ctx.Amount;
			}

			return 0;
		}

		public static decimal GetThreeWayGatedTransferImpactAmount(IAccountAttribute accountAttribute) {

			SafeArrayHandle context = SafeArrayHandle.Wrap(accountAttribute.Context);

			if(!context.IsCleared) {

				NeuraliumThreeWayGatedTransferAttributeContext ctx = new NeuraliumThreeWayGatedTransferAttributeContext();
				ctx.Rehydrate(context);

				if(ctx.Role == ThreeWayGatedTransferAttributeContext.Roles.Sender) {
					return ctx.Amount + ctx.VerifierServiceFee;
				}

				if(ctx.Role == ThreeWayGatedTransferAttributeContext.Roles.Receiver) {
					return ctx.VerifierServiceFee;
				}

				if(ctx.Role == ThreeWayGatedTransferAttributeContext.Roles.Verifier) {
					return 0;
				}
			}

			return 0;
		}
	}
}