using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Identifiers;
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
using Neuralia.Blockchains.Tools.Locking;
using Neuralia.Blockchains.Tools.Serialization;
using Serilog;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation.V1 {

	public class NeuraliumTransactionInterpretationProcessor<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> : TransactionInterpretationProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>, INeuraliumTransactionInterpretationProcessor<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshot<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshot<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccount, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshot, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshot<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccount, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshot, new(){

		public event Func<Amount, Task> ApplyUniversalBasicBountiesCallback;
		

		public NeuraliumTransactionInterpretationProcessor(INeuraliumCentralCoordinator centralCoordinator) : this(centralCoordinator, TransactionImpactSet.OperationModes.Real) {

		}

		public NeuraliumTransactionInterpretationProcessor(INeuraliumCentralCoordinator centralCoordinator, TransactionImpactSet.OperationModes operationMode) : base(centralCoordinator, operationMode) {
			this.snapshotCacheSet = new NeuraliumSnapshotCacheSet<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>(NeuraliumCardsUtils.Instance);
		}

		protected override CardsUtils CardsUtils => NeuraliumCardsUtils.Instance;

		protected INeuraliumAccountSnapshotsProvider NeuraliumAccountSnapshotsProvider => (INeuraliumAccountSnapshotsProvider)this.AccountSnapshotsProvider;
		
		protected override async Task RegisterTransactionImpactSets() {
			await base.RegisterTransactionImpactSets().ConfigureAwait(false);

			// this.RegisterTransactionImpactSetOverride(new SupersetTransactionImpactSet<INeuraliumStandardPresentationTransaction, IStandardPresentationTransaction>());
			// this.RegisterTransactionImpactSetOverride(new SupersetTransactionImpactSet<INeuraliumJointPresentationTransaction, IJointPresentationTransaction>());

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumChainAccreditationCertificateTransaction, IChainAccreditationCertificateTransaction>(interpretTransactionAccreditationCertificatesFunc: async (t, parameters, parent, lockContext) => {

				await parent(t, parameters, lockContext).ConfigureAwait(false);
				
				ACCREDITATION_CERTIFICATE_SNAPSHOT certificate = await parameters.snapshotCache.GetAccreditationCertificateSnapshotModify((int) t.CertificateId.Value, lockContext).ConfigureAwait(false);

				certificate.ProviderBountyshare = t.ProviderBountyshare;
				certificate.InfrastructureServiceFees = t.InfrastructureServiceFees;
			});

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumChainOperatingRulesTransaction, IChainOperatingRulesTransaction>(async (t, affectedSnapshots, parent, lockContext) => {

				await parent(t, affectedSnapshots, lockContext).ConfigureAwait(false);

			}, interpretTransactionChainOptionsFunc: async (t, parameters, parent, lockContext) => {
				
				await parent(t, parameters, lockContext).ConfigureAwait(false);
				
				if(parameters.operationModes == TransactionImpactSet.OperationModes.Real) {
					CHAIN_OPTIONS_SNAPSHOT options = null;

					if(await parameters.snapshotCache.CheckChainOptionsSnapshotExists(1, lockContext).ConfigureAwait(false)) {
						options = await parameters.snapshotCache.GetChainOptionsSnapshotModify(1, lockContext).ConfigureAwait(false);
					} else {
						options = await parameters.snapshotCache.CreateNewChainOptionsSnapshot(1, lockContext).ConfigureAwait(false);
					}

					options.SAFUDailyRatio = t.SAFUDailyRatio;
					options.MinimumSAFUQuantity = t.MinimumSAFUQuantity;
					options.MaximumAmountDays = (int) t.MaximumAmountDays.Value;
					
					options.UBBAmount = t.UBBAmount;
					options.UBBBlockRate = t.UBBBlockRate;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumDailySAFURatioTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.chainOptions.Add(1);

			}, interpretTransactionChainOptionsFunc: async (t, parameters,lockContext) => {

				if(parameters.operationModes == TransactionImpactSet.OperationModes.Real) {
					CHAIN_OPTIONS_SNAPSHOT options = await parameters.snapshotCache.GetChainOptionsSnapshotModify(1, lockContext).ConfigureAwait(false);

					options.SAFUDailyRatio = t.SAFUDailyRatio;
					options.MinimumSAFUQuantity = t.MinimumSAFUQuantity;
					options.MaximumAmountDays = (int) t.MaximumAmountDays.Value;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumTransferTransaction>(async (t, affectedKeysSnapshots,lockContext) => {

				affectedKeysSnapshots.AddAccountId(t.TransactionId.Account);
				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {

				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account, lockContext).ConfigureAwait(false);
				INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.Recipient, lockContext).ConfigureAwait(false);

				if(senderAccount != null) {
					senderAccount.Balance -= t.Amount + t.Tip;
					senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
				}

				if(recipientAccount != null) {
					recipientAccount.Balance += t.Amount;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumMultiTransferTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccountId(t.TransactionId.Account);
				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {

				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account, lockContext).ConfigureAwait(false);

				if(senderAccount != null) {
					senderAccount.Balance -= t.Total + t.Tip;
					senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
				}

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient, lockContext).ConfigureAwait(false);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
					}
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IEmittingTransaction>(async (t, affectedKeysSnapshots,lockContext) => {

				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);
			}, async (t, parameters,lockContext) => {

				INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.Recipient, lockContext).ConfigureAwait(false);

				if(recipientAccount != null) {
					recipientAccount.Balance += t.Amount;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IMultiEmittingTransaction>(async (t, affectedKeysSnapshots,lockContext) => {

				affectedKeysSnapshots.AddAccounts(t.TargetAccounts);
			}, async (t, parameters,lockContext) => {

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient, lockContext).ConfigureAwait(false);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
					}
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<IDestroyNeuraliumsTransaction>(async (t, affectedKeysSnapshots,lockContext) => {

				affectedKeysSnapshots.AddAccountId(new AccountId(NeuraliumConstants.DEFAULT_MODERATOR_DESTRUCTION_ACCOUNT_ID, Enums.AccountTypes.Standard));
			}, async (t, parameters,lockContext) => {

				INeuraliumAccountSnapshot deflationAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(new AccountId(NeuraliumConstants.DEFAULT_MODERATOR_DESTRUCTION_ACCOUNT_ID, Enums.AccountTypes.Standard), lockContext).ConfigureAwait(false);

				if(deflationAccount != null) {
					// lets eliminate it all, we are deflating the neuralium
					deflationAccount.Balance = 0;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumSAFUContributionTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);
				affectedSnapshots.chainOptions.Add(1);

			}, async (t, parameters,lockContext) => {

				if(t.AcceptSAFUTermsOfService == false) {
					return;
				}

				// ensure the safu certificate is valid
				var safuCertificate = await this.AccreditationCertificateProvider.GetAccountAccreditationCertificateBase(NeuraliumConstants.SAFU_ACCREDITATION_CERTIFICATE_ID).ConfigureAwait(false);
				
				if(AccreditationCertificateUtils.IsValid(safuCertificate) == false) {
					return;
				}

				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account, lockContext).ConfigureAwait(false);

				if(senderAccount == null) {
					return;
				}

				var chainOptionsSnapshot = await this.LoadChainOptionsSnapshot(parameters.snapshotCache, lockContext).ConfigureAwait(false);
				
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

				INeuraliumAccountSnapshot safuAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(new AccountId(NeuraliumConstants.DEFAULT_NEURALIUM_SAFU_ACCOUNT_ID, Enums.AccountTypes.Standard), lockContext).ConfigureAwait(false);

				if(safuAccount != null) {
					safuAccount.Balance += total;
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumSAFUTransferTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {

				AccountId safuAccountId = new AccountId(NeuraliumConstants.DEFAULT_NEURALIUM_SAFU_ACCOUNT_ID, Enums.AccountTypes.Standard);

				if(t.TransactionId.Account != safuAccountId) {
					return;
				}

				decimal total = 0;

				foreach(RecipientSet recipientSet in t.Recipients) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.Recipient, lockContext).ConfigureAwait(false);

					if(recipientAccount != null) {
						recipientAccount.Balance += recipientSet.Amount;
						total += recipientSet.Amount;
					}
				}

				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(safuAccountId, lockContext).ConfigureAwait(false);
				senderAccount.Balance -= total;
				senderAccount.Balance = Math.Max(senderAccount.Balance, 0);
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumFreezeSuspiciousFundsTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {
				var impacts = t.GetFlatImpactTree();

				foreach((AccountId key, decimal value) in impacts.holders) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(key, lockContext).ConfigureAwait(false);

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

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumUnfreezeClearedFundsTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {

				foreach(NeuraliumUnfreezeClearedFundsTransaction.AccountUnfreeze recipientSet in t.Accounts) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId, lockContext).ConfigureAwait(false);

					recipientAccount?.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);
				}
			});

			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumUnwindStolenFundsTreeTransaction>(async (t, affectedSnapshots,lockContext) => {

				affectedSnapshots.AddAccounts(t.TargetAccounts);

			}, async (t, parameters,lockContext) => {

				// ok, lets unwind the ones that have funds they should not have
				foreach(NeuraliumUnwindStolenFundsTreeTransaction.AccountUnwindImpact recipientSet in t.AccountUnwindImpacts) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId, lockContext).ConfigureAwait(false);

					if(recipientAccount != null) {
						recipientAccount.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);

						recipientAccount.Balance -= recipientSet.UnwoundAmount;
						recipientAccount.Balance = Math.Max(recipientAccount.Balance, 0);
					}
				}

				// ok, lets refund the ones that were wronged
				foreach(NeuraliumUnwindStolenFundsTreeTransaction.AccountRestoreImpact recipientSet in t.AccountRestoreImpacts) {
					INeuraliumAccountSnapshot recipientAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(recipientSet.AccountId, lockContext).ConfigureAwait(false);

					if(recipientAccount != null) {
						recipientAccount.RemoveCollectionEntry(entry => entry.CorrelationId == t.FreezeId && entry.AttributeType == NeuraliumAccountAttributesTypes.Instance.FREEZE);

						recipientAccount.Balance += recipientSet.RestoreAmount;
					}
				}
			});
			
			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumThreeWayGatedTransferTransaction, IThreeWayGatedTransaction>(async (t, affectedSnapshots, parent, lockContext) => {

				await parent(t, affectedSnapshots, lockContext).ConfigureAwait(false);

			}, async (t, parameters, parent, lockContext) => {

				await parent(t, parameters, lockContext).ConfigureAwait(false);
				
				DateTime transactionStart = this.TrxDt(t);
				
				// 10 days is an international standard. it's more than enough time...  but still, they can select
				DateTime expirationTime = transactionStart + TimeSpan.FromDays(t.Duration);

				
				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.SenderAccountId, lockContext).ConfigureAwait(false);

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
				var receiverAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.ReceiverAccountId, lockContext).ConfigureAwait(false);

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
				
				var verifierAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.VerifierAccountId, lockContext).ConfigureAwait(false);

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
			

			this.TransactionImpactSets.RegisterTransactionImpactSetOverride<INeuraliumGatedJudgementTransaction, IGatedJudgementTransaction>(async (t, affectedSnapshots,  parent, lockContext) => {

				await parent(t, affectedSnapshots, lockContext).ConfigureAwait(false);
			}, async (t, parameters, parent, lockContext) => {

				await parent(t, parameters, lockContext).ConfigureAwait(false);
				
				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.SenderAccountId, lockContext).ConfigureAwait(false);

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

				INeuraliumAccountSnapshot receiverAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.ReceiverAccountId, lockContext).ConfigureAwait(false);

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

				INeuraliumAccountSnapshot verifierAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.VerifierAccountId, lockContext).ConfigureAwait(false);

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
			this.TransactionImpactSets.RegisterTransactionImpactSet<INeuraliumRefillNeuraliumsTransaction>(async (t, affectedKeysSnapshots,lockContext) => {

				affectedKeysSnapshots.AddAccountId(t.TransactionId.Account);
			}, async (t, parameters,lockContext) => {

				INeuraliumAccountSnapshot senderAccount = await parameters.snapshotCache.GetAccountSnapshotModify<INeuraliumAccountSnapshot>(t.TransactionId.Account, lockContext).ConfigureAwait(false);

				if(senderAccount != null) {
					senderAccount.Balance += 1000;
				}
			});
#endif
		}

		public async Task<CHAIN_OPTIONS_SNAPSHOT> LoadChainOptionsSnapshot(ISnapshotCacheSet<ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> snapshotCache, LockContext lockContext) {
			CHAIN_OPTIONS_SNAPSHOT chainOptionsSnapshot = await snapshotCache.GetChainOptionsSnapshotReadonly(1, lockContext).ConfigureAwait(false);

			if(chainOptionsSnapshot == null) {
				chainOptionsSnapshot = (await AccountSnapshotsProvider.LoadChainOptionsSnapshot().ConfigureAwait(false)) as CHAIN_OPTIONS_SNAPSHOT;
			}

			return chainOptionsSnapshot;
		}
		public override async Task ApplyBlockElectionsInfluence(List<IFinalElectionResults> publicationResult, Dictionary<TransactionId, ITransaction> transactions, LockContext lockContext) {
			await base.ApplyBlockElectionsInfluence(publicationResult, transactions, lockContext).ConfigureAwait(false);

			// now apply the network service fees if applicable

			var neuraliumElectionResult = publicationResult.OfType<INeuraliumFinalElectionResults>().ToList();

			if(neuraliumElectionResult.Any(r => r.InfrastructureServiceFees?.Value != 0)) {

				await this.SetNetworkServiceFees(neuraliumElectionResult.Where(r => (r.InfrastructureServiceFees != null) && (r.InfrastructureServiceFees.Value != 0)).Select(r => r.InfrastructureServiceFees.Value).ToList(), lockContext).ConfigureAwait(false);
			}
		}

		public override async Task ApplyBlockElectionsInfluence(List<SynthesizedBlock.SynthesizedElectionResult> finalElectionResults, Dictionary<TransactionId, ITransaction> transactions, LockContext lockContext) {
			await base.ApplyBlockElectionsInfluence(finalElectionResults, transactions, lockContext).ConfigureAwait(false);

			// now apply the network service fees if applicable

			var neuraliumElectionResult = finalElectionResults.OfType<NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult>().ToList();

			if(neuraliumElectionResult.Any(r => r.InfrastructureServiceFees != 0)) {

				await this.SetNetworkServiceFees(neuraliumElectionResult.Where(r => r.InfrastructureServiceFees != 0).Select(r => r.InfrastructureServiceFees).ToList(), lockContext).ConfigureAwait(false);
			}
		}

		private async Task SetNetworkServiceFees(List<decimal> networkServiceFees, LockContext lockContext) {
			AccountId networkServiceFeesAccount = new AccountId(NeuraliumConstants.DEFAULT_NETWORK_MAINTENANCE_SERVICE_FEES_ACCOUNT_ID, Enums.AccountTypes.Standard);

			if(await this.IsAnyAccountTracked(new[] {networkServiceFeesAccount}.ToList()).ConfigureAwait(false)) {

				SnapshotKeySet impactedSnapshotKeys = new SnapshotKeySet();

				impactedSnapshotKeys.AddAccountId(networkServiceFeesAccount);

				// now, we can query the snapshots we will need
				await this.snapshotCacheSet.EnsureSnapshots(impactedSnapshotKeys, lockContext).ConfigureAwait(false);

				STANDARD_ACCOUNT_SNAPSHOT serviceFeesAccount = await this.snapshotCacheSet.GetStandardAccountSnapshotModify(networkServiceFeesAccount, lockContext).ConfigureAwait(false);

				if(serviceFeesAccount == null) {
					serviceFeesAccount = await snapshotCacheSet.CreateNewStandardAccountSnapshot(networkServiceFeesAccount, null, lockContext).ConfigureAwait(false);
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

		public async Task ApplyUniversalBasicBounties(BlockId blockId){
			var chainOptions = await this.AccountSnapshotsProvider.LoadChainOptionsSnapshot().ConfigureAwait(false);

			if (chainOptions == null){
				return;
			}
			var neuraliumChainOptions = (INeuraliumChainOptionsSnapshot)chainOptions;

			if (neuraliumChainOptions.UBBAmount != 0 && neuraliumChainOptions.UBBBlockRate != 0 && blockId.Value % neuraliumChainOptions.UBBBlockRate == 0){

				if (this.ApplyUniversalBasicBountiesCallback != null){
					await this.ApplyUniversalBasicBountiesCallback(neuraliumChainOptions.UBBAmount).ConfigureAwait(false);
				}
			}
		}
	}
}