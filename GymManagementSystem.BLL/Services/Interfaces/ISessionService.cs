using GymManagementSystem.BLL.ViewModels.Sessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct);

        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);

        Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync(CancellationToken ct = default);

        Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct);

        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default);

        Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default);

    }
}
