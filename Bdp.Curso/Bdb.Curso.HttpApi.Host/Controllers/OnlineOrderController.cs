using AutoMapper;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.HttpApi.Host.Authorization;
using Bdb.Curso.HttpApi.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
 

namespace Bdb.Curso.HttpApi.Host.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OnlineOrderController : ControllerBase
    {
        private readonly IInvAppServices _invAppServices;

        private readonly CacheService _cacheService;


        public OnlineOrderController(IInvAppServices invAppServices, CacheService cacheService)
        {                           
            _invAppServices = invAppServices;
            _cacheService = cacheService;

        }
                                   

        // GET: api/lista-productos
        [HttpGet("lista-productos")]

       // [CustomAuthorize(AppPermissions.Pages_Query_Products)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts([FromQuery] 
        GetProductRequest input)
        {
            //Manejo del caching
            string cacheKey = "GetProducts";
            var dataChache = _cacheService.Get<List<ProductDTO>>(cacheKey);

            List<ProductDTO> data;

            if(dataChache != null)
            {
                data = dataChache;
            }
            else
            {
                data = await _invAppServices.GetProducts(input.searchTerm, input.pageNumber);

                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(1));

            }
                                

            if (data.Count == 0)
                return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
            //return Ok(data);

        }





        // GET: api/lista-productos
        [HttpGet("lista-productos10")]

        //[CustomAuthorize(AppPermissions.Pages_Query_Products)]
        //[Authorize("Usuarios")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts10([FromQuery]
        GetProductRequest input)
        {
            //Manejo del caching
            string cacheKey = "GetProducts";
            var dataChache = _cacheService.Get<List<ProductDTO>>(cacheKey);

            List<ProductDTO> data;

            if (dataChache != null)
            {
                data = dataChache;
            }
            else
            {
                data = await _invAppServices.GetProducts(input.searchTerm, input.pageNumber);

                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(1));

            }


            if (data.Count == 0)
                return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
            //return Ok(data);

        }





        [HttpPost("Registro")]
        public async Task<IActionResult> InventMov([FromBody] ProductMovRequest request)
        {
            var data = await _invAppServices.InventMov(request);
          
            return Ok(data);
        }
                                   

        //[HttpPost("RegistroSp")]
        //public async Task<IActionResult> InventMovSp([FromBody] ProductMovRequest request)
        //{
        //    var data =await _invAppServices.InventMovSp(request);
        //    return Ok(data);
        //}



    }



}
