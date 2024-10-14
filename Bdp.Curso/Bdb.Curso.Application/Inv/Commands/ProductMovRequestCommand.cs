using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Inv.Commands
{
    public class ProductMovRequestCommand   : IRequest<bool>
    {
        public int ProductId { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }

    }
}
