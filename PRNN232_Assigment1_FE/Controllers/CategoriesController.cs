using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Common.Dto.CategoryDto;
using System.Text.Json;
using Common.Dto.CategoryDTO;
using System.Text;
using System.Net.Http;

namespace PRNN232_Assigment1_FE.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly FUNewsManagementContext _context;
        private readonly HttpClient _httpClient;

        public CategoriesController(FUNewsManagementContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7252/api/");
        }

      //GET
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Category"); // Gọi endpoint API lấy danh sách

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();

                // Giải mã JSON thành list category DTO
                var categories = JsonSerializer.Deserialize<List<CategoryResponseDto>>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(categories);
            }
            else
            {
                // Xử lý lỗi
                TempData["Error"] = "Không thể lấy dữ liệu từ API.";
                return View(new List<CategoryResponseDto>());
            }
        }

        // GET: Categories/Details/5
       
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Gọi API lấy chi tiết category
                var response = await _httpClient.GetAsync($"/api/Category/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var category = JsonSerializer.Deserialize<CategoryResponseDto>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (category == null)
                        return NotFound();

                    return View(category);
                }
                else
                {
                    // Nếu API không trả về thành công
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi gọi API
                TempData["ErrorMessage"] = $"Lỗi khi lấy dữ liệu category: {ex.Message}";
                return View();
            }
        }


        //// GET: Categories/Create
        //public IActionResult Create()
        //{
        //    ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption");
        //    return View();
        //}

        //// POST: Categories/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
        //    return View(category);
        //}
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách categories để đổ dropdown (có thể gọi API GET hoặc lấy từ DB nếu có sẵn)
            // Giả sử bạn có sẵn trong _context hoặc gọi API
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dto.ParentCategoryId);
                return View(dto);
            }

            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(dto),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("Category", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Tạo category thất bại, mã lỗi: " + response.StatusCode);
                    ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dto.ParentCategoryId);
                    return View(dto);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gọi API: " + ex.Message);
                ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", dto.ParentCategoryId);
                return View(dto);
            }
        }



        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
            return View(category);
        }




        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
