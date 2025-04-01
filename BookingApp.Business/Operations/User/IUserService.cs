using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Business.Operations.User.Dtos;
using BookingApp.Business.Types;

namespace BookingApp.Business.Operations.User
{
    public interface IUserService
    {
        Task<ServiceMessage> AddUser(AddUserDto user); // async çünkü unit of work kullanılacak

        ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user);
    }
}
