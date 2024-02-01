using System.Reflection;
using Caching.Repository.Extensions;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(ops =>
{
	ops.Configuration = builder.Configuration.GetConnectionString("Redis");
	ops.InstanceName = $"{Assembly.GetExecutingAssembly().GetName().Name}_";
});

var app = builder.Build();

app.MapGet("/weatherforecast", async (IDistributedCache cache) =>
{
	var key = $"WeatherForecast_{DateTime.Now.Date.ToString("yyyyMMdd_hhmm")}";
	var forecast = await cache.GetRecordAsync<WeatherForecast[]>(key);

	if (forecast is not null) return forecast;

	forecast = ListWeatherForecast();
	await cache.SetRecordAsync(key, forecast);

	return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

WeatherForecast[] ListWeatherForecast()
{
	var summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	var weatherForecasts = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast
			(
				DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				Random.Shared.Next(-20, 55),
				summaries[Random.Shared.Next(summaries.Length)]
			))
		.ToArray();
	return weatherForecasts;
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
