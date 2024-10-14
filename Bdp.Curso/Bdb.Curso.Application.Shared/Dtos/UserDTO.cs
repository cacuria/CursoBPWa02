namespace Bdb.Curso.Application.Shared.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string? Name { get; set; }

        public string? Roles { get; set; }

                              
        public string? Email { get; set; }

        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorExpiry { get; set; }
                
    }
}
