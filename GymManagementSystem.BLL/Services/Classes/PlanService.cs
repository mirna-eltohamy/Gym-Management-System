using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _planRepository;
        private readonly IGenericRepository<Membership> _membershipRepository;

        public PlanService(IGenericRepository<Plan> planRepository,
            IGenericRepository<Membership> membershipRepository) 
        {
            _planRepository = planRepository;
            _membershipRepository = membershipRepository;
        }

      

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(ct:ct);

            var plansViewModel = plans.Select(plan => new PlanViewModel()
            {
               Id = plan.Id,
               Name = plan.Name,
               Description = plan.Description,
               DurationDays = plan.DurationDays,
               Price = plan.Price,
               IsActive = plan.IsActive,
            });

            return plansViewModel;
        }
        public async Task<PlanViewModel> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);

            var model = new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                IsActive = plan.IsActive
            };
            return model;
        }
        public async Task<PlanEditViewModel?> GetPlanToEditAsync(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);

            ////Check for active memberships
            var activeMembership = await _membershipRepository.AnyAsync(m => m.EndDate > DateTime.UtcNow && m.PlanId == plan.Id);
            if (activeMembership) return null;

            var model = new PlanEditViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                DurationDays = plan.DurationDays,
                Price = plan.Price,
                Description = plan.Description
            };

            return model;
        }
        public async Task<bool> EditPlanAsync(int id, PlanEditViewModel model, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id);

            //Plan Unchanged
            if (plan.DurationDays == model.DurationDays &&
            plan.Price == model.Price &&
            plan.Description == model.Description)
                return false;
           
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.Description = model.Description;

            var count = await _planRepository.UpdateAsync(plan);

            return count>0;
        }
        public async Task<bool> ActivatePlanAsync(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);

            if(plan.IsActive)
            {
                var activeMembership = await _membershipRepository.AnyAsync(m => m.EndDate > DateTime.UtcNow && m.PlanId == plan.Id);
                if (activeMembership) return false;
                plan.IsActive = false;
                
            }
            else
            {
                plan.IsActive = true;
            }
            var count = await _planRepository.UpdateAsync(plan, ct);

            return count>0;
        }
    }
}
