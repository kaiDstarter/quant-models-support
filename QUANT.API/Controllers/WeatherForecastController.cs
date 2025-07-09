using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace QUANT.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpPost]
        public async Task<IActionResult> Capture(CaptureData captureData)
        {
            IFormFile file = captureData.preparedImage;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Tạo zip trong memory
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                var zipEntry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);
                using var entryStream = zipEntry.Open();
                using var fileStream = file.OpenReadStream();
                await fileStream.CopyToAsync(entryStream);
            }

            zipStream.Seek(0, SeekOrigin.Begin); // Đưa con trỏ về đầu stream
            string directory = Path.Combine(Environment.CurrentDirectory, "uploads");
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string savePath = Path.Combine(directory, "image.zip");
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok();
            //// Trả về file zip cho client
            //var zipFileName = Path.GetFileNameWithoutExtension(file.FileName) + ".zip";
            //return File(zipStream.ToArray(), "application/zip", zipFileName);
        }
    }
    public class CaptureData
    {
        //public byte[] preparedImage { get; set; }
        public IFormFile preparedImage { get; set; }
        public string language { get; set; }
        public string timezone { get; set; }
        public string symbol { get; set; }
    }
}
