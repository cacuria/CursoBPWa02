using Microsoft.Extensions.DependencyInjection;
 

namespace Bdb.Curso.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // Registrar MediatR usando la versión correcta
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationModule).Assembly));

            // Aquí puedes registrar otros servicios de la capa de aplicación
            return services;
        }
    }
}
