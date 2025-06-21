using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRNN232_Assigment1_FE.Models;
using System.Net.Http;
using System.Text.Json;

namespace PRNN232_Assigment1_FE.Controllers
{
    [Authorize(Roles =("Admin"))]
    public class AdminAccountMvcController : Controller
    {
        private readonly HttpClient _client;

        public AdminAccountMvcController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7252/api/");
        }

        public async Task<IActionResult> Index(string keyword)
        {
            ViewBag.Keyword = keyword;

            HttpResponseMessage response;

            var query = "odata/AdminCrudAccount?$count=true";

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                // Giả sử lọc theo tên
                query += $"&$filter=contains(tolower(AccountName),'{keyword.ToLower()}')";
            }

            response = await _client.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch account list.";
                return View(new List<AdminAccountViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var odataResult = JsonConvert.DeserializeObject<ODataResponse<List<AdminAccountViewModel>>>(json);

            ViewBag.TotalCount = odataResult.Count;

            return View(odataResult.Value);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminAccountViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new AdminCRUDdto
            {
                AccountId = vm.AccountId,
                AccountName = vm.AccountName,
                AccountEmail = vm.AccountEmail,
                AccountPassword = vm.AccountPassword,
                AccountRole = vm.AccountRole,
            };

            var response = await _client.PostAsJsonAsync("AdminCrudAccount", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to create account.";
                return View(vm);
            }

            TempData["Success"] = "Account created successfully!";
            return RedirectToAction("Index");


        }

        [HttpGet]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _client.GetAsync($"AdminCrudAccount/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch account for deletion.";
                return RedirectToAction("Index");
            }

            var dto = JsonConvert.DeserializeObject<ResponseDto>(await response.Content.ReadAsStringAsync());
            var account = JsonConvert.DeserializeObject<AdminAccountViewModel>(dto.Result.ToString());

            return View(account); // => Trả về Delete.cshtml
        }



        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(short accountId)
        {
            var response = await _client.DeleteAsync($"AdminCrudAccount/{accountId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to delete account.";
            }
            else
            {
                TempData["Success"] = "Account deleted successfully!";
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(short id)
        {
            var res = await _client.GetAsync($"AdminCrudAccount/{id}");
            var dto = JsonConvert.DeserializeObject<ResponseDto>(await res.Content.ReadAsStringAsync());
            var data = JsonConvert.DeserializeObject<AdminAccountViewModel>(dto.Result.ToString());

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminAccountViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new AdminCRUDdto
            {
                AccountId = vm.AccountId,
                AccountName = vm.AccountName,
                AccountEmail = vm.AccountEmail,
                AccountPassword = vm.AccountPassword,
                AccountRole = vm.AccountRole,
            };

            await _client.PutAsJsonAsync($"AdminCrudAccount/{vm.AccountId}", dto);
            TempData["Success"] = "Account updated successfully!";
            return RedirectToAction("Index");

        }
    }


}
