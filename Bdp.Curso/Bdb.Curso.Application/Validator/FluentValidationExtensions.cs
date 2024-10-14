using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Validator
{
    public static class FluentValidationExtensions
    {
        public static IMvcBuilder AddCustomFluentValidation(this IMvcBuilder builder)
         {
       
            builder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<LoginModelValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<CreateUserInputValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<GetProductRequestValidator>();

            });


            return builder;
        }
    }
}
