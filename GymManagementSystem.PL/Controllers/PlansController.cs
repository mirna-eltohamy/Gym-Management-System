using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.PL
{
    [Authorize(Roles = "SuperAdmin")]
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);

            return View(plans); 
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanByIdAsync(id, ct);
            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToEditAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Cannot update plan with active memberships!";
                return RedirectToAction("Index");
            }
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit (int id, PlanEditViewModel model, CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var result = await _planService.EditPlanAsync(id, model, ct);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Plan updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["InfoMessage"] = result.error;
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ActivatePlanAsync(id, ct);
            if(result)
            {
                TempData["SuccessMessage"] = "Plan status changed";
            }
            else
            {
                TempData["ErrorMessage"] = "Cannot deactivate plan with active memberships";
            }
            return RedirectToAction("Index");
        }


    }
}
