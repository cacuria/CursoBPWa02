using AutoMapper;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using MailKit.Search;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Inv.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDTO>>
    {
        private readonly IGenericRepository<Product> _productsRepository;
        private readonly IMapper _mapper;

        public GetProductsQueryHandler(
            IGenericRepository<Product> productsRepository, 
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;

        }

        public async Task<List<ProductDTO>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            int pageSize = 10; // definir como politica

            IQueryable<ProductDTO> query;
            List<ProductDTO> paginatedProducts = new();

                                   
            try
            {
                query = _productsRepository
                       .Include(p => p.Category)           // utilizamos la relacion foranea
                       .Include(p => p.Supplier)
                       .Where(p => p.Name.Contains(request.SearchTerm))
                       .OrderBy(p => p.Name)
                       .Select(p => new ProductDTO
                       {
                           Id = p.Id,
                           Name = p.Name,
                           CategoryName = p.Category.Name,
                           SupplierName = p.Supplier.Name
                       }
                       );

                paginatedProducts = await query
                  .Skip((request.PageNumber - 1) * pageSize)  // Omite los registros anteriores según la página actual
                  .Take(pageSize)  // Toma solo el número de registros especificado por el tamaño de la página
                  .ToListAsync();  // Ejecuta la consulta y convierte el resultado en una lista
            }
            catch (Exception ex)
            {


            }                                       
            return paginatedProducts;

        }

    }
}
