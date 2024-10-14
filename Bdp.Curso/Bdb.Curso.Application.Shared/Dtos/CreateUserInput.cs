namespace Bdb.Curso.Core.Entities
{
    public class CreateUserInput
    {                                    
        public string UserName { get; set; } = string.Empty;
                                               
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
                                                                                              
        public string Roles { get; set; } = string.Empty;           

    }
}
