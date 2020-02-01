using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures.Types;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Gated;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.Structures;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.General.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.SAFU;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1.Security;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures.Types;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Gated;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.General.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Processors.TransactionInterpretation.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Tools.AccountAttributeContexts;
using Neuralia.Blockchains.Core;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Serialization;
using Serilog;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation.V1 {

	public class NeuraliumTransactionInterpretationProcessor<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> : TransactionInterpretationProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshot<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshot<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccount, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshot, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshot<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccount, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshot, new() {

		public NeuraliumTransactionInterpretationProcessor(INeuraliumCentralCoordinator centralCoordinator) : this(centralCoordinator, TransactionImpactSet.OperationModes.Real) {

		}

		public NeuraliumTransactionInterpretationProcessor(INeuraliumCentralCoordinator centralCoordinator, TransactionImpactSet.OperationModes operationMode) : base(centralCoordinator, operationMode) {
			this.snapshotCacheSet = new NeuraliumSnapshotCacheSet<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>(NeuraliumCardsUtils.Instance);
		}

		protected override CardsUtils CardsUtils => NeuraliumCardsUtils.Instance;

		protected override void RegisterTransactionImpactSets() {
			base.RegisterTransactionImpactSets();

			// this.RegisterTransactionImpactSetOverride(new SupersetTransactionImpactSet<INeuraliumStandardPresentationTransaction, IStandardPresentationTransaction>());
			// this.RegisterTransactionImpactSetOverride(new SupersetTransactionImpactSet<INeuraliumJointPresentationTransaction, IJointPresentationTransaction>());

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumChainAccreditationCertificateTransaction, IChainAccreditationCertificateTransaction>(interpretTransactionAccreditationCertificatesFunc: (t, parameters, parent) => {

				parent(t, parameters);
				
				ACCREDITATION_CERTIFICATE_SNAPSHOT certificate = parameters.snapshotCache.GetAccreditationCertificateSnapshotModify((int) t.CertificateId.Value);

				certificate.ProviderBountyshare = t.ProviderBountyshare;
				certificate.InfrastructureServiceFees = t.InfrastructureServiceFees;
			});

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumChainOperatingRulesTransaction, IChainOperatingRulesTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots, parent) => {

				parent(t, affectedSnapshots);

			}, interpretTransactionChainOptionsFunc: (t, parameters, parent) => {
				
				parent(t, parameters);
				
				if(parameters.operationModes == TransactionImpactSet.OperationModes.Real) {
					CHAIN_OPTIONS_SNAPSHOT options = null;

					if(parameters.snapshotCache.CheckChainOptionsSnapshotExists(1)) {
						options = parameters.snapshotCache.GetChainOptionsSnapshotModify(1);
					} else {
						options = parameters.snapshotCache.CreateNewChainOptionsSnapshot(1);
					}

					options.SAFUDailyRatio = t.SAFUDailyRatio;
					options.MinimumSAFUQuantity = t.MinimumSAFUQuantity;
					options.MaximumAmountDays = (int) t.MaximumAmountDays.Value;
				}

			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumDailySAFURatioTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.chainOptions.Add(1);

			}, interpretTransactionChainOptionsFunc: (t, parameters) => {

				if(parameters.operationModes == TransactionImpactSet.OperationModes.Real) {
					CHAIN_OPTIONS_SNAPSHOT options = parameters.snapshotCache.GetChainOptionsSnapshotModify(1);

					options.SAFUDailyRatio = t.SAFUDailyRatio;
					options.MinimumSAFUQuantity = t.MinimumSAFUQuantity;
					options.MaximumAmountDays = (int) t.MaximumAmountDays.Value;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumTransferTransaction>(getImpactedSnapshotsFunc: (t, affectedKeysSnapshots) => {

				affectedKeysSnapshots.AddAccountId(t.TransactionId.Account);
				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account);
				INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.Recipient);

				if(senderAccount != null) {
					senderAccount.Balance -= t.Amount + t.Tip;
					senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
				}

				if(recipientAccount != null) {
					recipientAccount.Balance += t.Amount;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumMultiTransferTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccountId(t.TransactionId.Account);
				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account);

				if(senderAccount != null) {
					senderAccount.Balance -= t.Total + t.Tip;
					senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
				}

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
					}
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IEmittingTransaction>(getImpactedSnapshotsFunc: (t, affectedKeysSnapshots) => {

				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);
			}, interpretTransactionAccountsFunc: (t, parameters) => {

				INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.Recipient);

				if(recipientAccount != null) {
					recipientAccount.Balance += t.Amount;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IMultiEmittingTransaction>(getImpactedSnapshotsFunc: (t, affectedKeysSnapshots) => {

				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);
			}, interpretTransactionAccountsFunc: (t, parameters) => {

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
					}
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IDestroyNeuraliumsTransaction>(getImpactedSnapshotsFunc: (t, affectedKeysSnapshots) => {

				affectedKeysSnapshots.AddAccountId(new AccountId(NeuraliumConstants.DEFAULT_MODERATOR_DESTRUCTION_ACCOUNT_ID, Enums.AccountTypes.Standard));
			}, interpretTransactionAccountsFunc: (t, parameters) => {

				INeuraliumAccountSnapshot deflationAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(new AccountId(NeuraliumConstants.DEFAULT_MODERATOR_DESTRUCTION_ACCOUNT_ID, Enums.AccountTypes.Standard));

				if(deflationAccount != null) {
					// lets eliminate it all, we are deflating the neuralium
					deflationAccount.Balance = 0;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumSAFUContributionTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);
				affectedSnapshots.chainOptions.Add(1);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				if(t.AcceptSAFUTermsOfService == false) {
					return;
				}

				// ensure the safu certificate is valid
				var safuCertificate = this.AccreditationCertificateProvider.GetAccountAccreditationCertificateBase(NeuraliumConstants.SAFU_ACCREDITATION_CERTIFICATE_ID);
				
				if(AccreditationCertificateUtils.IsValid(safuCertificate) == false) {
					return;
				}

				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account);

				if(senderAccount == null) {
					return;
				}

				CHAIN_OPTIONS_SNAPSHOT chainOptionsSnapshot = parameters.snapshotCache.GetChainOptionsSnapshotReadonly(1);

				decimal total = chainOptionsSnapshot.SAFUDailyRatio * t.DailyProtection * t.NumberDays;
				senderAccount.Balance -= total + t.Tip;
				senderAccount.Balance = Math.Max(senderAccount.Balance, 0);

				// lets determine the participation range

				decimal protectedNeuraliums = t.DailyProtection / chainOptionsSnapshot.SAFUDailyRatio;

				if(protectedNeuraliums <= 0) {
					return;
				}

				// we cant go longer than the maximum amount of days
				if(t.NumberDays > chainOptionsSnapshot.MaximumAmountDays) {
					return;
				}

				senderAccount.CreateNewCollectionEntry(out IAccountAttribute feature);

				feature.AttributeType = NeuraliumAccountAttributesTypes.Instance.SAFU.Value;
				feature.CorrelationId = NeuraliumConstants.SAFU_ACCREDITATION_CERTIFICATE_ID;
				feature.Start = t.Start;

				DateTime transactionStart = this.TrxDt(t);

				if((feature.Start == null) || (feature.Start < transactionStart)) {
					feature.Start = transactionStart;
				}

				if(feature.Start > transactionStart.AddDays(30)) {
					feature.Start = transactionStart.AddDays(30);
				}

				TimeSpan range = TimeSpan.FromDays(t.NumberDays);

				feature.Expiration = feature.Start + range;

				senderAccount.AddCollectionEntry(feature);

				INeuraliumAccountSnapshot safuAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(new AccountId(NeuraliumConstants.DEFAULT_NEURALIUM_SAFU_ACCOUNT_ID, Enums.AccountTypes.Standard));

				if(safuAccount != null) {
					safuAccount.Balance += total;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumSAFUTransferTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				AccountId safuAccountId = new AccountId(NeuraliumConstants.DEFAULT_NEURALIUM_SAFU_ACCOUNT_ID, Enums.AccountTypes.Standard);

				if(t.TransactionId.Account != safuAccountId) {
					return;
				}

				decimal total = 0;

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
						total += recipientSet.Amount;
					}
				}

				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(safuAccountId);
				senderAccount.Balance -= total;
				senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumFreezeSuspiciousFundsTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {
				var impacts = t.GetFlatImpactTree();

				foreach((AccountId key, decimal value) in impacts.holders) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(key);

					if(recipientAccount != null) {
						recipientAccount.CreateNewCollectionEntry(out IAccountAttribute freeze);

						freeze.AttributeType = NeuraliumAccountAttributesTypes.Instance.FREEZE.Value;
						freeze.CorrelationId = t.FreezeId;

						NeuraliumFreezeAttributeContext freezeFeatureContext = new NeuraliumFreezeAttributeContext();
						freezeFeatureContext.Amount = value;

						freeze.Context = freezeFeatureContext.DehydrateContext();

						recipientAccount.AddCollectionEntry(freeze);
					}
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumUnfreezeClearedFundsTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				foreach(NeuraliumUnfreezeClearedFundsTransaction.AccountUnfreeze recipientSet in t.Accounts) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId);

					recipientAccount?.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumUnwindStolenFundsTreeTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, interpretTransactionAccountsFunc: (t, parameters) => {

				// ok, lets unwind the ones that have funds they should not have
				foreach(NeuraliumUnwindStolenFundsTreeTransaction.AccountUnwindImpact recipientSet in t.AccountUnwindImpacts) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId);

					if(recipientAccount != null) {
						recipientAccount.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);

						recipientAccount.Balance -= recipientSet.UnwoundAmount;
						recipientAccount.Balance = Math.Max(recipientAccount.Balance, 0);
					}
				}

				// ok, lets refund the ones that were wronged
				foreach(NeuraliumUnwindStolenFundsTreeTransaction.AccountRestoreImpact recipientSet in t.AccountRestoreImpacts) {
					INeuraliumAccountSnapshot recipientAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId);

					if(recipientAccount != null) {
						recipientAccount.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);

						recipientAccount.Balance += recipientSet.RestoreAmount;
					}
				}
			});
			
			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumThreeWayGatedTransferTransaction, IThreeWayGatedTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots, parent) => {

				parent(t, affectedSnapshots);

			}, interpretTransactionAccountsFunc: (t, parameters, parent) => {

				parent(t, parameters);
				
				DateTime transactionStart = this.TrxDt(t);
				
				// 10 days is an international standard. it's more than enough time...  but still, they can select
				DateTime expirationTime = transactionStart + TimeSpan.FromDays(t.Duration);

				
				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.SenderAccountId);

				if(senderAccount != null) {
					senderAccount.Balance -= t.SenderVerifierBaseServiceFee + t.Tip;
					senderAccount.Balance = Math.Max(senderAccount.Balance, 0);

					senderAccount.CreateNewCollectionEntry(out IAccountAttribute feature);

					feature.AttributeType = NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER.Value;
					feature.CorrelationId = t.CorrelationId;
					feature.Expiration = expirationTime;
					
					NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();

					threeWayGatedTransferFeature.Amount = t.Amount;
					threeWayGatedTransferFeature.VerifierServiceFee = t.SenderVerifierServiceFee;
					threeWayGatedTransferFeature.Role = ThreeWayGatedTransferAttributeContext.Roles.Sender;
					feature.Context = threeWayGatedTransferFeature.DehydrateContext();

					senderAccount.AddCollectionEntry(feature);
				}

				// receiver
				var receiverAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.ReceiverAccountId);

				if(receiverAccount != null) {

					receiverAccount.Balance -= t.ReceiverVerifierBaseServiceFee;
					receiverAccount.Balance = Math.Max(receiverAccount.Balance, 0);
					
					receiverAccount.CreateNewCollectionEntry(out IAccountAttribute feature);

					feature.AttributeType = NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER.Value;
					feature.CorrelationId = t.CorrelationId;
					feature.Expiration = expirationTime;
					
					NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();

					threeWayGatedTransferFeature.Amount = t.Amount;
					threeWayGatedTransferFeature.VerifierServiceFee = t.ReceiverVerifierServiceFee;
					threeWayGatedTransferFeature.Role = ThreeWayGatedTransferAttributeContext.Roles.Receiver;
					feature.Context = threeWayGatedTransferFeature.DehydrateContext();

					receiverAccount.AddCollectionEntry(feature);
				}
				
				var verifierAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.VerifierAccountId);

				if(verifierAccount != null) {
					verifierAccount.Balance += (t.SenderVerifierBaseServiceFee + t.ReceiverVerifierBaseServiceFee );
					
					verifierAccount.CreateNewCollectionEntry(out IAccountAttribute feature);

					feature.AttributeType = NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER.Value;
					feature.CorrelationId = t.CorrelationId;
					feature.Expiration = expirationTime;
					
					NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();

					threeWayGatedTransferFeature.Amount = t.Amount;
					threeWayGatedTransferFeature.VerifierServiceFee = t.SenderVerifierServiceFee + t.ReceiverVerifierServiceFee;
					threeWayGatedTransferFeature.Role = ThreeWayGatedTransferAttributeContext.Roles.Verifier;
					feature.Context = threeWayGatedTransferFeature.DehydrateContext();

					verifierAccount.AddCollectionEntry(feature);
				}
			});
			

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumGatedJudgementTransaction, IGatedJudgementTransaction>(getImpactedSnapshotsFunc: (t, affectedSnapshots,  parent) => {

				parent(t, affectedSnapshots);
			}, interpretTransactionAccountsFunc: (t, parameters, parent) => {

				parent(t, parameters);
				
				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.SenderAccountId);

				DateTime transactionStart = this.TrxDt(t);
				
				if(senderAccount != null) {
					var freeze = senderAccount.AppliedAttributesBase.SingleOrDefault(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);

					if(freeze != null) {
						if(freeze.Expiration > transactionStart) {
							
							NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();
							threeWayGatedTransferFeature.Rehydrate(freeze.Context);
							
							senderAccount.Balance -= threeWayGatedTransferFeature.VerifierServiceFee;
							if(t.Judgement == GatedJudgementTransaction.GatedJudgements.Accepted) {
								senderAccount.Balance -= threeWayGatedTransferFeature.Amount;
							} 
							
							senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
						}

						senderAccount.RemoveCollectionEntry(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);
					}
				}

				INeuraliumAccountSnapshot receiverAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.ReceiverAccountId);

				if(receiverAccount != null) {
					var freeze = receiverAccount.AppliedAttributesBase.SingleOrDefault(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);
					
					
					if(freeze != null) {
						if(freeze.Expiration > transactionStart) {

							NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();
							threeWayGatedTransferFeature.Rehydrate(freeze.Context);
							
							receiverAccount.Balance -= threeWayGatedTransferFeature.VerifierServiceFee;
							if(t.Judgement == GatedJudgementTransaction.GatedJudgements.Accepted) {
								receiverAccount.Balance += threeWayGatedTransferFeature.Amount;
							} 
							
							receiverAccount.Balance = Math.Max(receiverAccount.Balance, 0);
						}
						receiverAccount.RemoveCollectionEntry(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);

					}
				}

				INeuraliumAccountSnapshot verifierAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.VerifierAccountId);

				if(verifierAccount != null) {
					var freeze = verifierAccount.AppliedAttributesBase.SingleOrDefault(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);
					
					
					if(freeze != null) {
						if(freeze.Expiration > transactionStart) {

							NeuraliumThreeWayGatedTransferAttributeContext threeWayGatedTransferFeature = new NeuraliumThreeWayGatedTransferAttributeContext();
							threeWayGatedTransferFeature.Rehydrate(freeze.Context);

							// take the fees no matter the judgement
							verifierAccount.Balance += threeWayGatedTransferFeature.VerifierServiceFee;
							verifierAccount.Balance = Math.Max(verifierAccount.Balance, 0);
						}

						verifierAccount.RemoveCollectionEntry(f => f.CorrelationId == t.CorrelationId && f.AttributeType == NeuraliumAccountAttributesTypes.Instance.THREE_WAY_GATED_TRANSFER);
					}
				}
			});

#if TESTNET || DEVNET
			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumRefillNeuraliumsTransaction>(getImpactedSnapshotsFunc: (t, affectedKeysSnapshots) => {

				affectedKeysSnapshots.AddAccountId(t.TransactionId.Account);
			}, interpretTransactionAccountsFunc: (t, parameters) => {

				INeuraliumAccountSnapshot senderAccount = parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account);

				if(senderAccount != null) {
					senderAccount.Balance += 1000;
				}
			});
#endif
		}

		public override void ApplyBlockElectionsInfluence(List<IFinalElectionResults> publicationResult, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyBlockElectionsInfluence(publicationResult, transactions);

			// now apply the network service fees if applicable

			var neuraliumElectionResult = publicationResult.OfType<INeuraliumFinalElectionResults>().ToList();

			if(neuraliumElectionResult.Any(r => r.InfrastructureServiceFees?.Value != 0)) {

				this.SetNetworkServiceFees(neuraliumElectionResult.Where(r => (r.InfrastructureServiceFees != null) && (r.InfrastructureServiceFees.Value != 0)).Select(r => r.InfrastructureServiceFees.Value).ToList());
			}
		}

		public override void ApplyBlockElectionsInfluence(List<SynthesizedBlock.SynthesizedElectionResult> finalElectionResults, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyBlockElectionsInfluence(finalElectionResults, transactions);

			// now apply the network service fees if applicable

			var neuraliumElectionResult = finalElectionResults.OfType<NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult>().ToList();

			if(neuraliumElectionResult.Any(r => r.InfrastructureServiceFees != 0)) {

				this.SetNetworkServiceFees(neuraliumElectionResult.Where(r => r.InfrastructureServiceFees != 0).Select(r => r.InfrastructureServiceFees).ToList());
			}
		}

		private void SetNetworkServiceFees(List<decimal> networkServiceFees) {
			AccountId networkServiceFeesAccount = new AccountId(NeuraliumConstants.DEFAULT_NETWORK_MAINTENANCE_SERVICE_FEES_ACCOUNT_ID, Enums.AccountTypes.Standard);

			if(this.IsAnyAccountTracked(new[] {networkServiceFeesAccount}.ToList())) {

				SnapshotKeySet impactedSnapshotKeys = new SnapshotKeySet();

				impactedSnapshotKeys.AddAccountId(networkServiceFeesAccount);

				// now, we can query the snapshots we will need
				this.snapshotCacheSet.EnsureSnapshots(impactedSnapshotKeys);

				STANDARD_ACCOUNT_SNAPSHOT serviceFeesAccount = this.snapshotCacheSet.GetStandardAccountSnapshotModify(networkServiceFeesAccount);

				if(serviceFeesAccount == null) {
					serviceFeesAccount = this.snapshotCacheSet.CreateNewStandardAccountSnapshot(networkServiceFeesAccount, null);
				}

				if(networkServiceFees.Any()) {
					serviceFeesAccount.Balance += networkServiceFees.Sum();
				}
			}
		}

		protected override void ApplyDelegateResultsToSnapshot(ACCOUNT_SNAPSHOT snapshot, IDelegateResults delegateResults, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyDelegateResultsToSnapshot(snapshot, delegateResults, transactions);

			INeuraliumDelegateResults results = (INeuraliumDelegateResults) delegateResults;

			snapshot.Balance += results.BountyShare;
		}

		protected override void ApplyElectedResultsToSnapshot(ACCOUNT_SNAPSHOT snapshot, IElectedResults electedResults, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyElectedResultsToSnapshot(snapshot, electedResults, transactions);

			INeuraliumElectedResults results = (INeuraliumElectedResults) electedResults;

			snapshot.Balance += results.BountyShare;

			// now we apply transaction fees
			foreach(TransactionId transaction in results.Transactions) {

				if(!transactions.ContainsKey(transaction)) {
					// this is a serious issue, obviously.  lets prevent it from crashing, but really, its a big deal
					Log.Error($"Transaction was not found for TID {transaction} while applying elected result tips.");

					continue;
				}

				if(transactions[transaction] is ITipTransaction tipTransaction) {
					snapshot.Balance += tipTransaction.Tip;
				}
			}

		}

		protected override void ApplyDelegateResultsToSnapshot(ACCOUNT_SNAPSHOT snapshot, AccountId accountId, SynthesizedBlock.SynthesizedElectionResult synthesizedElectionResult, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyDelegateResultsToSnapshot(snapshot, accountId, synthesizedElectionResult, transactions);

			if(synthesizedElectionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult) {
				snapshot.Balance += neuraliumSynthesizedElectionResult.DelegateBounties[accountId];
			}
		}

		protected override void ApplyElectedResultsToSnapshot(ACCOUNT_SNAPSHOT snapshot, AccountId accountId, SynthesizedBlock.SynthesizedElectionResult synthesizedElectionResult, Dictionary<TransactionId, ITransaction> transactions) {
			base.ApplyElectedResultsToSnapshot(snapshot, accountId, synthesizedElectionResult, transactions);

			if(synthesizedElectionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult) {

				(decimal bountyShare, decimal tips) electedEntry = neuraliumSynthesizedElectionResult.ElectedGains[accountId];
				decimal gain = electedEntry.bountyShare + electedEntry.tips;
				snapshot.Balance += gain;

				Log.Information($"We were elected in the block! We were allocated {electedEntry.bountyShare}N in bounty, {electedEntry.tips}N in tips for a total gain of {gain}N. Our new total is {snapshot.Balance}N");
			}
		}
	}
}