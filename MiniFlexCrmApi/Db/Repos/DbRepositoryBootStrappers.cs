using System.Reflection;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public static class RepositoryBootstrapper
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Find all types that inherit from DbEntity (i.e., *DbModel classes)
        var dbModels = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(DbEntity).IsAssignableFrom(t) && t.Name.EndsWith("DbModel"))
            .ToList();

        foreach (var modelType in dbModels)
        {
            var repoType = typeof(DbEntityRepo<>).MakeGenericType(modelType);
            var interfaceType = typeof(IRepo<>).MakeGenericType(modelType);

            // Register repository as a scoped service
            services.AddSingleton(interfaceType, repoType);
        }
        
        services.AddSingleton<IUserRepo,UserRepo>();

        return services;
    }
}