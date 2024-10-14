namespace Bdb.Curso.Core.Entities
{
    public class ProductBalance
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Relación con el usuario
        public User User { get; set; } // Navegación a la entidad User

        public DateTime Created { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Amount { get; set; }
    }
}
