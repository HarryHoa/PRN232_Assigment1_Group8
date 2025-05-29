using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PRNN232_Assigment1_FE.Models;

namespace PRNN232_Assigment1_FE.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        //if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        //{
        //    return RedirectToAction("Login", "Account");
        //}

        //ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
