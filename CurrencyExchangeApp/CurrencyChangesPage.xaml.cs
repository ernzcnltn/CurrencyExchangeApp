using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace CurrencyExchangeApp
{
    public partial class CurrencyChangesPage : ContentPage
    {
        private const string CurrentRatesApiUrl = "https://api.currencyfreaks.com/v2.0/rates/latest?apikey=0252f13cff474ff6b2e83bb207591d57";
        private const string PreviousWeekRatesApiUrl = "https://api.currencyfreaks.com/v2.0/timeseries?apikey=0252f13cff474ff6b2e83bb207591d57&startDate=2024-08-01&endDate=2024-08-13&base=usd";
        private Dictionary<string, double>? previousWeekRates;

        public CurrencyChangesPage()
        {
            InitializeComponent();
            LoadWeeklyChanges();
        }

        private async void LoadWeeklyChanges()
        {
            try
            {
                
                previousWeekRates = await FetchPreviousWeekRatesAsync();

                
                var currentRates = await FetchCurrentRatesAsync();

                if (previousWeekRates != null && currentRates != null)
                {
                   
                    usdChangeLabel.Text = $"USD: {CalculateChange("USD", currentRates)}%";
                    eurChangeLabel.Text = $"EUR: {CalculateChange("EUR", currentRates)}%";
                    gbpChangeLabel.Text = $"GBP: {CalculateChange("GBP", currentRates)}%";
                    btcChangeLabel.Text = $"BTC: {CalculateChange("BTC", currentRates)}%";
                }
                else
                {
                    await DisplayAlert("Error", "No data available.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load weekly changes: {ex.Message}", "OK");
            }
        }

        private double CalculateChange(string currencyCode, Dictionary<string, double> currentRates)
        {
            if (previousWeekRates != null &&
                previousWeekRates.TryGetValue(currencyCode, out double oldRate) &&
                currentRates.TryGetValue(currencyCode, out double newRate))
            {
                return ((newRate - oldRate) / oldRate) * 100;
            }
            return 0;
        }

        private async Task<Dictionary<string, double>> FetchCurrentRatesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(CurrentRatesApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseString);
                        return apiResponse?.Rates ?? new Dictionary<string, double>();
                    }
                    else
                    {
                        throw new Exception($"Failed to fetch current rates. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while fetching current rates: {ex.Message}", "OK");
                    return new Dictionary<string, double>();
                }
            }
        }

        private async Task<Dictionary<string, double>> FetchPreviousWeekRatesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(PreviousWeekRatesApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseString);
                        return apiResponse?.Rates ?? new Dictionary<string, double>();
                    }
                    else
                    {
                        throw new Exception($"Failed to fetch previous week rates. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while fetching previous week rates: {ex.Message}", "OK");
                    return new Dictionary<string, double>();
                }
            }
        }


        public class ApiResponse
        {
            [JsonProperty("rates")]
            public Dictionary<string, double>? Rates { get; set; }
        }
    }
}