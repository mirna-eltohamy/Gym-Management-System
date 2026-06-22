using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainers;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var model = await _trainerService.GetAllTrainersAsync(ct);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model,CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var result = await _trainerService.CreateTrainerAsync(model, ct);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Trainer Added Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.error;
                    return RedirectToAction("Index");

                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var model = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (model == null)
            {
                TempData["ErrorMessage"] = $"Trainer with Id {id} not found";
                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerToUpdateAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Trainer not found";
                return RedirectToAction("Index");
            }
            return View(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if(ModelState.IsValid)
            {
                var result = await _trainerService.UpdateTrainerAsync(id, model, ct);
                if(result.success)
                {
                    TempData["SuccessMessage"] = "Trainer updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.error;
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> delete(int id, CancellationToken ct)
        {
            var result = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Trainer not found";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.DeleteTrainerAsync(id, ct);
            if(result)
            {
                TempData["SuccessMessage"] = "Trainer deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to remove trainer! Trainer has scheduled sessions" ;
            }
            return RedirectToAction("Index");
        }



    }
}
