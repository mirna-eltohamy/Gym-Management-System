using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        public Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct);
        public Task<PlanViewModel> GetPlanByIdAsync(int id, CancellationToken ct);
        public Task<PlanEditViewModel?> GetPlanToEditAsync(int id, CancellationToken ct);
        public Task<Result> EditPlanAsync(int id, PlanEditViewModel model , CancellationToken ct);
        public Task<bool> ActivatePlanAsync(int id, CancellationToken ct);


    }
}
