using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Application.Contracts;
public interface IModule
{
	IEndpointRouteBuilder RegisterEndpoints(IEndpointRouteBuilder endpoints);
}
