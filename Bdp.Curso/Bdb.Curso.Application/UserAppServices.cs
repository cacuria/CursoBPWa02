using AutoMapper;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application
{
    public class UserAppServices: IUserAppServices
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

        public async Task<UserDTO> Login(LoginModel login)
        {                  
             //var passHass =string.Empty;

             //passHass = _hassher.HashPassword (login.Password);
             
            var userDb = await _usersRepository.Where(u => u.UserName == login.UserName ).FirstOrDefaultAsync();

            if (userDb == null  )
                return null;

            var validarPass = _hassher.VerifyPassword(login.Password, userDb.Password );

            if (!validarPass)
                return null;
                                         
            return _mapper.Map<UserDTO>(userDb);  

        }

 

        public async Task<UserDTO> CreateUser(CreateUserInput input)
        {
            var userDb = _mapper.Map<User>(input);

            userDb.Password = _hassher.HashPassword(input.Password);

            userDb.TwoFactorExpiry = DateTime.Now;
            userDb.TwoFactorCode = "";

            await _usersRepository.AddAsync(userDb);

            return _mapper.Map<UserDTO>(userDb);

        }

        public async Task<bool> UpdateUser(int id, CreateUserInput input)
        {
            var userDb = await _usersRepository.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (userDb == null)
                return false;
            
            var userUpd =  userDb ;
            userUpd.Name = input.Name;
            userUpd.Email = input.Email;
            userUpd.Password = _hassher.HashPassword(input.Password);
            userUpd.Roles = input.Roles;

            try
            {
                await _usersRepository.UpdateAsync(userUpd);
            }
            catch (Exception eex)
            {
                          
            }

            return true;
                                                     
        }



        public async Task<UserDTO> GetUserByUsername(string username)
        {
            var ret = new UserDTO();

            var userByUserName = await _usersRepository.Where(u => u.UserName == username).FirstOrDefaultAsync();

            if (userByUserName != null)
            {
                ret = new UserDTO
                {
                    Id = userByUserName.Id,
                    UserName = userByUserName.UserName,
                    Email = userByUserName.Email,
                    Roles = userByUserName.Roles,
                    TwoFactorCode = userByUserName.TwoFactorCode,
                    TwoFactorExpiry = userByUserName.TwoFactorExpiry

                };
            }

            return ret;


        }



    }
}
