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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SystemAccountModel accountModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all required fields correctly.";
                return View(accountModel);
            }

            try
            {
                var accountData = new
                {
                    AccountId = accountModel.AccountId,
                    AccountName = accountModel.AccountName,
                    AccountEmail = accountModel.AccountEmail,
                    AccountPassword = accountModel.AccountPassword,
                    AccountRole = accountModel.AccountRole // Convert to int if needed
                };

                var jsonContent = JsonSerializer.Serialize(accountData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("SystemAccount/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    ModelState.Clear();
                    TempData["SuccessMessage"] = "Account created successfully!";
                    return View(new SystemAccountModel());
                }
                else
                {
                    // Log chi tiết lỗi
                    System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response: {responseContent}");

                    TempData["ErrorMessage"] = $"Registration failed: {response.StatusCode}";
                    return View(accountModel);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = "Connection error. Please try again.";
                return View(accountModel);
            }
        }
    }
}