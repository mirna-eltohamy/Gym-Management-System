using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);

            return View(members);
        }

        [HttpGet]
        public IActionResult Create() 
        {  
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberViewModel model, CancellationToken ct)
        {
            if(ModelState.IsValid) // Server-side Validation
            {
                var result = await _memberService.CreateMemberAsync(model, ct);
                if(result)
                {
                    TempData["SuccessMessage"] = "Member Created Successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create member!";
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsAsync(id, ct);
            if(result is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction("Index");
            }
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Health record not found";
                return RedirectToAction("Index");
            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberToUpdateAsync(id, ct);

            if (result is null)
            {
                TempData["ErrorMessage"] = "Member not found!";
                return RedirectToAction("Index");
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.UpdateMemberAsync(id, model, ct);

                if(result)
                {
                    TempData["SuccessMessage"] = "Member updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update member";
                    return RedirectToAction("Index");
                }
            }

            //in case model state invalid -> stay (no redirection)
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> delete(int id, CancellationToken ct)
        {
            var result = await _memberService.GetMemberDetailsAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete member";
            }
            return RedirectToAction("Index");
        }



    }
}
