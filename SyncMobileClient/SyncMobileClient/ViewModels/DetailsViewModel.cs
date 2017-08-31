using SyncMobileClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Plugin.Connectivity;
using SyncMobileClient.Data;

namespace SyncMobileClient.ViewModels
{
    public class DetailsViewModel
    {
        public UserPreference CurrentPreference { get; set; }
        public string CurrentPreferenceName { get; set; }
        public string NewValue { get; set; }
        public ICommand UpdateValueCommand { get; set; }
        public ICommand DeleteValueCommand { get; set; }
        public INavigation Navigation { get; set; }

        public DetailsViewModel(UserPreference preference)
        {
            CurrentPreference = preference;
            CurrentPreferenceName = preference.PreferenceName;
            UpdateValueCommand = new Command(UpdateValue);
            DeleteValueCommand = new Command(DeleteValue);
        }

        public async void UpdateValue()
        {
            CurrentPreference.PreferenceValue = NewValue;
            await CurrentPreference.Update();

            await Navigation.PopAsync();
        }

        public async void DeleteValue()
        {
            await CurrentPreference.Delete();

            await Navigation.PopAsync();
        }
    }
}
