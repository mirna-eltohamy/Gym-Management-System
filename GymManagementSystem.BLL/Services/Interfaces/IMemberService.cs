using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct);
        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct);
        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct);
        Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct);
        Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct);
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct);


    }
}
