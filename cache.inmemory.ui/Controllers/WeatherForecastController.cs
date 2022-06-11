using cache.inmemory.ui.Services;
using Microsoft.AspNetCore.Mvc;

namespace cache.inmemory.ui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {        

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService weatherForecastService)
        {
            _logger = logger;
            this._weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        [Route("getconcurrent")]
        public async Task<IActionResult> GetConcurrent()
        {
            var result = await this._weatherForecastService.GetAllCitiesConcurrent();

            return Ok(result);
        }

        [HttpGet]
        [Route("getbasic")]
        public async Task<IActionResult> GetBasic()
        {
            var result = await this._weatherForecastService.GetAllCitiesBasic();

            return Ok(result);
        }
    }
}