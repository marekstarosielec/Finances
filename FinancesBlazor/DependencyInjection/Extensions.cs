using DataView;
using Finances.DataSource;
using FinancesBlazor.ViewManager;
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

    public static void InjectEntities(this IServiceCollection services, IConfiguration configuration)
    {
        var allEntitiesTypes = Assembly.GetCallingAssembly().GetTypes().Where(m => m.IsClass && m.GetInterface(nameof(IEntity)) != null).ToList();
        var allEntitiesInstances = new List<View>();
        foreach (var entityType in allEntitiesTypes)
        {
            var entityInstance = Activator.CreateInstance(entityType) as IEntity;
            if (entityInstance == null)
                throw new InvalidCastException($"Failed to create instance of type {entityType.Name}");
            allEntitiesInstances.Add(entityInstance.GetView(configuration));
        }
        services.AddSingleton(allEntitiesInstances);
    }

    public static void InjectViews(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceFactory = new DataSourceFactory(configuration);
        var allEntitiesTypes = AppDomain.CurrentDomain.Load("Finances.DataView").GetTypes().Where(m => m.IsClass && m.GetInterface(nameof(IDataView)) != null).ToList();
        var allEntitiesInstances = new List<DataView.DataView>();
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
