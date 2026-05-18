using System.Text.Json;

namespace TechMoveApp.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                // Directly using your dedicated ExchangeRate-API key for pair conversion
                string apiKey = "570f88ea8d41dd2e838304bd";
                string url = $"https://v6.exchangerate-api.com/v6/{apiKey}/pair/USD/ZAR";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(jsonString);

                    // The standard key format returns the target value inside 'conversion_rate'
                    if (doc.RootElement.TryGetProperty("conversion_rate", out var rateElement))
                    {
                        return rateElement.GetDecimal();
                    }
                }
            }
            catch
            {
                // Realistic fallback rate to prevent crashes if the API faces downtime
                return 18.75m;
            }

            return 18.75m;
        }
    }
}