using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(P => P.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(P => P.Description)
                .HasColumnType("varchar")
                .HasMaxLength(200);

            builder.Property(P => P.Price)
                .HasPrecision(10, 2);

            builder.Property(P => P.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationDaysCheck", "DurationDays Between 1 and 365");
            });

            builder.HasData(
                   new Plan { Id = 1, Name = "Base Plan", Description="Access to gym equipment during staffed hours", DurationDays=30,Price=300 },
                   new Plan { Id = 2, Name = "Standard Plan", Description="Includes gym equipment and 2 group classes per week", DurationDays=60,Price=500 },
                   new Plan { Id = 3, Name = "Premium Plan", Description="Unlimited access to equipment, classes and sauna", DurationDays=90,Price=900 },
                   new Plan { Id = 4, Name = "Annual Plan", Description= "Full year access with personal trainer sessions", DurationDays=365,Price=3000 }
                  );

        }
    }
}
