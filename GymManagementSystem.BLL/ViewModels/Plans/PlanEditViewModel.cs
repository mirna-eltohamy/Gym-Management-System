using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Plans
{
    public class PlanEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Required(ErrorMessage ="Description is required!")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Max 200 charachters")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Duration days required!")]
        [Range(1, 365, ErrorMessage = "Plan Duration from 1 to 365 days")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price is required!")]
        public decimal Price { get; set; }

        public Membership? Membership { get; set; }
    }
}
