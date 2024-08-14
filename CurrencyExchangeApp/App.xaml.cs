using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace CurrencyExchangeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            string theme = Preferences.Get("theme", "light");
            ApplyTheme(theme);
            MainPage = new NavigationPage(new MainPage());
        }

        public void SetTheme(string theme)
        {
            Preferences.Set("theme", theme);
            ApplyTheme(theme);
        }

        private void ApplyTheme(string theme)
        {
            if (theme == "dark")
            {
                Resources["PageBackgroundStyle"] = new Style(typeof(ContentPage))
                {
                    Setters = { new Setter { Property = ContentPage.BackgroundColorProperty, Value = "#2C2C2C" } }
                };
                Resources["EntryStyle"] = (Style)Resources["DarkModeEntryStyle"];
                Resources["ButtonStyle"] = (Style)Resources["DarkModeButtonStyle"];
                Resources["ResultFrameStyle"] = (Style)Resources["DarkModeFrameStyle"];
                Resources["ResultLabelStyle"] = (Style)Resources["DarkModeLabelStyle"];
            }
            else
            {
                Resources["PageBackgroundStyle"] = new Style(typeof(ContentPage))
                {
                    Setters = { new Setter { Property = ContentPage.BackgroundColorProperty, Value = "#F5F5F5" } }
                };
                Resources["EntryStyle"] = (Style)Resources["LightModeEntryStyle"];
                Resources["ButtonStyle"] = (Style)Resources["LightModeButtonStyle"];
                Resources["ResultFrameStyle"] = (Style)Resources["LightModeFrameStyle"];
                Resources["ResultLabelStyle"] = (Style)Resources["LightModeLabelStyle"];
            }
        }
    }
}