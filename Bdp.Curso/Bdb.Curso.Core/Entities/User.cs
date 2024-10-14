namespace Bdb.Curso.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        //Decoraciones
        public string Name { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime Created { get; set; }

        //Seguridad basada en roles
        public string Roles { get; set; }

        //Doble factor de autenticacion con cuenta azure
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorExpiry { get; set; }



    }
}
