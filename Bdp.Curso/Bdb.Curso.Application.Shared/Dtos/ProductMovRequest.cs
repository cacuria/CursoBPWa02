namespace Bdb.Curso.Application.Shared.Dtos
{
    public class ProductMovRequest
    {
        public int ProductId { get; set; }
        public int TypeId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }

    }
}
