using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Interfaces.ChainState;
using Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Dal.Sqlite.ChainState;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Dal.Interfaces.ChainState;
using Neuralia.Blockchains.Common.Classes.Blockchains.Common.Providers;

namespace Neuralium.Blockchains.Neuralium.Classes.NeuraliumChain.Providers {
	public interface INeuraliumChainStateProvider : IChainStateProvider<INeuraliumChainStateSqliteDal, INeuraliumChainStateSqliteContext, NeuraliumChainStateSqliteEntry, NeuraliumChainStateSqliteModeratorKeysEntry> {
	}

	/// <summary>
	///     Provide access to the chain state that is saved in the DB
	/// </summary>
	public class NeuraliumChainStateProvider : NeuraliumChainStateProviderGenerix<INeuraliumCentralCoordinator, INeuraliumChainComponentProvider, NeuraliumChainStateSqliteDal, NeuraliumChainStateSqliteContext, NeuraliumChainStateSqliteEntry, NeuraliumChainStateSqliteModeratorKeysEntry>, INeuraliumChainStateProvider {

		public NeuraliumChainStateProvider(INeuraliumCentralCoordinator centralCoordinator) : base(centralCoordinator) {
		}
	}

	public abstract class NeuraliumChainStateProviderGenerix<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_STATE_DAL, CHAIN_STATE_CONTEXT, CHAIN_STATE_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT> : ChainStateProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER, CHAIN_STATE_DAL, CHAIN_STATE_CONTEXT, CHAIN_STATE_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, INeuraliumChainStateProvider
		where CENTRAL_COORDINATOR : ICentralCoordinator<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_COMPONENT_PROVIDER : IChainComponentProvider<CENTRAL_COORDINATOR, CHAIN_COMPONENT_PROVIDER>
		where CHAIN_STATE_DAL : class, IChainStateDal<CHAIN_STATE_CONTEXT, CHAIN_STATE_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>
		where CHAIN_STATE_CONTEXT : class, INeuraliumChainStateContext
		where CHAIN_STATE_SNAPSHOT : class, IChainStateEntry<CHAIN_STATE_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, new()
		where MODERATOR_KEYS_SNAPSHOT : class, IChainStateModeratorKeysEntry<CHAIN_STATE_SNAPSHOT, MODERATOR_KEYS_SNAPSHOT>, new() {

		public NeuraliumChainStateProviderGenerix(CENTRAL_COORDINATOR centralCoordinator) : base(centralCoordinator) {
		}

		//		protected override CHAIN_STATE_SNAPSHOT CreateNewEntry() {
		//			CHAIN_STATE_SNAPSHOT sqliteEntry = new CHAIN_STATE_SNAPSHOT();
		//
		//			sqliteEntry.ChainInception = DateTime.MinValue;
		//			sqliteEntry.BlockHeight = 0;
		//
		//			return sqliteEntry;
		//		}
	}

}