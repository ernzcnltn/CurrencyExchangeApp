using System;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyExchangeApp
{
    public partial class MainPage : ContentPage
    {
        private const string ApiUrl = "https://v6.exchangerate-api.com/v6/e6b9238d0f4ca3c6b2d366a3/latest/USD";

        public MainPage()
        {
            InitializeComponent();
            InitializeCurrencyPickers();
        }

        private async void InitializeCurrencyPickers()
        {
            try
            {
                var currencies = await FetchCurrencyCodesAsync();
                fromCurrencyPicker.ItemsSource = currencies;
                toCurrencyPicker.ItemsSource = currencies;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load currencies: {ex.Message}", "OK");
            }
        }

        private async Task<List<string>> FetchCurrencyCodesAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(ApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var rates = JsonConvert.DeserializeObject<ExchangeRates>(responseString);

                        if (rates?.ConversionRates != null)
                        {
                            return new List<string>(rates.ConversionRates.Keys);
                        }
                        else
                        {
                            throw new Exception("Currency codes not available.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to fetch data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while fetching data: {ex.Message}", "OK");
                    return new List<string>(); 
                }
            }
        }

        private async Task<double> GetConversionRateAsync(string fromCurrency, string toCurrency)
        {
            if (string.IsNullOrEmpty(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                throw new ArgumentException("Currency codes cannot be null or empty.");

            string apiUrl = $"https://v6.exchangerate-api.com/v6/e6b9238d0f4ca3c6b2d366a3/latest/{fromCurrency}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var rates = JsonConvert.DeserializeObject<ExchangeRates>(responseString);

                        if (rates?.ConversionRates != null && rates.ConversionRates.ContainsKey(toCurrency))
                        {
                            return rates.ConversionRates[toCurrency];
                        }
                        else
                        {
                            throw new Exception("Currency not supported or conversion rate not available.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to fetch data. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred while fetching conversion rate: {ex.Message}", "OK");
                    return 0; 
                }
            }
        }

        private async void OnConvertClicked(object sender, EventArgs e)
        {
            try
            {
                var amountText = amountEntry.Text;
                if (double.TryParse(amountText, out var amount))
                {
                    var fromCurrency = fromCurrencyPicker.SelectedItem?.ToString();
                    var toCurrency = toCurrencyPicker.SelectedItem?.ToString();

                    if (fromCurrency == null || toCurrency == null)
                    {
                        resultLabel.Text = "Please select both currencies";
                        return;
                    }

                    var rate = await GetConversionRateAsync(fromCurrency, toCurrency);
                    if (rate == 0)
                    {
                        resultLabel.Text = "Conversion rate not available.";
                        return;
                    }

                    var convertedAmount = amount * rate;
                    resultLabel.Text = $" {amount} {fromCurrency} is {convertedAmount} {toCurrency}";
                }
                else
                {
                    resultLabel.Text = "Invalid amount entered";
                }
            }
            catch (Exception ex)
            {
                resultLabel.Text = $"Error: {ex.Message}";
            }
        }


        private async void OnViewCurrencyChangesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CurrencyChangesPage());
        }

        private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
        {
            string theme = e.Value ? "dark" : "light";
            (Application.Current as App)?.SetTheme(theme);
        }

        public class ExchangeRates
        {
            [JsonProperty("conversion_rates")]
            public Dictionary<string, double>? ConversionRates { get; set; }
        }
    }
}