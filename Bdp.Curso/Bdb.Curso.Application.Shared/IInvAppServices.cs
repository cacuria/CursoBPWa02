


using Bdb.Curso.Application.Shared.Dtos;

namespace Bdb.Curso.Application.Shared
{
    public interface IInvAppServices
    {


        Task<List<ProductDto>> GetProducts(string searchTerm, int pageNumber = 1);

        Task<bool> InventMov(ProductMovRequest request);

      //  Task<bool> InventMovSp(ProductMovRequest request);


    }



}
