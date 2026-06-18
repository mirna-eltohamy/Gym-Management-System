using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService) 
        {
            _sessionService = sessionService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var model = await _sessionService.GetAllSessionsAsync(ct);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct),"Id","Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetAllCategoriesForDropDownAsync(ct),"Id","CategoryName");
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var result = await _sessionService.CreateSessionAsync(model, ct);
                if (result)
                {
                    TempData["SuccessMessage"] = "Session Created Successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create session!";
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

    }
}
