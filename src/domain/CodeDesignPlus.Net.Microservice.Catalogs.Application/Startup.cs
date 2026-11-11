using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.Options;
using CodeDesignPlus.Net.Microservice.Catalogs.Application.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Application
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            MapsterConfigTypeDocument.Configure();

            services.AddOptions<SeedOptions>()
                .Bind(configuration.GetSection(SeedOptions.Section))
                .ValidateDataAnnotations();
        }
    }
}
