using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Neuralia.Blockchains.Core.Configuration;
using Neuralia.Blockchains.Tools;
using Neuralium.Core.Classes.Runtime;
using Neuralium.Core.Classes.Services;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Neuralium.Core.Classes.General {
	public static class RpcEventLogSinkExtensions {
		public static LoggerConfiguration RpcEventLogSink(this LoggerSinkConfiguration loggerConfiguration, Bootstrap bootstrap, IFormatProvider formatProvider = null) {
			return loggerConfiguration.Sink(new RpcEventLogSink(bootstrap, new RpcEventFormatter()));
		}
	}

	/// <summary>
	///     special serilog sink to redirect log messages to the rpc message event
	/// </summary>
	public class RpcEventLogSink : ILogEventSink {
		private readonly Bootstrap bootstrap;
		private readonly IFormatProvider formatProvider;
		private IRpcService rpcService;

		public RpcEventLogSink(Bootstrap bootstrap, IFormatProvider formatProvider) {
			this.formatProvider = formatProvider;
			this.bootstrap = bootstrap;
		}

		private IRpcService RpcService {
			get {
				if(this.rpcService == null) {
					this.rpcService = this.bootstrap.ServiceProvider?.GetService<IRpcService>();
				}

				return this.rpcService;
			}
		}

		public void Emit(LogEvent logEvent) {

			if(!(this.RpcService?.RpcProvider.ConsoleMessagesEnabled ?? false)) {
				return;
			}

			this.RpcService.RpcProvider.LogMessage(logEvent.RenderMessage(this.formatProvider), logEvent.Timestamp.DateTime, logEvent.Level.ToString(), logEvent.Properties.Select(p => (object) new {p.Key, Value = p.Value.ToString()}).ToArray());
		}
	}

	public class RpcEventFormatter : IFormatProvider, ICustomFormatter {

		public string Format(string format, object arg, IFormatProvider formatProvider) {
			return $"{DateTimeEx.CurrentTime}- {arg}";
		}

		public object GetFormat(Type formatType) {
			if(formatType == typeof(ICustomFormatter)) {
				return this;
			}

			return null;
		}
	}
}