using GymManagementSystem.BLL.Services.Attachment;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
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
                if(result.success)
                {
                    TempData["SuccessMessage"] = "Member Created Successfully!";
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
        public async Task<IActionResult> Picture(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null || string.IsNullOrWhiteSpace(member.Photo)) return NotFound();

            var result = _attachmentService.GetFile("MembersPictures", member.Photo);
            if (result is null) return NotFound();

            return File(result.Value.stream, result.Value.contentType);
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

                if(result.success)
                {
                    TempData["SuccessMessage"] = "Member updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = result.error;
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
            if (result.success)
            {
                TempData["SuccessMessage"] = "Member deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] =  result.error;
            }
            return RedirectToAction("Index");
        }



    }
}
