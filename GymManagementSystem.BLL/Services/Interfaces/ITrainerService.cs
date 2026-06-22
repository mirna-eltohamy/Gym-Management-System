using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Trainers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct);
        Task<TrainerToUpdateViewModel> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct);
        Task<Result> UpdateTrainerAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct);
        Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct);
    }
}
