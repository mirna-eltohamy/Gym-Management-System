using GymManagementSystem.BLL.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct);

        Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);

        Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync(CancellationToken ct = default);

    }
}
