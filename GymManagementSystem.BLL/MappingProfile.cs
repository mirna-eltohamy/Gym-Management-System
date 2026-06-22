using AutoMapper;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.BLL.ViewModels.Sessions;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            MapMember();
            MapPlan();
            MapTrainer();
            MapSession();
        }

        private void MapMember()
        {
            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(d => d.Address, o => o.MapFrom(s => new Address()
                {
                    BuildingNumber = s.BuildingNumber,
                    Street = s.Street,
                    City = s.City
                }))
                .ForMember(d => d.HealthRecord, o => o.MapFrom(s => new HealthRecord()
                {
                    BloodType = s.HealthRecordViewModel.BloodType,
                    Weight = s.HealthRecordViewModel.Weight,
                    Height = s.HealthRecordViewModel.Height,
                    Note = s.HealthRecordViewModel.Note
                }));

            CreateMap<Member, MemberViewModel>()
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateOfBirth.ToShortDateString()))
                .ForMember(d => d.Address, o => o.MapFrom(s => $"{s.Address.BuildingNumber} - {s.Address.Street} - {s.Address.City} "));

            CreateMap<HealthRecord, HealthRecordViewModel>();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(d => d.BuildingNumber, o => o.MapFrom(s => s.Address.BuildingNumber))
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City));


        }
        private void MapPlan() 
        {
            CreateMap<Plan, PlanViewModel>();

            CreateMap<Plan, PlanEditViewModel>();

        }
        private void MapTrainer()
        {
            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(d => d.Address, o => o.MapFrom(s => new Address()
                {
                    BuildingNumber = s.BuildingNumber,
                    Street = s.Street,
                    City = s.City
                }));

            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(d => d.Address, o => o.MapFrom(s => $"{s.Address.BuildingNumber} - {s.Address.Street} - {s.Address.City} "))
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.DateOfBirth.ToShortDateString()))
                .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Specialty, o => o.MapFrom(s => s.Specialty.ToString()));

            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(d => d.BuildingNumber, o => o.MapFrom(s => s.Address.BuildingNumber))
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City));

        }
        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(d=>d.TrainerName, o=>o.MapFrom(s=>s.Trainer.Name))
                .ForMember(d=>d.CategoryName, o=>o.MapFrom(s=>s.Category.CategoryName));

            CreateMap<CreateSessionViewModel, Session>();

            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();

            CreateMap<Session, UpdateSessionViewModel>();

        }

    }
}
