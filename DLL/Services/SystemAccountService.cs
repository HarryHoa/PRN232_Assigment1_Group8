using Azure;
using Common.Dto;
using DAL.Models;
using DLL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DLL.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly FUNewsManagementContext _context;

        public SystemAccountService(FUNewsManagementContext context)
        {
            _context = context;
        }

       
        
        public async Task<ResponseDto> Login(string email, string password)
        {
            var account = _context.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email && a.AccountPassword == password);
            if (account == null)
            {
                return new ResponseDto(
                    statusCode: 404,
                    message: "Account not found",
                    isSuccess: false,
                    result: null
                );
            }
            if (string.IsNullOrEmpty(account.AccountEmail) || string.IsNullOrEmpty(account.AccountPassword))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email or password cannot be empty",
                    isSuccess: false,
                    result: null
                );
            }
            string mailValidate = @"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$";
            if (!Regex.IsMatch(account.AccountEmail, mailValidate))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email must be FUNewsManagement",
                    isSuccess: false,
                    result: null
                );
            }
            var accountDto = new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountEmail = account.AccountEmail,
                AccountPassword = account.AccountPassword,
                AccountName = account.AccountName,
                AccountRole = account.AccountRole
            };
            return new ResponseDto(
                statusCode: 200,
                message: "Login successful",
                isSuccess: true,
                result: accountDto
            );


            // Additional logic for successful login can be added here.
        }

        public async Task<ResponseDto> ResgisterUser(SystemAccountDto account)
        {
            var acc =  new SystemAccount
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountPassword = account.AccountPassword,
                AccountRole = account.AccountRole
            };
            _context.SystemAccounts.Add(acc);
            _context.SaveChanges();
            var accDto = new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountPassword = account.AccountPassword,
                AccountRole = account.AccountRole
            };
            return new ResponseDto(
                 statusCode: 201,
                 message: "Account created successfully",
                 isSuccess: true,
                 result: accDto
             );
        }
    }
}
