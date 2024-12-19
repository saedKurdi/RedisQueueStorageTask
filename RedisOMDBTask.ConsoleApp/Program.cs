using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisOMDBTask.MVCApp.Services.Abstract;
using RedisOMDBTask.MVCApp.Services.Concrete;

namespace RedisOMDBTask.ConsoleApp
{
    public class Program
    {
        private readonly IQueueService _queueService;

        public Program(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Write("Write a movie name you want to search: ");
                var movieName = Console.ReadLine();
                await WriteMovieNameToQueue(movieName);
            }
        }

        private async Task WriteMovieNameToQueue(string? movieName)
        {
            if (string.IsNullOrEmpty(movieName) || string.IsNullOrWhiteSpace(movieName))
            {
                Console.WriteLine("Do not enter empty data!");
            }
            else if (!IsValidUTF8(movieName))
            {
                Console.WriteLine("The input contains invalid UTF-8 characters!");
            }
            else
            {
                await _queueService.SendMessageAsync(movieName);
            }
        }

        private bool IsValidUTF8(string text)
        {
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                var decodedText = System.Text.Encoding.UTF8.GetString(bytes);
                return text == decodedText;
            }
            catch
            {
                return false;
            }
        }


        // Entry point
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    // Register services
                    services.AddScoped<IQueueService>(qs => new QueueService("DefaultEndpointsProtocol=https;AccountName=omdbmovies;AccountKey=pe3WpayamqnibQVqb9lpgjzTKgm+4A4AEckXFDEREJKZ2pd3bd6N3jF1j4G996Jwz7Z7B5WMudsl+AStduaQAA==;BlobEndpoint=https://omdbmovies.blob.core.windows.net/;QueueEndpoint=https://omdbmovies.queue.core.windows.net/;TableEndpoint=https://omdbmovies.table.core.windows.net/;FileEndpoint=https://omdbmovies.file.core.windows.net/;", "omdbmovienames"));
                    services.AddScoped<Program>();
                })
                .Build();

            // Resolve the Program class and run the application
            var program = host.Services.GetRequiredService<Program>();
            await program.RunAsync();
        }
    }
}
