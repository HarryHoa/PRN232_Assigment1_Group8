﻿using Common.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PRNN232_Assigment1_FE.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace PRNN232_Assigment1_FE.Controllers
{
        public class LoginController : Controller
        {
            private readonly HttpClient _httpClient;

            public LoginController(HttpClient httpClient)
            {
                _httpClient = httpClient;
                _httpClient.BaseAddress = new Uri("https://localhost:7252/api/");
            }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginData = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("SystemAccount/login_jwt", content);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Mật khẩu hoặc Mail không đúng, hãy thử lại";
                    return View(model);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiWrapper = JsonSerializer.Deserialize<ApiResponse<JwtResponse>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiWrapper?.Result == null)
                {
                    TempData["ErrorMessage"] = "Login fail, please check your account";
                    return View(model);
                }

                var jwt = apiWrapper.Result.AccessToken;

                // Decode JWT để lấy thông tin người dùng (như AccountId, Role, Email...)
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);

                var claims = new List<Claim>();

                foreach (var claim in token.Claims)
                {
                    // CHUYỂN "role" thành ClaimTypes.Role
                    if (claim.Type == "role")
                        claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                    else
                        claims.Add(claim);
                }



                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

               
                var role = claims.FirstOrDefault(c => c.Type == "role")?.Value;

                Console.WriteLine("User role from token: " + role); 

                if (role == "3" || role == "Admin")
                {
                    return RedirectToAction("Index", "AdminAccountMvc");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Login fail, please check your account";
                return View(model);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Index(LoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //        try
        //        {
        //            var loginData = new
        //            {
        //                email = model.Email,
        //                password = model.Password
        //            };

        //        var jsonContent = JsonSerializer.Serialize(loginData);
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PostAsync("SystemAccount/login_jwt", content);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            TempData["ErrorMessage"] = "Mật khẩu hoặc Mail không đúng, hãy thử lại";
        //            return View(model);
        //        }

        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var apiWrapper = JsonSerializer.Deserialize<ApiResponse<SystemAccountDto>>(responseContent, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        });
        //        if (apiWrapper?.Result == null)
        //        {
        //            TempData["ErrorMessage"] = "Login fail, please check your account";
        //            return View(model);
        //        }
        //        var apiResponse = apiWrapper.Result;


        //        // Tạo claims để lưu thông tin user
        //        var claims = new List<Claim>
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, apiResponse.AccountId.ToString()),
        //                new Claim(ClaimTypes.Name, apiResponse.AccountName ?? ""),
        //                new Claim(ClaimTypes.Email, apiResponse.AccountEmail ?? ""),
        //                new Claim(ClaimTypes.Role, apiResponse.AccountRole == 1 ? "Staff" : apiResponse.AccountRole == 2 ? "Lecturer" : "Admin")

        //            };

        //        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);

        //        // Cấu hình authentication properties để đảm bảo cookie được lưu
        //        var authProperties = new AuthenticationProperties
        //        {
        //            IsPersistent = true, 
        //            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) 
        //        };

        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            principal,
        //            authProperties);
        //        Console.WriteLine(apiResponse);
        //        if (apiResponse.AccountRole == 3) 
        //        {
        //            return RedirectToAction("Index", "AdminAccountMvc");
        //        }
        //        else // Staff hoặc role khác
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Login fail, please check your account";
        //        return View(model);
        //    }
        //}
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    return RedirectToAction("Login", "Account");
        //}

        public async Task<IActionResult> Forbidden()
        {
            return View();
        }
    }
}
        

  
