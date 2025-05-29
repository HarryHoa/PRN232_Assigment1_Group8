using Microsoft.AspNetCore.Mvc;
using PRNN232_Assigment1_FE.Models;
using System.Text;
using System.Text.Json;

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

                var response = await _httpClient.PostAsync("SystemAccount/login", content);

                if (response != null && response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Mật khẩu hoặc Mail kh đúng hãy thử lại ";

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Login fail, please check your account";

                return View(model);
            }

        }
           
    }

    public class ResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
    }
}
