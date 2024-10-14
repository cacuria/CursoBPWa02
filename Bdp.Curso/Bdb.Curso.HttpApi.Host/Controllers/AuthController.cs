using AutoMapper;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.HttpApi.Host.Authorization;
using Bdb.Curso.HttpApi.Host.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace Bdb.Curso.HttpApi.Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly JwtTokenService _jw;
     

        private readonly IUserAppServices _userAppServices;
        public AuthController(JwtTokenService jw,   IUserAppServices userAppServices)
        {
            _jw = jw;             
            _userAppServices = userAppServices;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await _userAppServices.Login(login);

            if (string.IsNullOrEmpty(user.UserName))
                return Unauthorized();

            // Generar el Access Token y el Refresh Token

            var tokenResponse = await _jw.GenerateToken(user, login.ClientType ?? string.Empty);

            // Crear el objeto de respuesta usando la Dto
            var response = new AuthResponseDto
            {
                IsSuccess = true,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken.Token,
                RefreshTokenExpires = tokenResponse.RefreshToken.Expires
            };


            // Retornar la respuesta con el token y el refresh token
            return Ok(response);

        }

        [HttpPost("validate-token")]
        public    IActionResult ValidateToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }


            ObjectResult returned;

            try
            {
                // Validar el token
                var claimsPrincipal = _jw.ValidateToken(request.Token);

                var answer = false;

                if (claimsPrincipal != null)
                    answer = true;
                else
                    return BadRequest("Token is required.");

                returned = StatusCode(StatusCodes.Status201Created, new { isSuccess = answer, Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }) });


            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized(new { IsValid = false, Message = "Token has expired." });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { IsValid = false, ex.Message });
            }

            return returned;

        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Elimina cualquier cookie de autenticación
            HttpContext.SignOutAsync();
 
            // Devuelve un mensaje de confirmación
            return Ok(new { message = "Sesión cerrada exitosamente." });
        }




        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutUser(int id, CreateUserInput user)
        {
            if (id == 0) return BadRequest();

            try
            {
                await _userAppServices.UpdateUser(id, user);
            }
            catch  
            {
                return NotFound();
            }


            return Ok(new { Message = "User updated successfully." });
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost("create")]
        public async Task<ActionResult<UserDto>> PostUser(CreateUserInput user)
        {

            UserDto ret = new();


            if (user == null) return BadRequest();

            try
            {
                ret = await _userAppServices.CreateUser(user);
            }
            catch  
            {
                return NotFound();
            }

            return Ok(ret);


        }




        [HttpPost("validate-2fa")]
        [CustomAuthorize(AppPermissions.Pages_General_Data)]
        public async Task<IActionResult> ValidateTwoFactor([FromBody] TwoFactorDto twoFactorDto)
        {
            if (twoFactorDto == null || string.IsNullOrEmpty(twoFactorDto.Username) || string.IsNullOrEmpty(twoFactorDto.Code))
            {
                return BadRequest("Datos inválidos.");
            }

            // Obtener al usuario desde tu repositorio o servicio
            var user = await _userAppServices.GetUserByUsername(twoFactorDto.Username);
            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            try
            {
                var tokenResponse = await _jw.ValidateTwoFactorAndGenerateToken(user, twoFactorDto.Code);
                return Ok(tokenResponse);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(ex.Message);
            }
        }







    }


}
