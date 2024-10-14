using Bdb.Curso.Application.Inv.Commands;
using Bdb.Curso.Application.Inv.Queries;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using MediatR;

namespace Bdb.Curso.Application
{
    public class InvAppServices : IInvAppServices
    {

        private readonly IMediator _mediator; // Usaremos MediatR para orquestar los commands y queries

        public InvAppServices(IMediator mediator
            )
        {

            _mediator = mediator;
        }

        public async Task<List<ProductDto>> GetProducts(string searchTerm, int pageNumber = 1)
        {
            var query = new GetProductsQuery { SearchTerm = searchTerm, PageNumber = pageNumber };
            return await _mediator.Send(query);

        }

        public async Task<bool> InventMov(ProductMovRequest request)
        {
                        var command = new ProductMovRequestCommand 
            {
                ProductId = request.ProductId,
                TypeId = request.TypeId,
                Amount = request.Amount,
                UserId = request.UserId

            };

            return await _mediator.Send(command);
        }

       

    }
}
