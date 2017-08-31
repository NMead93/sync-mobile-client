using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCLStorage;
using Xamarin.Forms;
using SyncMobileClient.Data;
using Plugin.Connectivity;

namespace SyncMobileClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string path = FileSystem.Current.LocalStorage.Path + "/Customer2.db3";
            LocalDb.Initialize(path);

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            base.OnStart();
            CrossConnectivity.Current.ConnectivityChanged += async (sender, args) =>
            {
                await RestService.SyncWithAzure();
            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
