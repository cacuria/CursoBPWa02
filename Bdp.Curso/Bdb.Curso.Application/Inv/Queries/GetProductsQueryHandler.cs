using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bdb.Curso.Application.Inv.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IGenericRepository<Product> _productsRepository;


        public GetProductsQueryHandler(
            IGenericRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;


        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            int pageSize = 10; // definir como politica

            IQueryable<ProductDto> query;
            List<ProductDto> paginatedProducts = new();


            try
            {
                query = _productsRepository
                       .Include(p => p.Category)           // utilizamos la relacion foranea
                       .Include(p => p.Supplier)
                       .Where(p => p.Name.Contains(request.SearchTerm ?? string.Empty))
                       .OrderBy(p => p.Name)
                       .Select(p => new ProductDto
                       {
                           Id = p.Id,
                           Name = p.Name,
                           CategoryName = p.Category.Name,
                           SupplierName = p.Supplier.Name
                       }
                       );

                paginatedProducts = await query
                  .Skip((int)((request.PageNumber - 1) * pageSize ?? 0))  // Omite los registros anteriores según la página actual
                  .Take(pageSize)  // Toma solo el número de registros especificado por el tamaño de la página
                  .ToListAsync(cancellationToken: cancellationToken);  // Ejecuta la consulta y convierte el resultado en una lista
            }
            catch
            {


            }
            return paginatedProducts;

        }

    }
}
