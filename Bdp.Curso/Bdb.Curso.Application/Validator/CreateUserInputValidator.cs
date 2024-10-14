using Bdb.Curso.Core.Entities;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Validator
{
    public class CreateUserInputValidator : AbstractValidator<CreateUserInput>
    {
        public CreateUserInputValidator()
        {                                
            
            RuleFor(x => x.Name).nameValidation();

            RuleFor(x => x.Password).passwordValidation();

            RuleFor(x => x.Email).emailValidation();

            RuleFor(x => x.UserName).userNameValidation();
                        
        }
    }
}
