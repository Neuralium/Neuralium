using CommandLine;
using Neuralia.Blockchains.Core.Configuration;

namespace Neuralium.Core.Classes.Configuration {

	public class NeuraliumOptions : ICommandLineOptions {

		[Option("accept-license-agreement", Default = "", Required = false, HelpText = "Do you accept the software license agreement? value must be YES.")]
		public string AcceptSoftwareLicenseAgreement { get; set; }

		[Option('r', "runtime-mode", Default = "", Required = false, HelpText = "Are we running this in docker or not.")]
		public string RuntimeMode { get; set; }

		[Option('m', "cloud-mode", Default = "", Required = false, HelpText = "What cloud mode are we.")]
		public string CloudMode { get; set; }

		[Option("serialization-type", Default = "", Required = false, HelpText = "What type of serialization mode does this node take.")]
		public string SerializationType { get; set; }

		[Option("no-rpc", Default = false, Required = false, HelpText = "Will skip the RPC server startup if set.")]
		public bool NoRPC { get; set; }

		[Option("debug-console", Default = false, Required = false, HelpText = "Will skip the peer to peer server startup if set.")]
		public bool DebugConsole { get; set; }

		[Option("no-p2p", Default = false, Required = false, HelpText = "Will skip the peer to peer server startup if set.")]
		public bool NoP2p { get; set; }

		[Option("no-time", Default = false, Required = false, HelpText = "Will skip the NTP time server query at startup if set.")]
		public bool NoTimeServer { get; set; }

		[Option("no-wallet", Default = false, Required = false, HelpText = "Will skip the creation or loading of the wallet startup if set.")]
		public bool NoWallet { get; set; }

		[Option("no-contract-chain", Default = false, Required = false, HelpText = "Will disable the contract chain if set.")]
		public bool NoContractChain { get; set; }

		[Option("no-neuraliums-chain", Default = false, Required = false, HelpText = "Will disable the neuraliums chain if set.")]
		public bool NoNeuraliumsChain { get; set; }

		[Option("config-section", Default = null, Required = false, HelpText = "Choose config section.")]
		public string ConfigSection { get; set; }

		[Option("skip-genesis-hash-verification", Default = false, Required = false, HelpText = "if set, the genesis hash verification will be skipped.")]
		public bool SkipGenesisHashVerification { get; set; }

		[Option("skip-digest-hash-verification", Default = false, Required = false, HelpText = "if set, the digest hash verification will be skipped.")]
		public bool SkipDigestHashVerification { get; set; }

		[Option('p', "port", Default = null, Required = false, HelpText = "Will skip the RPC server startup if set.")]
		public int? Port { get; set; }

		[Option("array-pools", Default = null, Required = false, HelpText = "do we use large buffers?.")]
		public bool? UseArrayPools { get; set; }
		
		// // Omitting long name, defaults to name of property, ie "--verbose"
		// [Option(Default = false, HelpText = "Prints all messages to standard output.")]
		// public bool Verbose { get; set; }

		// [Option("stdin", Default = false, HelpText = "Read from stdin")]
		// public bool stdin { get; set; }

		// [Value(0, MetaName = "offset", HelpText = "File offset.")]
		// public long? Offset { get; se
	}
}