using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Bdb.Curso.HttpApi.Host.Services;

namespace Bdb.Curso.HttpApi.Host.Authorization
{
    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly string _requiredPermission;

        public CustomAuthorizeFilter(JwtTokenService jwtTokenService, string requiredPermission)
        {
            _jwtTokenService = jwtTokenService;
            _requiredPermission = requiredPermission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
     

            if (token != null)
            {
                try
                {
                    // Desencriptar y validar el token
                    var jwt = _jwtTokenService.DecryptToken(token);

                    // Validar token y permisos
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var claimsPrincipal = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
                    {
                        ValidIssuer = _jwtTokenService.GetIssuer(),
                        ValidAudience = _jwtTokenService.GetAudience(),
                        IssuerSigningKey = _jwtTokenService.GetPublicKey(),
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    }, out _);

                    // Verificar el permiso en los claims
                    var hasPermission = claimsPrincipal.HasClaim("Permission", _requiredPermission);

                    if (!hasPermission)
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
                catch (Exception)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            else
            {
                context.Result = new UnauthorizedResult(); // No hay token en la solicitud
            }
        }
    }



    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string requiredPermission) : base(typeof(CustomAuthorizeFilter))
        {
            Arguments = new object[] { requiredPermission };
        }
    }
}
