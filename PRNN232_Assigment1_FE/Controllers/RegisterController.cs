using Microsoft.AspNetCore.Mvc;
using PRNN232_Assigment1_FE.Models;
using System.Text;
using System.Text.Json;

namespace PRNN232_Assigment1_FE.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegisterController(HttpClient httpClient)
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
        public async Task<IActionResult>Index(SystemAccountModel accountModel)
        {
            if (!ModelState.IsValid)
            {
                return View(accountModel);
            }
            try
            {
                var accountData = new 
                {
                    accountId = accountModel.AccountId,
                    accountName = accountModel.AccountName,
                    accountEmail = accountModel.AccountEmail,
                    accountPassword = accountModel.AccountPassword,
                    accountRole = accountModel.AccountRole,


                };
                var jsonContent = JsonSerializer.Serialize(accountData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("SystemAccount/register", content);
                if (response != null && response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Registration failed, please try again.";
                    return View(accountModel);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during registration: " + ex.Message;
            }
            return View(accountModel);

        }
    }
}
