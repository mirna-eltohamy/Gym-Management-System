using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GymManagementSystem.DAL.Data_Seeding
{
    public class GymDataSeeding
    {
        public static async Task SeedAsync<TEntity>(GymDbContext context, string seedFilePath, ILogger logger, CancellationToken ct = default) where TEntity : class
        {
            try
            {
                var seeded = await context.Set<TEntity>().AnyAsync(ct);
                if (seeded) 
                { 
                    logger.LogError($"{typeof(TEntity).Name} table already seeded ");
                    return;
                }

                var data = LoadDataFromJsonFile<TEntity>(seedFilePath);
                if (data.Any())
                {
                    await context.Set<TEntity>().AddRangeAsync(data, ct);
                    var count = await context.SaveChangesAsync(ct);

                    logger.LogInformation($"Seeded {count} {typeof(TEntity).Name}{(count>1?"s":"")} ");

                }
            }
            catch(Exception ex)  
            {
                logger.LogError(ex, ex.Message);
            }
        }

        public static List<T> LoadDataFromJsonFile<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed data file not found: {filePath}");

            var jsonData = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            
            var data  = JsonSerializer.Deserialize<List<T>>(jsonData, options) ?? [];

            return data;
        }
    }
}
