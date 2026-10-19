using System.Diagnostics;
using HireGameDev.Web.Authorization;
using HireGameDev.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HireGameDev.Web.Controllers;

public sealed class HomeController(IWebHostEnvironment environment) : Controller
{
    [HttpGet]
    public IActionResult Index() =>
        User.HomePath() is { } home
            ? Redirect(home)
            : View();

    [HttpGet]
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
    });

    [HttpGet]
    public IActionResult Styleguide() => environment.IsDevelopment() ? View() : NotFound();
}
