using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
