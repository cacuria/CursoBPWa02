using FluentValidation;
using Bdb.Curso.Application.Shared.Dtos;


namespace Bdb.Curso.Application.Validator
{
    public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
    {
        public GetProductRequestValidator()
        {
            RuleFor(x => x.searchTerm).searchTermValidation();
             
            RuleFor(x => x.pageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Debe ser mayor o igual a 1");
                                       
        }

    }
}
