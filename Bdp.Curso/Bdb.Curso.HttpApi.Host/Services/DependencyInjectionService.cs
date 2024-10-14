 
using Bdb.Curso.EntityFrameworkCore;
using Bdb.Curso.EntityFrameworkCore.Repositories;
using Bdb.Curso.Application;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Infraestructure;


namespace Bdb.Curso.HttpApi.Host.Services
{
    public static class DependencyInjectionService
    {

        public static IServiceCollection AddApplication(
            this IServiceCollection services )
        {
                                                
            //Registrar repositorios
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            // Registro de UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<IInvAppServices, InvAppServices>();

            services.AddScoped<JwtTokenService>();
            

            services.AddScoped<IPasswordHasherService,  PasswordHasherService>();

            services.AddScoped<IUserAppServices, UserAppServices>();

            services.AddScoped<IEmailSenderService, EmailSenderService>();

            services.AddApplicationLayer();


            return services;
              
        }



    }
}
