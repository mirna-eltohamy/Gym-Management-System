using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagementSystem.PL
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticService _analyticService;

        public HomeController(IAnalyticService analyticService) 
        {
            _analyticService = analyticService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _analyticService.GetAnalyticsAsync(ct);
            return View(result);
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
}
