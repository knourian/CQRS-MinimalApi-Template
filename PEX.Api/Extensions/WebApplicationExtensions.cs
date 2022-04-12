using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEX.Api.Extensions;
public static class WebApplicationExtensions
{
	public static WebApplication ConfigureApplication(this WebApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException(nameof(app));
		}

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		return app;
	}
}
