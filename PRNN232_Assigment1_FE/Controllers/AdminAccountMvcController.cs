using Common.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRNN232_Assigment1_FE.Models;
using System.Net.Http;

namespace PRNN232_Assigment1_FE.Controllers
{
    public class AdminAccountMvcController : Controller
    {
        private readonly HttpClient _client;

        public AdminAccountMvcController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7252/api/");
        }

        public async Task<IActionResult> Index()
        {
            var res = await _client.GetAsync("AdminCrudAccount");
            var json = await res.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<ResponseDto>(json);
            var data = JsonConvert.DeserializeObject<List<AdminAccountViewModel>>(dto.Result.ToString());

            return View(data);
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
