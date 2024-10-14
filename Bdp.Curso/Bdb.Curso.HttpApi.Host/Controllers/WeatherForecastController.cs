using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Bdb.Curso.HttpApi.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase

    {
        private readonly TelemetryClient _telemetryClient;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

     

        public WeatherForecastController(  TelemetryClient telemetryClient)
        {
             
            _telemetryClient = telemetryClient;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var i = 0;
            var y = 10;
            var x = 0;
            try
            {
                
               x  = y / i;
            }
            catch (Exception ee)
            {
                Log.Error(ee,"Error consumo de datos del cliente");

                _telemetryClient.TrackException(ee);
            }
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
