using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaftLabs.ExternalUserService.Extensions;
using RaftLabs.ExternalUserService.Services;

namespace RaftLabs.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddExternalUserService(context.Configuration);
                })
                .Build();

            var userService = host.Services.GetRequiredService<IExternalUserService>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting External User Service Demo");

                // Test getting a single user
                Console.WriteLine("=== Testing GetUserByIdAsync ===");
                var user = await userService.GetUserByIdAsync(2);
                if (user != null)
                {
                    Console.WriteLine($"User: {user.FullName} ({user.Email})");
                }
                else
                {
                    Console.WriteLine("User not found");
                }

                // Test getting a page of users
                Console.WriteLine("\n=== Testing GetUsersPageAsync ===");
                var pageUsers = await userService.GetUsersPageAsync(1);
                Console.WriteLine($"Page 1 Users ({pageUsers.Count()}):");
                foreach (var u in pageUsers)
                {
                    Console.WriteLine($"  - {u.FullName} ({u.Email})");
                }

                // Test getting all users
                Console.WriteLine("\n=== Testing GetAllUsersAsync ===");
                var allUsers = await userService.GetAllUsersAsync();
                Console.WriteLine($"Total Users: {allUsers.Count()}");

                // Test caching by calling the same method again
                Console.WriteLine("\n=== Testing Caching (calling GetAllUsersAsync again) ===");
                var allUsersAgain = await userService.GetAllUsersAsync();
                Console.WriteLine($"Total Users (from cache): {allUsersAgain.Count()}");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during demo execution");
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}