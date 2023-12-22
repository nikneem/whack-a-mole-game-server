using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wam.Core.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AzureServices>().Bind(configuration.GetSection(AzureServices.SectionName)); //.ValidateOnStart();
        return services;
    }
}