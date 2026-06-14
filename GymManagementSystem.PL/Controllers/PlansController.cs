using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.PL
{
    public class PlansController : Controller
    {
        //Open connection with DB with each obj
        //public readonly GymDbContext _context =new GymDbContext();

        public readonly IPlanRepository _planRepository;

        //public PlansController() 
        //{
        //    _planRepository = new PlanRepository(); 
        //}

        //DI - ask CLR to inject inject obj from class implementing IPlanRepository
        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        //Index
        // GET: /Plans/Index ----> List all plans
        public async Task<IActionResult> Index()
        {
            //var plans= await _context.Plans.ToListAsync();

            var plans = await _planRepository.GetAllAsync();

            //View() returns rz page with same action name - Index
            //View(plans) - sends model represents plans in DB
            return View(plans); 
        }


        //Details
        // GET: /Plans/Details/Id
        public async Task<IActionResult> Details(int id)
        {
            //var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);

            var plan = await _planRepository.GetByIdAsync(id);

            if (plan is null) return RedirectToAction("Index");

            return View(plan);
        }


    }
}
