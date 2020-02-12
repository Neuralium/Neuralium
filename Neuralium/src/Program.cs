using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.Runtime;

namespace Neuralium {
	internal class Program {
		public static async Task<int> Main(string[] args) {
			var result = Parser.Default.ParseArguments<NeuraliumOptions>(args);

			return await result.MapResult(async options => await RunProgram(options), HandleParseError);
		}

		private static async Task<int> RunProgram(NeuraliumOptions cmdOptions) {
			Bootstrap boostrapper = new Bootstrap();
			boostrapper.SetCmdOptions(cmdOptions);

			return await boostrapper.Run();
		}

		private static Task<int> HandleParseError(IEnumerable<Error> errors) {

			return Task.FromResult(-1);
		}
	}
}