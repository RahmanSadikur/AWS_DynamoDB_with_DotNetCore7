using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using System;

namespace dynamodb_sample.Controllers
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
        private readonly IDynamoDBContext _dynamoDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger ,IDynamoDBContext dynamoDbContext)
        {
            _logger = logger;
            _dynamoDbContext = dynamoDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public Task<IEnumerable<WeatherForecast>> Get(string city="Dhaka")
        {
            return WeatherForecasts(city);
        }
        [HttpPost]
        public async Task save(string city)
        {
            var wf = generateSave(city);
            foreach (var item in wf)
            {
                await _dynamoDbContext.SaveAsync<WeatherForecast>(item);
            }

         
        }
        [HttpDelete]
        public async Task delete(string city,string date)
        {
       var item=_dynamoDbContext.LoadAsync<WeatherForecast>(city,date );
       await _dynamoDbContext.DeleteAsync<WeatherForecast>(item);

        }

        private async Task<IEnumerable<WeatherForecast>> WeatherForecasts(string city = "Dhaka")

        {
            return await _dynamoDbContext.QueryAsync<WeatherForecast>(city,QueryOperator.Equal, new[] { "2022-12-14T17:42:34Z" },new DynamoDBOperationConfig()).GetRemainingAsync();
           
        }

        private IEnumerable<WeatherForecast> generateSave(string city)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {  City = city,
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)).ToString("MM/dd/yyyy"),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}