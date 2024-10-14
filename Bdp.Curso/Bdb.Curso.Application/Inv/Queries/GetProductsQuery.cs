using Bdb.Curso.Application.Shared.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Inv.Queries
{
    public class GetProductsQuery : IRequest<List<ProductDTO>>
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }

    }
}
