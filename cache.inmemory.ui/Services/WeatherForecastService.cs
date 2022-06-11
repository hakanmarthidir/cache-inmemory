using Microsoft.Extensions.Caching.Memory;
namespace cache.inmemory.ui.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IMemoryCache _memoryCache;
        const string CACHE_CITY_LIST = "citylist";

        private static readonly SemaphoreSlim GetCitiesSemaphore = new SemaphoreSlim(1, 1);
        public WeatherForecastService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private async Task<List<string>> GetCitiesAsync()
        {
            var result = await Task.Run<List<string>>(() =>
            {
                return new List<string>() { "Istanbul", "Stuttgart", "Esslingen", "Berlin", "Frankfurt", "Mannheim", "Tübingen" };

            });
            return result;
        }

        public async Task<List<string>> GetAllCitiesBasic()
        {
            if (!_memoryCache.TryGetValue(CACHE_CITY_LIST, out List<string> cities))
            {
                cities = await this.GetCitiesAsync();

                //Expiration Policies : Absolute or Sliding or Both
                //if you want you can apply SetSize(2048)
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.Normal)
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                this._memoryCache.Set(CACHE_CITY_LIST, cities, cacheEntryOptions);
            }

            return cities;

        }

        public async Task<List<string>> GetAllCitiesConcurrent()
        {
            return await this.GetAllCitiesSemaphore(GetCitiesSemaphore);
        }

        private async Task<List<string>> GetAllCitiesSemaphore(SemaphoreSlim semaphore)
        {
            List<string> cities = null;

            _memoryCache.TryGetValue(CACHE_CITY_LIST, out cities);
            if (cities != null) return cities;

            try
            {
                await semaphore.WaitAsync();

                _memoryCache.TryGetValue(CACHE_CITY_LIST, out cities);
                if (cities != null) return cities;

                cities = await this.GetCitiesAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.Normal)
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                this._memoryCache.Set(CACHE_CITY_LIST, cities, cacheEntryOptions);

            }
            finally
            {
                semaphore.Release();
            }

            return cities;
        }

      
    }

}
