using System;
using System.IO.Abstractions;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage.Base;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainPool;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainPool;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainState;
using Blockchains.Neuralium.Classes.NeuraliumChain.Providers;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Wallet;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Blocks.Serialization.Blockchain.Utils;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Events.Digests;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Factories;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.DataAccess;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Factories {
	public interface INeuraliumChainDalCreationFactory : IChainDalCreationFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider> {
		Func<string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumChainStateDal> CreateChainStateDalFunc { get; }
	}

	public class NeuraliumChainDalCreationFactory : ChainDalCreationFactory<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider>, INeuraliumChainDalCreationFactory {

		// contexts
		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumChainStateContext> CreateChainStateContextFunc => EntityFrameworkContext.CreateContext<NeuraliumChainStateSqliteContext>;

		// contexts
		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumChainPoolContext> CreateChainPoolContextFunc => EntityFrameworkContext.CreateContext<NeuraliumChainPoolSqliteContext>;

		// here are replaceable injection functions
		public virtual Func<string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumChainPoolDal> CreateChainPoolDalFunc => (folderPath, serviceSet, serializationType) => new NeuraliumChainPoolSqliteDal(folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumStandardAccountSnapshotContext> CreateStandardAccountSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumStandardAccountSnapshotSqliteContext>;
		public virtual Func<int, string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumStandardAccountSnapshotDal> CreateStandardAccountSnapshotDalFunc => (groupSize, folderPath, serviceSet, serializationType) => new NeuraliumStandardAccountSnapshotSqliteDal(groupSize, folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumJointAccountSnapshotContext> CreateJointAccountSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumJointAccountSnapshotSqliteContext>;
		public virtual Func<int, string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumJointAccountSnapshotDal> CreateJointAccountSnapshotDalFunc => (groupSize, folderPath, serviceSet, serializationType) => new NeuraliumJointAccountSnapshotSqliteDal(groupSize, folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumAccreditationCertificatesSnapshotContext> CreateAccreditationCertificatesSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumAccreditationCertificatesSnapshotSqliteContext>;
		public virtual Func<int, string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumAccreditationCertificatesSnapshotDal> CreateAccreditationCertificatesSnapshotDalFunc => (groupSize, folderPath, serviceSet, serializationType) => new NeuraliumAccreditationCertificatesSnapshotSqliteDal(groupSize, folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumAccountKeysSnapshotContext> CreateStandardAccountKeysSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumStandardAccountKeysSnapshotSqliteContext>;
		public virtual Func<int, string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumAccountKeysSnapshotDal> CreateStandardAccountKeysSnapshotDalFunc => (groupSize, folderPath, serviceSet, serializationType) => new NeuraliumStandardAccountKeysSnapshotSqliteDal(groupSize, folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumChainOptionsSnapshotContext> CreateChainOptionsSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumChainOptionsSnapshotSqliteContext>;
		public virtual Func<string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumChainOptionsSnapshotDal> CreateChainOptionsSnapshotDalFunc => (folderPath, serviceSet, serializationType) => new NeuraliumChainOptionsSnapshotSqliteDal(folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public virtual Func<AppSettingsBase.SerializationTypes, INeuraliumTrackedAccountsContext> CreateTrackedAccountsSnapshotContextFunc => EntityFrameworkContext.CreateContext<NeuraliumTrackedAccountsSqliteContext>;
		public virtual Func<int, string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumTrackedAccountsDal> CreateTrackedAccountsSnapshotDalFunc => (groupSize, folderPath, serviceSet, serializationType) => new NeuraliumTrackedAccountsSqliteDal(groupSize, folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		// here are replaceable injection functions
		public virtual Func<string, BlockchainServiceSet, AppSettingsBase.SerializationTypes, INeuraliumChainStateDal> CreateChainStateDalFunc => (folderPath, serviceSet, serializationType) => new NeuraliumChainStateSqliteDal(folderPath, serviceSet, GlobalSettings.SoftwareVersion, this, serializationType);

		public override Func<INeuraliumCentralCoordinator, string, IFileSystem, IWalletSerialisationFal> CreateWalletSerialisationFal => (centralCoordinator, chainWalletDirectoryPath, fileSystem) => new NeuraliumWalletSerialisationFal(centralCoordinator, chainWalletDirectoryPath, fileSystem);

		public override CHAIN_STATE_DAL CreateChainStateDal<CHAIN_STATE_DAL, CHAIN_STATE_SNAPSHOT>(string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_STATE_DAL) this.CreateChainStateDalFunc(folderPath, serviceSet, serializationType);
		}

		public override CHAIN_STATE_CONTEXT CreateChainStateContext<CHAIN_STATE_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_STATE_CONTEXT) this.CreateChainStateContextFunc(serializationType);
		}

		public override CHAIN_STATE_DAL CreateChainPoolDal<CHAIN_STATE_DAL>(string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_STATE_DAL) this.CreateChainPoolDalFunc(folderPath, serviceSet, serializationType);
		}

		public override Func<ChainConfigurations, BlockChannelUtils.BlockChannelTypes, string, string, IBlockchainDigestChannelFactory, IFileSystem, IBlockchainEventSerializationFalReadonly> CreateSerializedArchiveFal => (configurations, activeChannels, blocksFolderPath, digestFolderPath, blockchainDigestChannelFactory, fileSystem) => new NeuraliumBlockchainEventSerializationFal(configurations, activeChannels, blocksFolderPath, digestFolderPath, blockchainDigestChannelFactory, fileSystem);

		public override CHAIN_POOL_CONTEXT CreateChainPoolContext<CHAIN_POOL_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_POOL_CONTEXT) this.CreateChainPoolContextFunc(serializationType);
		}

		public override STANDARD_ACCOUNT_SNAPSHOT_DAL CreateStandardAccountSnapshotDal<STANDARD_ACCOUNT_SNAPSHOT_DAL>(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (STANDARD_ACCOUNT_SNAPSHOT_DAL) this.CreateStandardAccountSnapshotDalFunc(groupSize, folderPath, serviceSet, serializationType);
		}

		public override STANDARD_ACCOUNT_SNAPSHOT_CONTEXT CreateStandardAccountSnapshotContext<STANDARD_ACCOUNT_SNAPSHOT_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (STANDARD_ACCOUNT_SNAPSHOT_CONTEXT) this.CreateStandardAccountSnapshotContextFunc(serializationType);
		}

		public override JOINT_ACCOUNT_SNAPSHOT_DAL CreateJointAccountSnapshotDal<JOINT_ACCOUNT_SNAPSHOT_DAL>(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (JOINT_ACCOUNT_SNAPSHOT_DAL) this.CreateJointAccountSnapshotDalFunc(groupSize, folderPath, serviceSet, serializationType);
		}

		public override JOINT_ACCOUNT_SNAPSHOT_CONTEXT CreateJointAccountSnapshotContext<JOINT_ACCOUNT_SNAPSHOT_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (JOINT_ACCOUNT_SNAPSHOT_CONTEXT) this.CreateJointAccountSnapshotContextFunc(serializationType);
		}

		public override ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL CreateAccreditationCertificateAccountSnapshotDal<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL>(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_DAL) this.CreateAccreditationCertificatesSnapshotDalFunc(groupSize, folderPath, serviceSet, serializationType);
		}

		public override ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT CreateAccreditationCertificateSnapshotContext<ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (ACCREDITATION_CERTIFICATE_ACCOUNT_SNAPSHOT_CONTEXT) this.CreateAccreditationCertificatesSnapshotContextFunc(serializationType);
		}

		public override ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_DAL CreateStandardAccountKeysSnapshotDal<ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_DAL>(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_DAL) this.CreateStandardAccountKeysSnapshotDalFunc(groupSize, folderPath, serviceSet, serializationType);
		}

		public override ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_CONTEXT CreateStandardAccountKeysSnapshotContext<ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (ACCOUNT_KEYS_ACCOUNT_SNAPSHOT_CONTEXT) this.CreateStandardAccountKeysSnapshotContextFunc(serializationType);
		}

		public override CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_DAL CreateChainOptionsSnapshotDal<CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_DAL>(string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_DAL) this.CreateChainOptionsSnapshotDalFunc(folderPath, serviceSet, serializationType);
		}

		public override CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_CONTEXT CreateChainOptionsSnapshotContext<CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (CHAIN_OPTIONS_ACCOUNT_SNAPSHOT_CONTEXT) this.CreateChainOptionsSnapshotContextFunc(serializationType);
		}

		public override TRACKED_ACCOUNTS_DAL CreateTrackedAccountsDal<TRACKED_ACCOUNTS_DAL>(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return (TRACKED_ACCOUNTS_DAL) this.CreateTrackedAccountsSnapshotDalFunc(groupSize, folderPath, serviceSet, serializationType);
		}

		public override TRACKED_ACCOUNTS_CONTEXT CreateTrackedAccountsContext<TRACKED_ACCOUNTS_CONTEXT>(AppSettingsBase.SerializationTypes serializationType) {
			return (TRACKED_ACCOUNTS_CONTEXT) this.CreateTrackedAccountsSnapshotContextFunc(serializationType);
		}

		public INeuraliumChainStateContext CreateChainStateContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateChainStateContext<INeuraliumChainStateContext>(serializationType);
		}

		public INeuraliumChainPoolContext CreateChainPoolContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateChainPoolContext<INeuraliumChainPoolContext>(serializationType);
		}

		public INeuraliumChainStateDal CreateChainStateDal(string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateChainStateDal<INeuraliumChainStateDal, INeuraliumChainStateEntry>(folderPath, serviceSet, serializationType);
		}

		public INeuraliumStandardAccountSnapshotDal CreateStandardAccountSnapshotDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateStandardAccountSnapshotDal<INeuraliumStandardAccountSnapshotDal>(groupSize, folderPath, serviceSet, serializationType);
		}

		public INeuraliumStandardAccountSnapshotContext CreateStandardAccountSnapshotContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateStandardAccountSnapshotContext<INeuraliumStandardAccountSnapshotContext>(serializationType);
		}

		public INeuraliumJointAccountSnapshotDal CreateJointAccountSnapshotDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateJointAccountSnapshotDal<INeuraliumJointAccountSnapshotDal>(groupSize, folderPath, serviceSet, serializationType);
		}

		public INeuraliumJointAccountSnapshotContext CreateJointAccountSnapshotContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateJointAccountSnapshotContext<INeuraliumJointAccountSnapshotContext>(serializationType);
		}

		public INeuraliumAccountKeysSnapshotDal CreateStandardAccountKeysSnapshotDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateStandardAccountKeysSnapshotDal<INeuraliumAccountKeysSnapshotDal>(groupSize, folderPath, serviceSet, serializationType);
		}

		public INeuraliumAccountKeysSnapshotContext CreateStandardAccountKeysSnapshotContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateStandardAccountKeysSnapshotContext<INeuraliumAccountKeysSnapshotContext>(serializationType);
		}

		public INeuraliumAccreditationCertificatesSnapshotDal CreateAccreditationCertificatesAccountSnapshotDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateAccreditationCertificateAccountSnapshotDal<INeuraliumAccreditationCertificatesSnapshotDal>(groupSize, folderPath, serviceSet, serializationType);
		}

		public INeuraliumAccreditationCertificatesSnapshotContext CreateAccreditationCertificatesAccountSnapshotContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateAccreditationCertificateSnapshotContext<INeuraliumAccreditationCertificatesSnapshotContext>(serializationType);
		}

		public INeuraliumChainOptionsSnapshotDal CreateChainOptionsSnapshotDal(string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateChainOptionsSnapshotDal<INeuraliumChainOptionsSnapshotDal>(folderPath, serviceSet, serializationType);
		}

		public INeuraliumChainOptionsSnapshotContext CreateChainOptionsSnapshotContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateChainOptionsSnapshotContext<INeuraliumChainOptionsSnapshotContext>(serializationType);
		}

		public INeuraliumTrackedAccountsDal CreateTrackedAccountsDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateTrackedAccountsDal<INeuraliumTrackedAccountsDal>(groupSize, folderPath, serviceSet, serializationType);
		}

		public INeuraliumTrackedAccountsContext CreateTrackedAccountsContext(AppSettingsBase.SerializationTypes serializationType) {
			return this.CreateTrackedAccountsContext<INeuraliumTrackedAccountsContext>(serializationType);
		}
	}

}