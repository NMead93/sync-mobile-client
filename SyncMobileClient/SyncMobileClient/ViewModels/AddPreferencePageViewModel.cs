using Plugin.Connectivity;
using SyncMobileClient.Data;
using SyncMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SyncMobileClient.ViewModels
{
    public class AddPreferencePageViewModel
    {
        public string PreferenceName { get; set; }
        public string PreferenceValue { get; set; }
        public ICommand AddPreferenceCommand { get; set; }
        public ICommand NukeEverythingCommand { get; set; }
        public INavigation Navigation { get; set; }

        public AddPreferencePageViewModel()
        {
            PreferenceName = "";
            PreferenceValue = "";
            NukeEverythingCommand = new Command(NukeEverything);
            AddPreferenceCommand = new Command(AddPreference);
        }

        public async void NukeEverything()
        {
            await LocalDb.Instance.NukeTables();
            await Navigation.PopAsync();
        }

        private async void AddPreference()
        {
            UserPreference newPreference = new UserPreference("Customer", PreferenceName, PreferenceValue);

            await newPreference.Create();

            await Navigation.PopAsync();

        }
    }
}
