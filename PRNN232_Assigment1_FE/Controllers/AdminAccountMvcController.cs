using Common.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRNN232_Assigment1_FE.Models;
using System.Text.Json;

namespace PRNN232_Assigment1_FE.Controllers
{
    [Authorize(Roles = "3")]
    public class AdminAccountMvcController : Controller
    {
        private readonly HttpClient _client;

        public AdminAccountMvcController(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost:7252/api/");
        }

        public async Task<IActionResult> Index(string keyword, int page = 1, int pageSize = 5)
        {
            ViewBag.Keyword = keyword;

            var url = $"AdminCrudAccount/paged?pageIndex={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                url += $"&keyword={keyword}";
            }

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to fetch paged account list.";
                return View(new PaginatedListViewModel<AdminAccountViewModel>
                {
                    Items = new List<AdminAccountViewModel>(),
                    TotalItems = 0,
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    Keyword = keyword
                });
            }

            var stream = await response.Content.ReadAsStreamAsync();

            var pagedResult = await System.Text.Json.JsonSerializer.DeserializeAsync<BasePaginatedListDto<AdminAccountViewModel>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (pagedResult == null)
            {
                TempData["Error"] = "Không thể đọc dữ liệu phân trang từ API.";
                return View(new PaginatedListViewModel<AdminAccountViewModel>
                {
                    Items = new List<AdminAccountViewModel>(),
                    TotalItems = 0,
                    CurrentPage = page,
                    TotalPages = 0,
                    PageSize = pageSize,
                    Keyword = keyword
                });
            }

            return View(new PaginatedListViewModel<AdminAccountViewModel>
            {
                Items = pagedResult.Items.ToList(),
                TotalItems = pagedResult.TotalItems,
                CurrentPage = pagedResult.CurrentPage,
                TotalPages = pagedResult.TotalPages,
                PageSize = pagedResult.PageSize,
                Keyword = keyword
            });

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
