using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RedisOMDBTask.MVCApp.Dtos;
using StackExchange.Redis;
using System.Text.Json;

namespace QueueTriggerApp
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("QueueTrigger1")]
        public async Task Run([QueueTrigger("omdbmovienames", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            // sending request to omdb getting movie's poster and writing it to redis :
            var httpClient = new HttpClient();
            var response = new HttpResponseMessage();

            response = await httpClient.GetAsync($"https://www.omdbapi.com/?apikey=f32dcb14&s={message.MessageText}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OMDBRepsonseDto>(content);

            var movie = result.Search[0];
            Console.WriteLine(movie.Year + " " + movie.Title + " " + movie.Poster);

            var muxer = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { "redis-15253.c277.us-east-1-3.ec2.redns.redis-cloud.com", 15253 } },
                    User = "default",
                    Password = "PRgipOSCveFVseB7vo4GX043N1A2tTCa"
                }
            );
            var db = muxer.GetDatabase();
            db.ListRightPush("OMDBMovies", movie.Poster);
        }
    }
}
