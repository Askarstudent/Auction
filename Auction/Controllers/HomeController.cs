using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using User.Models;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly List<AuctionLot> _lots = new List<AuctionLot>();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;

        _lots = new List<AuctionLot>
        {
            new AuctionLot { Id = 1, Name = "Lot 1", Description = "Description of lot 1", CurrentPrice = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(7) },
            new AuctionLot { Id = 2, Name = "Lot 2", Description = "Description of lot 2", CurrentPrice = 200, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(7) },
            new AuctionLot { Id = 3, Name = "Lot 3", Description = "Description of lot 3", CurrentPrice = 300, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(7) }
        };
    }

    public IActionResult Index(string culture)
    {
        if (!string.IsNullOrWhiteSpace(culture))
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);
        }

        _logger.LogInformation("Retrieved list of auction lots");
        return View(_lots);
    }

    [AllowAnonymous]
    public IActionResult SignUp()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string login, string password, string returnUrl)
    {
        if (login == "admin" && password == "admin")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        ModelState.AddModelError("", "Login or password is incorrect.");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult AddLot(AuctionLot lot)
    {
        lot.Id = _lots.Any() ? _lots.Max(l => l.Id) + 1 : 1;
        _lots.Add(lot);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditLot(AuctionLot lot)
    {
        var existingLot = _lots.FirstOrDefault(l => l.Id == lot.Id);
        if (existingLot != null)
        {
            existingLot.Name = lot.Name;
            existingLot.Description = lot.Description;
            existingLot.CurrentPrice = lot.CurrentPrice;
            existingLot.StartTime = lot.StartTime;
            existingLot.EndTime = lot.EndTime;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteLot(int id)
    {
        var lotToRemove = _lots.FirstOrDefault(l => l.Id == id);
        if (lotToRemove != null)
        {
            _lots.Remove(lotToRemove);
        }
        return RedirectToAction("Index");
    }
}
