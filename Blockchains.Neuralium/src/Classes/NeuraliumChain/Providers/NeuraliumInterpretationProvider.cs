using System;
using System.Collections.Generic;
using System.Linq;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards.Implementations;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.DataStructures;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Blocks.Specialization.Elections.Results.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Moderator.V1;
using Blockchains.Neuralium.Classes.NeuraliumChain.Events.Transactions.Specialization.Tags;
using Blockchains.Neuralium.Classes.NeuraliumChain.Processors.TransactionInterpretation;
using Blockchains.Neuralium.Classes.NeuraliumChain.Tools;
using Blockchains.Neuralium.Classes.NeuraliumChain.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.DataStructures;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Elections.Results.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Specialization.Genesis;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Identifiers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Transactions.Specialization.Moderator.V1;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Processors.TransactionInterpretation;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Wallet.Account.Snapshots;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.Workflows.Tasks.Routing;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Providers {

	public interface INeuraliumInterpretationProviderCommon : IInterpretationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
	}

	public interface INeuraliumInterpretationProvider : INeuraliumInterpretationProviderGenerix<NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteContext, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumTrackedAccountsSqliteContext, INeuraliumBlock, INeuraliumAccountSnapshot, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry, NeuraliumStandardAccountKeysSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry, NeuraliumChainOptionsSnapshotSqliteEntry>, INeuraliumInterpretationProviderCommon {
	}

	public class NeuraliumInterpretationProvider : NeuraliumInterpretationProviderGenerix<NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteContext, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumTrackedAccountsSqliteContext, INeuraliumBlock, INeuraliumAccountSnapshot, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry, NeuraliumStandardAccountKeysSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry, NeuraliumChainOptionsSnapshotSqliteEntry, NeuraliumWalletStandardAccountSnapshot, NeuraliumAccountAttribute, NeuraliumWalletJointAccountSnapshot, NeuraliumAccountAttribute, NeuraliumJointMemberAccount>, INeuraliumInterpretationProvider {
		public NeuraliumInterpretationProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}

	public interface INeuraliumInterpretationProviderGenerix<STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_CONTEXT, BLOCK, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> : IInterpretationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_CONTEXT, BLOCK, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumStandardAccountSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_CONTEXT : class, INeuraliumJointAccountSnapshotContext<JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT : class, INeuraliumAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT : class, INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_CONTEXT : class, INeuraliumChainOptionsSnapshotContext<CHAIN_OPTIONS_SNAPSHOT>
		where TRACKED_ACCOUNTS_CONTEXT : class, INeuraliumTrackedAccountsContext
		where BLOCK : IBlock
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshotEntry<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccountEntry, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new() {
	}

	public abstract class NeuraliumInterpretationProviderGenerix<STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_CONTEXT, BLOCK, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT, STANDARD_WALLET_ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_SNAPSHOT, JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT> : InterpretationProvider<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_CONTEXT, BLOCK, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT, STANDARD_WALLET_ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_SNAPSHOT, JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT_CONTEXT : INeuraliumStandardAccountSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_CONTEXT : class, INeuraliumJointAccountSnapshotContext<JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT : class, INeuraliumAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT : class, INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_CONTEXT : class, INeuraliumChainOptionsSnapshotContext<CHAIN_OPTIONS_SNAPSHOT>
		where TRACKED_ACCOUNTS_CONTEXT : class, INeuraliumTrackedAccountsContext
		where BLOCK : IBlock
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshotEntry<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccountEntry, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new()
		where STANDARD_WALLET_ACCOUNT_SNAPSHOT : class, INeuraliumWalletStandardAccountSnapshot<STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_WALLET_ACCOUNT_SNAPSHOT : class, INeuraliumWalletJointAccountSnapshot<JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttribute, new()
		where JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccount, new() {

		protected NeuraliumInterpretationProviderGenerix(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		private INeuraliumAccountSnapshotsProviderCommon AccountSnapshotsProvider => this.CentralCoordinator.ChainComponentProvider.AccountSnapshotsProvider;

		protected override ICardUtils CardUtils => NeuraliumCardsUtils.Instance;

		public override void InterpretGenesisBlockSnapshots(IGenesisBlock genesisBlock) {
			base.InterpretGenesisBlockSnapshots(genesisBlock);

			var otherModeratorAccounts = genesisBlock.ConfirmedMasterTransactions.OfType<INeuraliumGenesisAccountPresentationTransaction>();

			this.HandleNeuraliumsModeratorExtraAccounts(otherModeratorAccounts);
		}

		public override SynthesizedBlock CreateSynthesizedBlock() {
			return new NeuraliumSynthesizedBlock();
		}

		protected override SynthesizedBlock SynthesizeBlock(IBlock block, AccountCache accountCache, Dictionary<TransactionId, ITransaction> blockConfirmedTransactions) {
			SynthesizedBlock synthesizedBlock = base.SynthesizeBlock(block, accountCache, blockConfirmedTransactions);

			if(synthesizedBlock is NeuraliumSynthesizedBlock neuraliumSynthesizedBlock && block is INeuraliumBlock neuraliumBlock) {

			}

			return synthesizedBlock;
		}

		protected override SynthesizedBlock.SynthesizedElectionResult SynthesizeElectionResult(SynthesizedBlock synthesizedBlock, IFinalElectionResults result, IBlock block, AccountCache accountCache, Dictionary<TransactionId, ITransaction> blockConfirmedTransactions) {
			SynthesizedBlock.SynthesizedElectionResult synthesizedElectionResult = base.SynthesizeElectionResult(synthesizedBlock, result, block, accountCache, blockConfirmedTransactions);

			if(synthesizedElectionResult is NeuraliumSynthesizedBlock.NeuraliumSynthesizedElectionResult neuraliumSynthesizedElectionResult && result is INeuraliumFinalElectionResults neuraliumFinalElectionResults) {

				neuraliumSynthesizedElectionResult.InfrastructureServiceFees = neuraliumFinalElectionResults.InfrastructureServiceFees ?? 0;

				foreach(AccountId delegateAccount in synthesizedElectionResult.DelegateAccounts) {

					if(neuraliumFinalElectionResults.DelegateAccounts[delegateAccount] is INeuraliumDelegateResults neuraliumDelegateResults) {
						neuraliumSynthesizedElectionResult.DelegateBounties.Add(delegateAccount, neuraliumDelegateResults.BountyShare.Value);
					}
				}

				foreach(var electedAccount in synthesizedElectionResult.ElectedAccounts) {

					if(neuraliumFinalElectionResults.ElectedCandidates[electedAccount.Key] is INeuraliumElectedResults neuraliumElectedResults) {

						decimal tips = 0;

						// let's sum up the tips we get!
						foreach(TransactionId transationId in neuraliumElectedResults.Transactions) {
							if(blockConfirmedTransactions.ContainsKey(transationId)) {
								if(blockConfirmedTransactions[transationId] is ITipTransaction tipTransaction) {
									tips += tipTransaction.Tip;
								}
							}
						}

						neuraliumSynthesizedElectionResult.ElectedGains.Add(electedAccount.Key, (neuraliumElectedResults.BountyShare, tips));
					}
				}

			}

			return synthesizedElectionResult;
		}

		protected override void LocalAccountSnapshotEntryChanged(Dictionary<AccountId, List<Action>> changedLocalAccounts, IAccountSnapshot newEntry, IWalletAccountSnapshot original) {
			base.LocalAccountSnapshotEntryChanged(changedLocalAccounts, newEntry, original);

			if(newEntry is INeuraliumAccountSnapshot neuraliumAccountSnapshot && original is INeuraliumWalletAccountSnapshot neuraliumWalletAccountSnapshot) {
				if(neuraliumAccountSnapshot.Balance != neuraliumWalletAccountSnapshot.Balance) {

					AccountId accountId = newEntry.AccountId.ToAccountId();

					this.InsertChangedLocalAccountsEvent(changedLocalAccounts, accountId, () => {
						// seems our total was updated for this account

						TotalAPI total = this.CentralCoordinator.ChainComponentProvider.WalletProvider.GetAccountBalance(accountId, true);
						this.CentralCoordinator.PostSystemEvent(NeuraliumSystemEventGenerator.NeuraliumAccountTotalUpdated(accountId.SequenceId, accountId.AccountType, total));

					});
				}
			}
		}

		protected override void HandleChainOperatingRulesTransaction(IChainOperatingRulesTransaction chainOperatingRulesTransaction) {

			base.HandleChainOperatingRulesTransaction(chainOperatingRulesTransaction);

			if(chainOperatingRulesTransaction is INeuraliumChainOperatingRulesTransaction neuraliumChainOperatingRulesTransaction) {

				INeuraliumChainStateProvider chainStateProvider = this.CentralCoordinator.ChainComponentProvider.ChainStateProvider;
				
			}
		}

		protected override ITransactionInterpretationProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> CreateInterpretationProcessor() {
			return new NeuraliumTransactionInterpretationProcessorFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>().CreateTransactionInterpretationProcessor(this.CentralCoordinator);
		}

		protected override ITransactionInterpretationProcessor<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_SNAPSHOT, JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> CreateWalletInterpretationProcessor() {
			return new NeuraliumTransactionInterpretationProcessorFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_SNAPSHOT, STANDARD_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_SNAPSHOT, JOINT_WALLET_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_WALLET_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>().CreateTransactionInterpretationProcessor(this.CentralCoordinator);
		}

		protected virtual void HandleNeuraliumsModeratorExtraAccounts(IEnumerable<INeuraliumGenesisAccountPresentationTransaction> neuraliumsExtraAccounts) {
			//TODO: implement this
		}
	}

}