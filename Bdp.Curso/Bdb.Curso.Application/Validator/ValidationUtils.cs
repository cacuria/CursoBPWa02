using FluentValidation;

namespace Bdb.Curso.Application.Validator
{
    public static class CustomValidators
    {

        public static IRuleBuilderOptions<T, string> searchTermValidation<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {                       
                 return ruleBuilder
                    .NotEmpty().WithMessage("El filtro no puede ser vacio")
                    .MinimumLength(3).WithMessage("Debe escribir al menos 3 caracteres");
        }



        public static IRuleBuilderOptions<T, string> passwordValidation<T>
          (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        }

        public static IRuleBuilderOptions<T, string> emailValidation<T>
        (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                    .NotEmpty().WithMessage("Email is required.")       // Verifica que el campo no esté vacío
                    .EmailAddress().WithMessage("Invalid email format."); // Verifica que el formato sea un correo válido


        }


        public static IRuleBuilderOptions<T, string> userNameValidation<T>
                (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                   .NotEmpty().WithMessage("Username is required.")         // Campo requerido
                    .MinimumLength(6).WithMessage("Username must be at least 6 characters long.")  // Mínimo 6 caracteres
                    .MaximumLength(16).WithMessage("Username must not exceed 16 characters.");  // Máximo 16 caracteres


        }


        public static IRuleBuilderOptions<T, string> nameValidation<T>
            (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                   .NotEmpty().WithMessage("Name is required.")         // Campo requerido
                    .MinimumLength(1).WithMessage("Name must be at least 1 characters long.")  // Mínimo 6 caracteres
                    .MaximumLength(128).WithMessage("Name must not exceed 128 characters.");  // Máximo 16 caracteres


        }


    }
}
