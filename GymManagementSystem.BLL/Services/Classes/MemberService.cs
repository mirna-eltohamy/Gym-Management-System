using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Email==model.Email, ct);
            if (emailExists) return Result.Validation("Failed to add member! Email already exists");

            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Phone==model.Phone, ct);
            if (phoneExists) return Result.Validation("Failed to add member! Phone Number already exists");


            var member = _mapper.Map<Member>(model);

            _unitOfWork.GetRepository<Member>().Add(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0? Result.OK() : Result.Fail("Failed to create member");
        }

        public async Task<Result> DeleteMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if(member == null) return Result.NotFound($"Member with Id {memberId} not found");

            var hasFutureSessions = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate> DateTime.UtcNow , ct);
            if(hasFutureSessions) return Result.Validation("Failed to remove member! Member has future sessions");

            _unitOfWork.GetRepository<Member>().Delete(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0? Result.OK():Result.Fail("Failed to remove member!");
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            var membersViewModel = _mapper.Map<IEnumerable<MemberViewModel>>(members);

            return membersViewModel;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            var model = _mapper.Map<MemberViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.Id == memberId && m.EndDate>DateTime.UtcNow, ct);
            if(activeMembership is not null)
            {
                model.PlanName = activeMembership.Plan.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
                model.MembershipEndDate = activeMembership.EndDate.ToString();

            }
           
            return model;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct)
        {
            var healthRecord = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(h => h.MemberId==memberId, ct);

            if (healthRecord is null) return null;

            var model = _mapper.Map<HealthRecordViewModel>(healthRecord);

            return model;
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            var model = _mapper.Map<MemberToUpdateViewModel>(member);

            return model;
        }

        public async Task<Result> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return Result.NotFound($"Member with Id {memberId} not found");

            //Check if email|phone already exist
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id!=memberId, ct);
            if (emailExists) return Result.Validation("Failed to update member! Email already exists");

            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);
            if (phoneExists) return Result.Validation("Failed to update member! Phone Number already exists");

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;


            _unitOfWork.GetRepository<Member>().Update(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);
            
            return count>0 ? Result.OK() : Result.Fail("Failed to update member") ;
        }
    }
}
