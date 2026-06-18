using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository,
            IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct)
        {
            var emailExists = await _trainerRepository.AnyAsync(t => t.Email == model.Email, ct);
            var phoneExists = await _trainerRepository.AnyAsync(t => t.Phone == model.Phone, ct);

            if (emailExists || phoneExists) return false;

            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Specialty = model.Specialty,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City,
                }

            };
            var count = await _trainerRepository.AddAsync(trainer, ct);

            return count>0;
        }

        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);

            var scheduledSession = await _sessionRepository.AnyAsync(s=>s.TrainerId==trainerId && s.StartDate > DateTime.Now, ct);
            if(scheduledSession) return false;

            var count = await _trainerRepository.DeleteAsync(trainer);

            return count>0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);


            var trainersViewModel = trainers.Select(trainer => new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone   = trainer.Phone,
                Specialty = trainer.Specialty.ToString()
             
            });

            return trainersViewModel;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);

            var trainerModel = new TrainerViewModel()
            {
                Name = trainer.Name,
                Specialty = $"{trainer.Specialty.ToString()} Trainer",
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };

            return trainerModel;

        }

        public async Task<TrainerToUpdateViewModel> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);

            var model = new TrainerToUpdateViewModel()
            {
                id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Specialty = trainer.Specialty
            };

            return model;
        }

        public async Task<bool> UpdateTrainerAsync(TrainerToUpdateViewModel model, CancellationToken ct)
        {
            var emailExists = await _trainerRepository.AnyAsync(t => t.Email == model.Email && t.Id!=model.id, ct);
            var phoneExists = await _trainerRepository.AnyAsync(t => t.Phone == model.Phone && t.Id != model.id, ct);

            if (emailExists || phoneExists) return false;

            var trainer = await _trainerRepository.GetByIdAsync(model.id, ct);

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Specialty = model.Specialty;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            
            var count = await _trainerRepository.UpdateAsync(trainer, ct);

            return count > 0;
        }
    }
}
