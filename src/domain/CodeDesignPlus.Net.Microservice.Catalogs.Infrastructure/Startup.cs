using CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Seeds;

namespace CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure
{
    public class Startup : IStartup
    {
        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHostedService<CatalogSeedService>();
        }
    }
}
