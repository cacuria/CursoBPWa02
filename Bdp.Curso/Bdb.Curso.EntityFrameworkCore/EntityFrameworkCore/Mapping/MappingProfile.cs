using AutoMapper;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;


namespace Bdb.Curso.EntityFrameworkCore.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<User, CreateUserInput>().ReverseMap();

            CreateMap<CreateUserInput, UserDto>().ReverseMap();

          //  CreateMap<ProductMovRequest, ProductMovRequestCommand>().ReverseMap();

        }


    }
}
