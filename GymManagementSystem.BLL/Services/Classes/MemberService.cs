using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<Membership> _membershipRepository;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;

        public MemberService(IGenericRepository<Member> memberRepository, 
            IGenericRepository<Membership> membershipRepository,
            IGenericRepository<HealthRecord> healthRecordRepository,
            IGenericRepository<Booking> bookingRepository) 
        {
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _healthRecordRepository = healthRecordRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            //check Email
            //check Phone number
            //Map from ViewModel to Model
            //Add to DB

            //var members = await _memberRepository.GetAllAsync(ct:ct);
            //var emailExists= members.Any(m=>m.Email==model.Email);
            //var phoneExists= members.Any(m=>m.Phone==model.Phone);

            var emailExists = await _memberRepository.AnyAsync(m=>m.Email==model.Email, ct);
            var phoneExists = await _memberRepository.AnyAsync(m=>m.Phone==model.Phone, ct);

            if(emailExists || phoneExists ) return false;

            var member = new Member()
            {
                Name=model.Name,
                Email=model.Email,  
                Phone=model.Phone,  
                Gender=model.Gender,
                Address=new Address() 
                {
                    BuildingNumber=model.BuildingNumber,
                    Street=model.Street,
                    City=model.City,
                },
                HealthRecord = new HealthRecord()
                {
                    Weight=model.HealthRecordViewModel.Weight,
                    Height=model.HealthRecordViewModel.Height,  
                    BloodType=model.HealthRecordViewModel.BloodType,
                    Note=model.HealthRecordViewModel.Note,
                }
            };

            var count =  await _memberRepository.AddAsync(member,ct);

            return count > 0;
        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepository.GetByIdAsync(memberId, ct);
            if(member == null) return false;

            //var healthRecordDeleted = await _healthRecordRepository.DeleteAsync(member.HealthRecord, ct);

            //var membershipDeleted = await _membershipRepository.DeleteAsync(member.)

            var hasFutureSessions = await _bookingRepository.AnyAsync(b => b.MemberId == memberId && b.Session.StartDate> DateTime.UtcNow , ct);
            if(hasFutureSessions) return false;

            var count = await _memberRepository.DeleteAsync(member, ct);

            return count > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);

            //Map from model to viewmodel

            //Manual Mapping

            //var membersViewModel = new List<MemberViewModel>();

            //foreach (var member in members)
            //{
            //    var memberViewModel = new MemberViewModel()
            //    {
            //        Id = member.Id,
            //        Name = member.Name,
            //        Photo = member.Photo,
            //        Phone = member.Phone,
            //        Email = member.Email,
            //        Gender = member.Gender.ToString()

            //    };
            //    membersViewModel.Add(memberViewModel);
            //}

            var membersViewModel = members.Select(member => new MemberViewModel()
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Phone = member.Phone,
                Email = member.Email,
                Gender = member.Gender.ToString()
            });

            return membersViewModel;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepository.GetByIdAsync(memberId, ct);

            var model = new MemberViewModel()
            {
                Photo = member.Photo,
                Name=member.Name,
                Phone = member.Phone,
                Email = member.Email,
                Gender = member.Gender.ToString(),
                DateOfBirth=member.DateOfBirth.ToString(),
                Address=$"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City} "
            };

            var activeMembership = await _membershipRepository.FirstOrDefaultAsync(m => m.Id == memberId && m.EndDate>DateTime.UtcNow, ct);
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
            var healthRecord = await _healthRecordRepository.FirstOrDefaultAsync(h => h.MemberId==memberId, ct);

            if (healthRecord is null) return null;

            var model = new HealthRecordViewModel()
            {
                Height = healthRecord.Height,
                Weight = healthRecord.Weight,
                BloodType = healthRecord.BloodType,
                Note = healthRecord.Note
            };

            return model;
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepository.GetByIdAsync(memberId, ct);

            var model = new MemberToUpdateViewModel
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                Photo = member.Photo,
                BuildingNumber = member.Address.BuildingNumber,
                Street = member.Address.Street,
                City = member.Address.City

            };

            return model;
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _memberRepository.GetByIdAsync(memberId, ct);
            if (member is null) return false;

            //Check if email|phone already exist
            var emailExists = await _memberRepository.AnyAsync(m => (m.Email == model.Email) && m.Id != member.Id, ct);
            var phoneExists = await _memberRepository.AnyAsync(m => (m.Phone == model.Phone)&&m.Id!=member.Id, ct);

            if (emailExists || phoneExists) return false;

            //Map - model -> DB
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.Address.City = model.City;


            //Update
            var count = await _memberRepository.UpdateAsync(member, ct);
            
            return count>0;
        }
    }
}
