using Microsoft.AspNetCore.Mvc;
using RedisOMDBTask.MVCApp.Models;
using StackExchange.Redis;
using System.Diagnostics;

namespace RedisOMDBTask.MVCApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMoviePosters()
        {
            try
            {
                var muxer = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "redis-15253.c277.us-east-1-3.ec2.redns.redis-cloud.com", 15253 } },
                User = "default",
                Password = "PRgipOSCveFVseB7vo4GX043N1A2tTCa",
            }
        );

                var db = muxer.GetDatabase();
                var list = db.ListRange("OMDBMovies", 0, -1);

                var posterUrls = list.Select(item => item.ToString()).ToList();
                return Json(posterUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie posters from Redis.");
                return StatusCode(500, "An error occurred while fetching movie posters.");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
