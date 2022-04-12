using PEX.Application.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Api.Extensions;
public static class ModuleExtensions
{
	public static WebApplication RegisterEndpoints(this WebApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException(nameof(app));
		}

		var modules = app.Services.GetServices<IModule>();
		foreach (var module in modules)
		{
			module.RegisterEndpoints(app);
		}

		return app;
	}
}
