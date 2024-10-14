using AutoMapper;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 

namespace Bdb.Curso.HttpApi.Host.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersImproveController : ControllerBase
    {

        //variabla privada
        private readonly IGenericRepository<User> _usersRepository;
        private readonly IMapper _mapper;

        public UsersImproveController(IGenericRepository<User> usersRepository, IMapper mapper) //injectarlo
        {
            //cargo en la privada
            _usersRepository = usersRepository;
            _mapper = mapper;

        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1)
        {
            // Mejoras con filtros y pagineo
                               
            int pageSize = 1; // definir como politica

            // var lista = await _usersRepository.GetAllAsync(); // listado completo

            //query
            var query = _usersRepository
                                .Where(f => f.Name.Contains(searchTerm));

            //ejecucion del query con el conteo
            var resultCount = await query.CountAsync();
             
            //ejecucion del query con el pagineo
            var lista = await query
                                .Skip(pageNumber -1)
                                .Take(pageSize)
                                .ToListAsync();
                                                          

            //conversion entre la entidad y la dto.... => mapper
            // Proyectar la entidad a el resultado

            var salida = _mapper.Map<IEnumerable<UserDTO>>(lista);
                              

            return Ok(salida);

        }




    }
}
