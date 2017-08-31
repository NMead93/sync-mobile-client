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
using System.Runtime.CompilerServices;

namespace SyncMobileClient.ViewModels
{
    public class MainPageViewModel
    {
        public List<UserPreference> PreferenceList { get; set; }
        public ICommand ExpiredChangesCommand { get; set; }

        public MainPageViewModel()
        {
            PreferenceList = new List<UserPreference>();
            ExpiredChangesCommand = new Command(ChangeToExpired);
        }

        private async void ChangeToExpired()
        {
            await LocalDb.Instance.ExpireLastSync();
        }

        public async Task PopulatePreferenceList()
        {
            await RestService.SyncWithAzure();

            var localListResults = await LocalDb.Instance.GetAll("UserPreference");
            IEnumerable<UserPreference> castedLocalListResults = (IEnumerable<UserPreference>)localListResults;
            PreferenceList = castedLocalListResults.ToList();
        }

        
    }
}
