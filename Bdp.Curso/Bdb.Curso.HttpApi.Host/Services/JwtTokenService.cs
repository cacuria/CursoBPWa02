using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using Bdb.Curso.HttpApi.Host.Authorization;
using Jose;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
 

namespace Bdb.Curso.HttpApi.Host.Services
{
                      
    public class JwtTokenService
    {
        private readonly JwtSettingsValues _jwtSettings;

        private readonly TwoFactorSettings _twoFactorSettings;
        private readonly IEmailSenderService _emailSender; // Servicio para enviar correos electrónicos

        private RSA _privateKey;
        private RSA _publicKey;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly IGenericRepository<User> _userRepository;

        public JwtTokenService(
            JwtSettingsValues jwtSettings,
            IOptions<TwoFactorSettings> twoFactorSettings,
            IEmailSenderService emailSender,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            IGenericRepository<User> userRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtSettings = jwtSettings;
            _twoFactorSettings = twoFactorSettings.Value;
            _emailSender = emailSender;
            LoadKeys();
            _userRepository = userRepository;
        }

        private void LoadKeys()
        {
            var privateKeyPem = _jwtSettings.PrivateKeyPath;  //File.ReadAllText(_jwtSettings.PrivateKeyPath);
            var publicKeyPem = _jwtSettings.PublicKeyPath; //  File.ReadAllText(_jwtSettings.PublicKeyPath);

            _privateKey = RSA.Create();
            _privateKey.ImportFromPem(privateKeyPem.ToCharArray());

            _publicKey = RSA.Create();
            _publicKey.ImportFromPem(publicKeyPem.ToCharArray());
        }

                            

        public async Task<TokenResponseDTO> GenerateToken(UserDTO user, string clientType)
        {
            if (_twoFactorSettings.Enabled)
            {
                // Generar el código de verificación y enviarlo por correo
                var verificationCode = new Random().Next(100000, 999999).ToString();
                user.TwoFactorCode = verificationCode;
                user.TwoFactorExpiry = DateTime.UtcNow.AddMinutes(10); // El código expira en 10 minutos



                var userDb = await _userRepository.Where(x => x.Id == user.Id).AsNoTracking().FirstOrDefaultAsync();

                if (userDb != null)
                {

                    userDb.TwoFactorCode = user.TwoFactorCode;
                    userDb.TwoFactorExpiry = user.TwoFactorExpiry;

                    try
                    {
                        // Actualizar el usuario en la base de datos
                        await _userRepository.UpdateAsync(userDb);

                    }
                    catch (Exception eex)
                    {

                        throw;
                    }
                }
                else
                {

                    throw new InvalidOperationException("Error enviando el código de verificación 2FA.");

                }


                try
                {
                    // Enviar el código de verificación por correo
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Código de verificación",
                        $"Tu código de verificación es: {verificationCode}");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error enviando el código de verificación 2FA.", ex);
                }

                // Retornar un token temporal hasta que el usuario valide el código de 2FA
                var tokenString = GeneratePending2FAToken(user);

                return new TokenResponseDTO
                {
                    AccessToken = tokenString,
                    RefreshToken = new RefreshTokenDTO() {Token=string.Empty,Expires=DateTime.Now } // No se genera RefreshToken hasta que se valide 2FA

                };
            }


            else
            {
                // Generar el token directamente
                return await GenerateFullToken(user, clientType);
            }
        }

        private string GeneratePending2FAToken(UserDTO user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(ClaimTypes.Role, "pending-2fa") // Indica que 2FA está pendiente
        };
                                          
       
            // Agregar los roles como claims y autorizaciones personalizadas
            var rolesList = user.Roles.Split(',').Select(r => r.Trim()).ToList();
            foreach (var role in rolesList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Obtener permisos desde la clase estática para cada rol
                var permissions = RolePermissionsStore.GetPermissionsByRole(role);
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }



            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(_privateKey);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = DateTime.UtcNow.AddMinutes(10), // Token temporal
            //    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            //};


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };


            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            // Codificar el token usando JWE
            var encryptedToken = JWT.Encode(
                accessTokenString,
                _publicKey,
                JweAlgorithm.RSA_OAEP,
                JweEncryption.A256GCM);

            return encryptedToken;
        }

        public async Task<TokenResponseDTO> ValidateTwoFactorAndGenerateToken(UserDTO user, string code)
        {
            // Verifica si el código es correcto y no ha expirado
            if (user.TwoFactorCode != code || user.TwoFactorExpiry < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Código 2FA incorrecto o expirado.");
            }

            // Limpiar el código de 2FA una vez validado
            user.TwoFactorCode = null;
            user.TwoFactorExpiry = null;


            // Obtener el usuario actualizado desde la base de datos
            var userFromDb = await _userRepository.GetByIdAsync(user.Id);

            // Actualizar el usuario en la base de datos
            await _userRepository.UpdateAsync(userFromDb);


            // Generar el token completo después de validar 2FA
            return await GenerateFullToken(user, "user");
        }

        private async Task<TokenResponseDTO> GenerateFullToken(UserDTO user, string clientType)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("client_type", clientType)
        };

            // Agregar los roles como claims y autorizaciones personalizadas
            var rolesList = user.Roles.Split(',').Select(r => r.Trim()).ToList();
            foreach (var role in rolesList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Obtener permisos desde la clase estática para cada rol
                var permissions = RolePermissionsStore.GetPermissionsByRole(role);
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new RsaSecurityKey(_privateKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            };

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            var encryptedToken = JWT.Encode(
                accessTokenString,
                _publicKey,
                JweAlgorithm.RSA_OAEP,
                JweEncryption.A256GCM);

            // Generar Refresh Token
            var refreshToken = await GenerateRefreshToken(user.Id);

            return new TokenResponseDTO
            {
                AccessToken = encryptedToken,
                RefreshToken = refreshToken
            };
        }

        public string DecryptToken(string encryptedToken)
        {
            try
            {
                var decryptedToken = JWT.Decode(encryptedToken, _privateKey);
                return decryptedToken;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException("Token inválido o no pudo ser desencriptado", ex);
            }
        }

        // Métodos para obtener el emisor, audiencia y clave pública
        public string GetIssuer()
        {
            return _jwtSettings.Issuer; // Obtener del archivo de configuración
        }

        public string GetAudience()
        {
            return _jwtSettings.Audience; // Obtener del archivo de configuración
        }

        public RsaSecurityKey GetPublicKey()
        {
            return new RsaSecurityKey(_publicKey); // Convertir la clave pública a RsaSecurityKey
        }

        public ClaimsPrincipal ValidateToken(string encryptedToken)
        {
            // Desencriptar el token
            var jwt = DecryptToken(encryptedToken);

            // Validar el token desencriptado
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new RsaSecurityKey(_privateKey) // Clave privada para verificar la firma
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);

            // Verifica si 2FA está habilitado
            if (_twoFactorSettings.Enabled)
            {
                // Busca si el token contiene la información de "pending-2fa"
                var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleClaim == "pending-2fa")
                {
                    // Si el usuario aún no ha validado el segundo factor, lanzar excepción
                    throw new SecurityTokenException("Autenticación de doble factor pendiente.");
                }
            }

            // El token es válido y el 2FA está completado (o no está habilitado)
            return principal;
        }

        private async Task<RefreshTokenDTO> GenerateRefreshToken(int userId)
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var refreshToken = Convert.ToBase64String(randomBytes);

            var rt = new RefreshTokenDTO
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays)
            };

            var newRefreshToken = new RefreshToken
            {
                Token = rt.Token,
                Expires = rt.Expires,
                UserId = userId,
                Created = DateTime.UtcNow
            };

            // Guardar en el repositorio
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return rt;
        }
    }






}