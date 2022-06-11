namespace cache.inmemory.ui.Services
{
    public interface IWeatherForecastService
    {
        Task<List<string>> GetAllCitiesBasic();
        Task<List<string>> GetAllCitiesConcurrent();
    }

}
