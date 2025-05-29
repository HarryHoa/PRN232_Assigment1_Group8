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
                // ✅ Sử dụng POST request với JSON body
                var loginData = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("SystemAccount/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // API trả về success thì đăng nhập thành công
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorMessage = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Email hoặc mật khẩu không đúng!",
                        System.Net.HttpStatusCode.BadRequest => "Dữ liệu không hợp lệ!",
                        _ => "Đăng nhập thất bại!"
                    };

                    ModelState.AddModelError("", errorMessage);
                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối server: {ex.Message}");
                return View(model);
            }
            catch (TaskCanceledException ex)
            {
                ModelState.AddModelError("", "Timeout khi kết nối server!");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi không xác định: {ex.Message}");
                return View(model);
            }
        }
    }
}