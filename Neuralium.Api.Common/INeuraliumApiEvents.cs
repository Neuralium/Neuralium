using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuralia.Blockchains.Core;

namespace Neuralium.Api.Common {
	public interface INeuraliumApiEvents {
		Task ReturnClientLongRunningEvent(int correlationId, int result, string error);
		Task LongRunningStatusUpdate(int correlationId, ushort eventId, byte eventType, ushort blockchainType, object message);

		Task EnterWalletPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, int attempt);
		Task EnterKeysPassphrase(int correlationId, ushort chainType, int keyCorrelationCode, string accountCode, string keyname, int attempt);
		Task RequestCopyWalletKeyFile(int correlationId, ushort chainType, int keyCorrelationCode, string accountCode, string keyname, int attempt);

		Task AccountTotalUpdated(string accountId, object total);
		Task RequestCopyWallet(int correlationId, ushort chainType);
		Task PeerTotalUpdated(int total);

		Task BlockchainSyncStatusChanged(ushort chainType, Enums.ChainSyncState syncStatus);
		Task WalletSyncStatusChanged(ushort chainType, Enums.ChainSyncState syncStatus);

		Task MiningStatusChanged(ushort chainType, bool isMining);

		Task WalletCreationStarted(int correlationId);
		Task WalletCreationEnded(int correlationId);

		Task AccountCreationStarted(int correlationId);
		Task AccountCreationEnded(int correlationId);
		Task AccountCreationMessage(int correlationId, string message);
		Task AccountCreationStep(int correlationId, string stepName, int stepIndex, int stepTotal);
		Task AccountCreationError(int correlationId, string error);

		Task KeyGenerationStarted(int correlationId, ushort chainType, string keyName, int keyIndex, int keyTotal);
		Task KeyGenerationEnded(int correlationId, ushort chainType, string keyName, int keyIndex, int keyTotal);
		Task KeyGenerationError(int correlationId, ushort chainType, string keyName, string error);
		Task KeyGenerationPercentageUpdate(int correlationId, ushort chainType, string keyName, int percentage);

		Task AccountPublicationStarted(int correlationId, ushort chainType);
		Task AccountPublicationEnded(int correlationId, ushort chainType);
		Task RequireNodeUpdate(ushort chainType, string chainName);
		
		Task AccountPublicationError(int correlationId, ushort chainType, string error);

		Task WalletSyncStarted(ushort chainType, long currentBlockId, long blockHeight, decimal percentage);
		Task WalletSyncEnded(ushort chainType, long currentBlockId, long blockHeight, decimal percentage);
		Task WalletSyncUpdate(ushort chainType, long currentBlockId, long blockHeight, decimal percentage, string estimatedTimeRemaining);
		Task WalletSyncError(ushort chainType, string error);

		Task BlockchainSyncStarted(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage);
		Task BlockchainSyncEnded(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage);
		Task BlockchainSyncUpdate(ushort chainType, long currentBlockId, long publicBlockHeight, decimal percentage, string estimatedTimeRemaining);
		Task BlockchainSyncError(ushort chainType, string error);

		Task TransactionSent(int correlationId, ushort chainType, string transactionId);
		Task TransactionConfirmed(ushort chainType, string transactionId, object transaction);
		Task TransactionReceived(ushort chainType, string transactionId);
		Task TransactionMessage(ushort chainType, string transactionId, string message);
		Task TransactionRefused(ushort chainType, string transactionId, string reason);
		Task TransactionError(int correlationId, ushort chainType, string transactionId, List<ushort> errorCodes);

		Task MiningStarted(ushort chainType);
		Task MiningEnded(ushort chainType, int status);
		Task MiningElected(ushort chainType, long electionBlockId, byte level);
		Task MiningPrimeElected(ushort chainType, long electionBlockId, byte level);
		Task MiningPrimeElectedMissed(ushort chainType, long publicationBlockId, long electionBlockId, byte level);

		Task NeuraliumMiningBountyAllocated(ushort chainType, long blockId, decimal bounty, decimal transactionTip, string delegateAccountId);
		Task NeuraliumMiningPrimeElected(ushort chainType, long electionBlockId, decimal bounty, decimal transactionTip, string delegateAccountId, byte level);
		Task NeuraliumTimelineUpdated();

		Task BlockInserted(ushort chainType, long blockId, DateTime timestamp, string hash, long publicBlockId, int lifespan);
		Task BlockInterpreted(ushort chainType, long blockId);

		Task DigestInserted(ushort chainType, int digestId, DateTime timestamp, string hash);

		Task ConsoleMessage(string message, DateTime timestamp, string level, object[] properties);
		Task Error(int correlationId, ushort chainType, string error);

		Task Message(ushort chainType, ushort messageCode, string defaultMessage, string[] properties);
		Task Alert(ushort chainType, ushort messageCode, string defaultMessage, int priorityLevel, int reportLevel, string[] parameters);
		Task ImportantWalletUpdate(ushort chainType);
		
		Task ConnectableStatusChanged(bool connectable);

		Task ShutdownCompleted();
		Task ShutdownStarted();

		Task TransactionHistoryUpdated(ushort chainType);

		Task ElectionContextCached(ushort chainType, long blockId, long maturityId, long difficulty);
		Task ElectionProcessingCompleted(ushort chainType, long blockId, int electionResultCount);

		Task AppointmentPuzzleBegin(ushort chainType, int secretCode, List<string> puzzles, List<string> instructions);
		Task AppointmentVerificationCompleted(ushort chainType, bool verified, string appointmentConfirmationCode);
		Task InvalidPuzzleEngineVersion(ushort chainType, int requiredVersion, int minimumSupportedVersion, int maximumSupportedVersion);

		Task THSTrigger(ushort chainType);
		Task THSBegin(ushort chainType, long difficulty, long targetNonce, long targetTotalDuration, long estimatedIterationTime, long estimatedRemainingTime, long startingNonce, long startingTotalNonce, long startingRound, long[] nonces, int[]solutions);
		Task THSRound(ushort chainType, int round, long totalNonce, long lastNonce, int lastSolution);
		Task THSIteration(ushort chainType, long[] nonces, long elapsed, long estimatedIterationTime, long estimatedRemainingTime, double benchmarkSpeedRatio);
		Task THSSolution(ushort chainType, List<long> nonces, List<int> solutions, long difficulty);
	}
}