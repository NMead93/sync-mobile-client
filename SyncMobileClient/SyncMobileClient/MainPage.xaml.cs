using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SyncMobileClient.ViewModels;
using SyncMobileClient.Data;
using SyncMobileClient.Models;
using System.Diagnostics;

namespace SyncMobileClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var newDetailsPage = new Details();
            var newDetailsViewModel = new DetailsViewModel((UserPreference)e.SelectedItem);
            newDetailsViewModel.Navigation = Navigation;
            newDetailsPage.BindingContext = newDetailsViewModel;

            try
            {
                await Navigation.PushAsync(newDetailsPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override async void OnAppearing()
        {
            MainPageViewModel viewModel = new MainPageViewModel();
            BindingContext = viewModel;
            await viewModel.PopulatePreferenceList();
            PreferenceList.ItemsSource = viewModel.PreferenceList;
        }
    }
}
