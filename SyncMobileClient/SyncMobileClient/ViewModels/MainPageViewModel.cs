using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyncMobileClient.Data;
using SyncMobileClient.Models;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Connectivity;

namespace SyncMobileClient.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public List<UserPreference> PreferenceList { get; set; }
        public string PreferenceName { get; set; }
        public string PreferenceValue { get; set; }
        public ICommand AddPreferenceCommand { get; set; }
        public ICommand NukeEverythingCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            PreferenceList = new List<UserPreference>();
            PreferenceName = "";
            PreferenceValue = "";
            NukeEverythingCommand = new Command(NukeEverything);
            AddPreferenceCommand = new Command(AddPreference);
        }

        public async Task PopulatePreferenceList()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                await RestService.SyncWithAzure();
            }

            var localListResults = await PreferenceLocalDb.Instance.GetAllPreferences();
            PreferenceList = localListResults.ToList();
        }

        public async void NukeEverything()
        {
            await PreferenceLocalDb.Instance.NukeTables();
        }

        private async void AddPreference()
        {
            UserPreference newCustomer = new UserPreference("Customer", PreferenceName, PreferenceValue);
            if (CrossConnectivity.Current.IsConnected)
            {
                //call restservice
                await RestService.AddOrDeletePreference(newCustomer, "Add");
            }
            await PreferenceLocalDb.Instance.AddPreference(newCustomer);

            await PopulatePreferenceList();

            OnPropertyChanged("PreferenceList");
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
