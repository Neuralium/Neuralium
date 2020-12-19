using System.Collections.Generic;
using System.Threading.Tasks;
using CommandLine;
using Neuralium.Core.Classes.Configuration;
using Neuralium.Core.Classes.Runtime;

namespace Neuralium {
	internal class Program {
		public static Task<int> Main(string[] args) {
			ParserResult<NeuraliumOptions> result = Parser.Default.ParseArguments<NeuraliumOptions>(args);

			return result.MapResult(RunProgramAsync, HandleParseErrorAsync);
		}

		private static Task<int> RunProgramAsync(NeuraliumOptions cmdOptions) {
			Bootstrap boostrapper = new Bootstrap();
			boostrapper.SetCmdOptions(cmdOptions);

			return boostrapper.Run();
		}

		private static Task<int> HandleParseErrorAsync(IEnumerable<Error> errors) {

			return Task.FromResult(-1);
		}
	}
}