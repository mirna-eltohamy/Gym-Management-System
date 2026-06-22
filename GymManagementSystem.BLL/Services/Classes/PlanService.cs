using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork,
                IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

      

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);

            var plansViewModel = _mapper.Map<IEnumerable<PlanViewModel>> (plans);

            return plansViewModel;
        }
        public async Task<PlanViewModel> GetPlanByIdAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            var model = _mapper.Map<PlanViewModel>(plan);

            return model;
        }
        public async Task<PlanEditViewModel?> GetPlanToEditAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);

            ////Check for active memberships
            var activeMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.EndDate > DateTime.UtcNow && m.PlanId == plan.Id);
            if (activeMembership) return null;

            var model = _mapper.Map<PlanEditViewModel>(plan);

            return model;
        }
        public async Task<Result> EditPlanAsync(int id, PlanEditViewModel model, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id);
            if (plan is null) return Result.NotFound($"Plan with Id {id} not found");

            //Plan Unchanged
            if (plan.DurationDays == model.DurationDays &&
            plan.Price == model.Price &&
            plan.Description == model.Description)
                return Result.Fail("Plan Unchanged!");

            plan.DurationDays = model.DurationDays;
            plan.Description = model.Description;
            plan.Price = model.Price;


            _unitOfWork.GetRepository<Plan>().Update(plan);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0? Result.OK() : Result.Fail("Failed to edit plan");
        }
        public async Task<bool> ActivatePlanAsync(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);


            if(plan.IsActive)
            {
                var activeMembership = await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.EndDate > DateTime.UtcNow && m.PlanId == plan.Id);
                if (activeMembership) return false;
                plan.IsActive = false;
                
            }
            else
            {
                plan.IsActive = true;
            }

            _unitOfWork.GetRepository<Plan>().Update(plan);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count>0;
        }
    }
}
