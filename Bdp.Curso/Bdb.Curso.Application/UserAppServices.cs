using AutoMapper;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bdb.Curso.Application
{
    public class UserAppServices : IUserAppServices
    {

        private readonly IGenericRepository<User> _usersRepository;
        private readonly IMapper _mapper;

        private readonly IPasswordHasherService _hassher;

        public UserAppServices(IGenericRepository<User> usersRepository, IMapper mapper
            , IPasswordHasherService hassher)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
            _hassher = hassher;

        }

        public async Task<UserDto> Login(LoginModel login)
        {
            //var passHass =string.Empty;

            var userDb = await _usersRepository.Where(u => u.UserName == login.UserName).FirstOrDefaultAsync();

            if (userDb == null)
                return new UserDto();

            var validarPass = _hassher.VerifyPassword(login.Password ?? string.Empty, userDb.Password);

            if (!validarPass)
                return new UserDto();

            return _mapper.Map<UserDto>(userDb);

        }



        public async Task<UserDto> CreateUser(CreateUserInput input)
        {
            var userDb = _mapper.Map<User>(input);

            userDb.Password = _hassher.HashPassword(input.Password ?? string.Empty);

            userDb.TwoFactorExpiry = DateTime.Now;
            userDb.TwoFactorCode = "";

            await _usersRepository.AddAsync(userDb);

            return _mapper.Map<UserDto>(userDb);

        }

        public async Task<bool> UpdateUser(int id, CreateUserInput input)
        {
            var userDb = await _usersRepository.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (userDb == null)
                return false;

            var userUpd = userDb;
            userUpd.Name = input.Name ?? string.Empty;
            userUpd.Email = input.Email ?? string.Empty;
            userUpd.Password = _hassher.HashPassword(input.Password ?? string.Empty);
            userUpd.Roles = input.Roles ?? string.Empty;

            try
            {
                await _usersRepository.UpdateAsync(userUpd);
            }
            catch  
            {

            }

            return true;

        }



        public async Task<UserDto> GetUserByUsername(string username)
        {
            var ret = new UserDto();

            var userByUserName = await _usersRepository.Where(u => u.UserName == username).FirstOrDefaultAsync();

            if (userByUserName != null)
            {
                ret = new UserDto
                {
                    Id = userByUserName.Id,
                    UserName = userByUserName.UserName ?? string.Empty,
                    Email = userByUserName.Email ?? string.Empty,
                    Roles = userByUserName.Roles ?? string.Empty,
                    TwoFactorCode = userByUserName.TwoFactorCode ?? string.Empty ,
                    TwoFactorExpiry = userByUserName.TwoFactorExpiry 

                };
            }

            return ret;


        }



    }
}
