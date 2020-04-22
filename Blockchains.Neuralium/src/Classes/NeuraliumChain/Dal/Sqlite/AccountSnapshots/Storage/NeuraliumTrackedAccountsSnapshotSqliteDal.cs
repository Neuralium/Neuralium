using System;
using System.Threading.Tasks;
using Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.AccountSnapshots.Storage;
using Blockchains.Neuralium.Classes.NeuraliumChain.Factories;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.AccountSnapshots.Storage;
using Neuralia.Blockchains.Common.Classes.Tools;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Versions;

namespace Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.AccountSnapshots.Storage {

	public interface INeuraliumTrackedAccountsSqliteDal : INeuraliumTrackedAccountsDal<NeuraliumTrackedAccountsSqliteContext>, ITrackedAccountsSqliteDal<NeuraliumTrackedAccountsSqliteContext> {
	}

	public class NeuraliumTrackedAccountsSqliteDal : TrackedAccountsSqliteDal<NeuraliumTrackedAccountsSqliteContext>, INeuraliumTrackedAccountsSqliteDal {

		public NeuraliumTrackedAccountsSqliteDal(int groupSize, string folderPath, BlockchainServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(groupSize, folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {

		}

	}
}