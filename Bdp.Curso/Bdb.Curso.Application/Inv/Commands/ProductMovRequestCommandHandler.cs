using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Inv.Commands
{
    public class ProductMovRequestCommandHandler :IRequestHandler<ProductMovRequestCommand,bool>
    {

        private readonly IGenericRepository<ProductKardex> _productKardexRepository;
        private readonly IGenericRepository<ProductBalance> _productBalanceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductMovRequestCommandHandler(
             IGenericRepository<ProductKardex> productKardexRepository,
             IGenericRepository<ProductBalance> productBalanceRepository,
             IUnitOfWork unitOfWork)
        {
            _productKardexRepository = productKardexRepository;
            _productBalanceRepository = productBalanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ProductMovRequestCommand request, CancellationToken cancellationToken)
        {
            bool result = false;

            if (request == null || request.Amount == 0)
                return false;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                //insertar un movimiento      - tabla 1
                //product kardex

                var itemkardex = new ProductKardex
                {
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    TypeId = request.TypeId,
                    Amount = request.Amount

                };
                await _productKardexRepository.AddAsync(itemkardex);


                //actualizar un saldo del producto    - tabla 2
                //product balance

                //buscarlo
                var prodbalanceItem = await _productBalanceRepository.Where(p => p.ProductId == request.ProductId).FirstOrDefaultAsync();



                // si existe el producto
                if (prodbalanceItem != null)
                {
                    if ((request.TypeId == 2) && (prodbalanceItem.Amount < request.Amount))
                        throw new Exception("Saldo negativo");


                    //update
                    switch (request.TypeId)
                    {
                        case 1:        //ingreso
                            prodbalanceItem.Amount += request.Amount;
                            prodbalanceItem.UserId = request.UserId;
                            prodbalanceItem.Created = DateTime.Now;
                            break;

                        case 2:        //egreso
                            prodbalanceItem.Amount -= request.Amount;
                            prodbalanceItem.UserId = request.UserId;
                            prodbalanceItem.Created = DateTime.Now;
                            break;

                        default:
                            break;
                    }
                    await _productBalanceRepository.UpdateAsync(prodbalanceItem);
                }
                else
                {
                    // si no existe 
                    //insercion inicial 

                    prodbalanceItem = new ProductBalance
                    {
                        ProductId = request.ProductId,
                        Amount = request.Amount,
                        UserId = request.UserId,
                        Created = DateTime.Now
                    };
                    await _productBalanceRepository.AddAsync(prodbalanceItem);

                }

                await _unitOfWork.SaveAsync();

                await _unitOfWork.CommitTransactionAsync();

                result = true;
            }
            catch (Exception eex)
            {
                await _unitOfWork.RollbackTransactionAsync();
            }

            return result;
        }
    }


}
