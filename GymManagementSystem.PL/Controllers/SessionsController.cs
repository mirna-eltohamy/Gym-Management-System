using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
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
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Session Created Successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = result.error;
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if(!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction("Index");
            }

            return View(result.value);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
                return View(result.value);
            }

            TempData["ErrorMessage"] = result.error;
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _sessionService.UpdateSessionAsync(id, model, ct);

            if(result.success)
            {
                TempData["SuccessMessage"] = "Session updated successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
                TempData["ErrorMessage"] = result.error;
                return View(model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if(result.success)
            {
                return View();
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.DeleteSessionAsync(id, ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Session removed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction("Index");

        }


    }
}
