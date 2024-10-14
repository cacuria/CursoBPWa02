using Bdb.Curso.Application.Shared;
using Microsoft.AspNetCore.Identity;

namespace Bdb.Curso.Infraestructure
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            //agregar pesonalizacion     --- Pepper

            //password = password + "CursoBPWa02";


            //// Generar un salt con un factor de coste personalizado (12 rondas)
            //string customSalt = BCrypt.Net.BCrypt.GenerateSalt(12);
                                                                        
                                                                           
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
