using System.Reflection;

namespace Finances.DependencyInjection;

public static class Extensions
{
    public static void InjectEntities(this IServiceCollection services)
    {
        var allServices = Assembly.GetCallingAssembly().GetTypes().Where(m => m.IsClass && m.GetInterface(nameof(IInjectAsSingleton)) != null).ToList();
        foreach (var service in allServices)
            services.AddSingleton(service);
        
    }
}
