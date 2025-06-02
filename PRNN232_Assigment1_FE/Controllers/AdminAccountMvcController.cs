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

      if (!string.IsNullOrWhiteSpace(keyword))
      {
          response = await _client.GetAsync($"AdminCrudAccount/search?keyword={keyword}");
      }
      else
      {
          response = await _client.GetAsync("AdminCrudAccount");
      }

      if (!response.IsSuccessStatusCode)
      {
          TempData["Error"] = "Failed to fetch account list.";
          return View(new List<AdminAccountViewModel>());
      }

      var apiResult = await response.Content.ReadFromJsonAsync<ResponseDto>();
      var data = System.Text.Json.JsonSerializer.Serialize(apiResult.Result);
      var accounts = System.Text.Json.JsonSerializer.Deserialize<List<AdminAccountViewModel>>(data, new JsonSerializerOptions
      {
          PropertyNameCaseInsensitive = true
      });

      return View(accounts);
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            await _client.DeleteAsync($"AdminCrudAccount/{id}");
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
            return RedirectToAction("Index");
        }
    }


}
