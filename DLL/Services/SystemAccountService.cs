using Azure;
using Common;
using Common.Dto;
using Common.SettingJWT;
using DAL.Models;
using DLL.Interface;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email or password cannot be empty",
                    isSuccess: false,
                    result: null
                );
            }
            string mailValidate = @"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$";
            if (!Regex.IsMatch(email, mailValidate))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email must be FUNewsManagement",
                    isSuccess: false,
                    result: null
                );
            }
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

        public async Task<ResponseDto> LoginJWT(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email or password cannot be empty",
                    isSuccess: false,
                    result: null
                );
            }
            string mailValidate = @"^[a-zA-Z0-9._%+-]+@FUNewsManagement\.org$";
            if (!Regex.IsMatch(email, mailValidate))
            {
                return new ResponseDto(
                    statusCode: 400,
                    message: "Email must be FUNewsManagement",
                    isSuccess: false,
                    result: null
                );
            }
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.AccountName ?? ""),
                new Claim(ClaimTypes.Email, account.AccountEmail ?? ""),
                new Claim(ClaimTypes.Role, account.AccountRole.Value.ToString() ?? "")
            };
            var accessTk = generateAccessToken(claims);
            var refeshTk = generateRefeshToken(claims);
            var response = new JwtResponse
            {
                accessToken = accessTk,
                refeshToken = refeshTk,
            };
            return new ResponseDto(
                statusCode: 200,
                message: "Login successful",
                isSuccess: true,
                result: response
            );
        }
        private string generateAccessToken(List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettingModel.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(JwtSettingModel.ExpireDayAccessToken),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = JwtSettingModel.Issuer,
                Audience = JwtSettingModel.Audience


            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private string generateRefeshToken(List<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettingModel.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(JwtSettingModel.ExpireDayRefreshToken),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = JwtSettingModel.Issuer,
                Audience = JwtSettingModel.Audience


            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
