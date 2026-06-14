using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Trainer : GymUser
    {
        public Specialty Specialty { get; set; }
    }
}
