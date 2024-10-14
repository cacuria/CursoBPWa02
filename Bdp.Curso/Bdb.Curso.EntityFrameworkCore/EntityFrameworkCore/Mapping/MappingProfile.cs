using AutoMapper;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;


namespace Bdb.Curso.EntityFrameworkCore.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();

            CreateMap<User, CreateUserInput>().ReverseMap();

            CreateMap<CreateUserInput, UserDTO>().ReverseMap();

          //  CreateMap<ProductMovRequest, ProductMovRequestCommand>().ReverseMap();

        }


    }
}
