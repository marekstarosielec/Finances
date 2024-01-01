using DataViews;
using Finances.DataSource;
using System.Reflection;

namespace Finances.DependencyInjection;

public static class Extensions
{
    public static void InjectServices(this IServiceCollection services)
    {
        var allServices = Assembly.GetCallingAssembly().GetTypes().Where(m => m.IsClass && m.GetInterface(nameof(IInjectAsSingleton)) != null).ToList();
        foreach (var service in allServices)
            services.AddSingleton(service); 
    }

    public static void InjectViews(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceFactory = new DataSourceFactory(configuration);
        var allEntitiesTypes = AppDomain.CurrentDomain.Load("Finances.DataView").GetTypes().Where(m => m.IsClass && m.GetInterface(nameof(IDataView)) != null).ToList();
        var allEntitiesInstances = new List<DataView>();
        foreach (var entityType in allEntitiesTypes)
        {
            var entityInstance = Activator.CreateInstance(entityType, dataSourceFactory) as IDataView;
            if (entityInstance == null)
                throw new InvalidCastException($"Failed to create instance of type {entityType.Name}");
            allEntitiesInstances.Add(entityInstance.GetDataView());
        }
        services.AddSingleton(allEntitiesInstances);
    }
}
