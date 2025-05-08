using Graduation_Project.Data;
using Graduation_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SeedDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting database seeding...");

            try
            {
                // Build configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Graduation Project/appsettings.json")
                    .Build();

                // Get connection string
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                Console.WriteLine($"Using connection string: {connectionString}");

                // Create DbContext options
                var optionsBuilder = new DbContextOptionsBuilder<GraduationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                // Create context
                using (var context = new GraduationDbContext(optionsBuilder.Options))
                {
                    Console.WriteLine("Database context created. Starting seeding...");
                    SeedData.Initialize(context);
                    Console.WriteLine("Database seeding completed successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
