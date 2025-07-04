﻿using Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Interface
{
   public interface ISystemAccountService
    {
         Task<ResponseDto> Login(string userName, string password);
        Task<ResponseDto> LoginJWT(string userName, string password);
        Task<ResponseDto> ResgisterUser(SystemAccountDto account);

    }
}
