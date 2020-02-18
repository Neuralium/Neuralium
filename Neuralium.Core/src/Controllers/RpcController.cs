using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Neuralium.Core.Controllers {

	[Route("rpc")]
	[ApiController]
	public class RpcController : Controller {

		public RpcController(IHubContext<RpcHub<IRpcClient>> hubContext) {

		}

		// GET api/values
		[HttpGet]
		public Task<IActionResult> Get() {

			string result = $"Neuralium node is online and RPC interfaces are available.";
			
			return Task.FromResult((IActionResult)this.Ok(result));
		}
	}
}