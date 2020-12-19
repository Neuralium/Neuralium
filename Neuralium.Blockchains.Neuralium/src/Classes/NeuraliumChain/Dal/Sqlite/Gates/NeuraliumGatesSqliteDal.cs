using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.Gates;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Sqlite.Gates;
using Neuralia.Blockchains.Components.Transactions.Identifiers;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Core.General.Types;
using Neuralia.Blockchains.Core.General.Versions;
using Neuralia.Blockchains.Core.Tools;
using Neuralia.Blockchains.Tools.Data;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.Gates;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Factories;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.Gates {

	public interface INeuraliumGatesSqliteDal : IGatesSqliteDal<NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite>, INeuraliumGatesDal<NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite>{
	}

	public class NeuraliumGatesSqliteDal : GatesSqliteDal<NeuraliumGatesSqliteContext, NeuraliumStandardAccountGatesSqlite, NeuraliumJointAccountGatesSqlite>, INeuraliumGatesSqliteDal{

		public NeuraliumGatesSqliteDal(string folderPath, ServiceSet serviceSet, SoftwareVersion softwareVersion, INeuraliumChainDalCreationFactory chainDalCreationFactory, AppSettingsBase.SerializationTypes serializationType) : base(folderPath, serviceSet, softwareVersion, chainDalCreationFactory, serializationType) {
		}
	}
}