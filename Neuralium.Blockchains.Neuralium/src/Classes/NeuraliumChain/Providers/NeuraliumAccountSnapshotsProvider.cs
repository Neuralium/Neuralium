using System.Threading.Tasks;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage;
using Microsoft.EntityFrameworkCore;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.AccountSnapshots.Cards;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Types.Specialized;
using Neuralia.Blockchains.Tools.Locking;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumAccountSnapshotsProviderCommon : IAccountSnapshotsProvider {

		Task ApplyUniversalBasicBounties(Amount amount);
	}

	public interface INeuraliumAccountSnapshotsProvider : INeuraliumAccountSnapshotsProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumStandardAccountSnapshotSqliteDal, NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteDal, NeuraliumJointAccountSnapshotSqliteContext, NeuraliumAccreditationCertificatesSnapshotSqliteDal, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteDal, NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteDal, NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumTrackedAccountsSqliteDal, NeuraliumTrackedAccountsSqliteContext, INeuraliumAccountSnapshot, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry, NeuraliumStandardAccountKeysSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry, NeuraliumChainOptionsSnapshotSqliteEntry>, INeuraliumAccountSnapshotsProviderCommon {
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumAccountSnapshotsProvider : NeuraliumAccountSnapshotsProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumStandardAccountSnapshotSqliteDal, NeuraliumStandardAccountSnapshotSqliteContext, NeuraliumJointAccountSnapshotSqliteDal, NeuraliumJointAccountSnapshotSqliteContext, NeuraliumAccreditationCertificatesSnapshotSqliteDal, NeuraliumAccreditationCertificatesSnapshotSqliteContext, NeuraliumStandardAccountKeysSnapshotSqliteDal, NeuraliumStandardAccountKeysSnapshotSqliteContext, NeuraliumChainOptionsSnapshotSqliteDal, NeuraliumChainOptionsSnapshotSqliteContext, NeuraliumTrackedAccountsSqliteDal, NeuraliumTrackedAccountsSqliteContext, INeuraliumAccountSnapshot, NeuraliumStandardAccountSnapshotSqliteEntry, NeuraliumStandardAccountAttributeSqliteEntry, NeuraliumJointAccountSnapshotSqliteEntry, NeuraliumJointAccountAttributeSqliteEntry, NeuraliumJointMemberAccountSqliteEntry, NeuraliumStandardAccountKeysSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotSqliteEntry, NeuraliumAccreditationCertificateSnapshotAccountSqliteEntry, NeuraliumChainOptionsSnapshotSqliteEntry>, INeuraliumAccountSnapshotsProvider {
		public NeuraliumAccountSnapshotsProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}

		protected override ICardUtils CardUtils => NeuraliumCardsUtils.Instance;
	}

	public interface INeuraliumAccountSnapshotsProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, STANDARD_ACCOUNT_SNAPSHOT_DAL, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_DAL, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_DAL, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_DAL, TRACKED_ACCOUNTS_CONTEXT, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> : IAccountSnapshotsProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, STANDARD_ACCOUNT_SNAPSHOT_DAL, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_DAL, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_DAL, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_DAL, TRACKED_ACCOUNTS_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where STANDARD_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumStandardAccountSnapshotDal<STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumStandardAccountSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumJointAccountSnapshotDal<JOINT_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumJointAccountSnapshotContext<JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumAccreditationCertificatesSnapshotDal<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL : class, INeuraliumAccountKeysSnapshotDal<STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT : DbContext, INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_DAL : class, INeuraliumChainOptionsSnapshotDal<CHAIN_OPTIONS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_CONTEXT : DbContext, INeuraliumChainOptionsSnapshotContext<CHAIN_OPTIONS_SNAPSHOT>
		where TRACKED_ACCOUNTS_DAL : class, INeuraliumTrackedAccountsDal
		where TRACKED_ACCOUNTS_CONTEXT : DbContext, INeuraliumTrackedAccountsContext
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshotEntry<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccountEntry, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new() {
	}

	public abstract class NeuraliumAccountSnapshotsProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, STANDARD_ACCOUNT_SNAPSHOT_DAL, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_DAL, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_DAL, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_DAL, TRACKED_ACCOUNTS_CONTEXT, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT> : AccountSnapshotsProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, STANDARD_ACCOUNT_SNAPSHOT_DAL, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_DAL, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_DAL, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_DAL, TRACKED_ACCOUNTS_CONTEXT, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>, INeuraliumAccountSnapshotsProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, STANDARD_ACCOUNT_SNAPSHOT_DAL, STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT_DAL, JOINT_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL, STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT_DAL, CHAIN_OPTIONS_SNAPSHOT_CONTEXT, TRACKED_ACCOUNTS_DAL, TRACKED_ACCOUNTS_CONTEXT, ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT, STANDARD_ACCOUNT_KEY_SNAPSHOT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT, CHAIN_OPTIONS_SNAPSHOT>
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where STANDARD_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumStandardAccountSnapshotDal<STANDARD_ACCOUNT_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where STANDARD_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumStandardAccountSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT, STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumJointAccountSnapshotDal<JOINT_ACCOUNT_SNAPSHOT_CONTEXT, JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where JOINT_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumJointAccountSnapshotContext<JOINT_ACCOUNT_SNAPSHOT, JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL : class, INeuraliumAccreditationCertificatesSnapshotDal<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT, ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT : DbContext, INeuraliumAccreditationCertificatesSnapshotContext<ACCREDITATION_CERTIFICATE_SNAPSHOT, ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_DAL : class, INeuraliumAccountKeysSnapshotDal<STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT, STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where STANDARD_ACCOUNT_KEYS_SNAPSHOT_CONTEXT : DbContext, INeuraliumAccountKeysSnapshotContext<STANDARD_ACCOUNT_KEY_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_DAL : class, INeuraliumChainOptionsSnapshotDal<CHAIN_OPTIONS_SNAPSHOT_CONTEXT, CHAIN_OPTIONS_SNAPSHOT>
		where CHAIN_OPTIONS_SNAPSHOT_CONTEXT : DbContext, INeuraliumChainOptionsSnapshotContext<CHAIN_OPTIONS_SNAPSHOT>
		where TRACKED_ACCOUNTS_DAL : class, INeuraliumTrackedAccountsDal<TRACKED_ACCOUNTS_CONTEXT>
		where TRACKED_ACCOUNTS_CONTEXT : DbContext, INeuraliumTrackedAccountsContext
		where ACCOUNT_SNAPSHOT : INeuraliumAccountSnapshot
		where STANDARD_ACCOUNT_SNAPSHOT : class, INeuraliumStandardAccountSnapshotEntry<STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where STANDARD_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_SNAPSHOT : class, INeuraliumJointAccountSnapshotEntry<JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT, JOINT_ACCOUNT_MEMBERS_SNAPSHOT>, ACCOUNT_SNAPSHOT, new()
		where JOINT_ACCOUNT_ATTRIBUTE_SNAPSHOT : class, INeuraliumAccountAttributeEntry, new()
		where JOINT_ACCOUNT_MEMBERS_SNAPSHOT : class, INeuraliumJointMemberAccountEntry, new()
		where STANDARD_ACCOUNT_KEY_SNAPSHOT : class, INeuraliumStandardAccountKeysSnapshotEntry, new()
		where ACCREDITATION_CERTIFICATE_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotEntry<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT>, new()
		where ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT : class, INeuraliumAccreditationCertificateSnapshotAccountEntry, new()
		where CHAIN_OPTIONS_SNAPSHOT : class, INeuraliumChainOptionsSnapshotEntry, new() {

		public NeuraliumAccountSnapshotsProviderGenerix(CENTRAL_COORDINATOR centralCoordinator) : base(centralCoordinator) {
		}

		public override bool IsAccountEntryNull(ACCOUNT_SNAPSHOT accountSnapshotEntry) {
			bool value = base.IsAccountEntryNull(accountSnapshotEntry);

			if(value) {
				return true;
			}

			return accountSnapshotEntry.Balance == 0;
		}

		protected override ICardUtils GetCardUtils() {
			return this.GetNeuraliumCardUtils();
		}

		protected INeuraliumCardsUtils GetNeuraliumCardUtils() {
			return NeuraliumCardsUtils.Instance;
		}

		public Task ApplyUniversalBasicBounties(Amount amount) {

			return this.StandardAccountSnapshotsDal.ApplyUniversalBasicBounties(amount);
		}

	#region snapshot operations

		protected override async Task<STANDARD_ACCOUNT_SNAPSHOT> PrepareNewStandardAccountSnapshots(STANDARD_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, AccountId temporaryHashId, IStandardAccountSnapshot source, LockContext lockContext) {
			STANDARD_ACCOUNT_SNAPSHOT snapshot = await base.PrepareNewStandardAccountSnapshots(db, accountId, temporaryHashId, source, lockContext).ConfigureAwait(false);

			return snapshot;
		}

		protected override async Task<STANDARD_ACCOUNT_SNAPSHOT> PrepareUpdateStandardAccountSnapshots(STANDARD_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, IStandardAccountSnapshot source, LockContext lockContext) {
			STANDARD_ACCOUNT_SNAPSHOT snapshot = await base.PrepareUpdateStandardAccountSnapshots(db, accountId, source, lockContext).ConfigureAwait(false);

			return snapshot;
		}

		protected override async Task<STANDARD_ACCOUNT_SNAPSHOT> PrepareDeleteStandardAccountSnapshots(STANDARD_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, LockContext lockContext) {
			STANDARD_ACCOUNT_SNAPSHOT snapshot = await base.PrepareDeleteStandardAccountSnapshots(db, accountId, lockContext).ConfigureAwait(false);

			return snapshot;
		}

		protected override async Task<JOINT_ACCOUNT_SNAPSHOT> PrepareNewJointAccountSnapshots(JOINT_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, AccountId temporaryHashId, IJointAccountSnapshot source, LockContext lockContext) {
			JOINT_ACCOUNT_SNAPSHOT snapshot = await base.PrepareNewJointAccountSnapshots(db, accountId, temporaryHashId, source, lockContext).ConfigureAwait(false);

			return snapshot;
		}

		protected override async Task<JOINT_ACCOUNT_SNAPSHOT> PrepareUpdateJointAccountSnapshots(JOINT_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, IJointAccountSnapshot source, LockContext lockContext) {
			JOINT_ACCOUNT_SNAPSHOT snapshot = await base.PrepareUpdateJointAccountSnapshots(db, accountId, source, lockContext).ConfigureAwait(false);

			return snapshot;
		}

		protected override async Task<JOINT_ACCOUNT_SNAPSHOT> PrepareDeleteJointAccountSnapshots(JOINT_ACCOUNT_SNAPSHOT_CONTEXT db, AccountId accountId, LockContext lockContext) {
			JOINT_ACCOUNT_SNAPSHOT snapshot = await base.PrepareDeleteJointAccountSnapshots(db, accountId, lockContext).ConfigureAwait(false);

			return snapshot;
		}

	#endregion

	}
}