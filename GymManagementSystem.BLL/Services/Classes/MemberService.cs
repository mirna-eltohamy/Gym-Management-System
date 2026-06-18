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

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Email==model.Email, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m=>m.Phone==model.Phone, ct);

            if(emailExists || phoneExists ) return false;

            var member = _mapper.Map<Member>(model);

            _unitOfWork.GetRepository<Member>().Add(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0;
        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if(member == null) return false;

            var hasFutureSessions = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate> DateTime.UtcNow , ct);
            if(hasFutureSessions) return false;

            _unitOfWork.GetRepository<Member>().Delete(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0;
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

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return false;

            //Check if email|phone already exist
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => (m.Email == model.Email) && m.Id != member.Id, ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => (m.Phone == model.Phone)&&m.Id!=member.Id, ct);

            if (emailExists || phoneExists) return false;

            //Map - model -> DB
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;


            //Update
            _unitOfWork.GetRepository<Member>().Update(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);
            
            return count>0;
        }
    }
}
