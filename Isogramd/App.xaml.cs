using Isogramd.Account;
using Isogramd.Util;
using Xamarin.Forms;

namespace Isogramd
{
    public partial class App : Application
    {
        public static Page FirstPage;
        private static UserData dataStore;

        public static string AppName = "Isogramd";

        public App()
        {
            InitializeComponent();

			System.Diagnostics.Debug.WriteLine("Loading app");
            dataStore = new UserData();
            System.Diagnostics.Debug.WriteLine("Done loading");
            Launch();
            //App.Get_Data_Store().Store("queue_for", "practice");
            //App.FirstPage = new Isogramd.Function.QueueExperience();
            //MainPage = App.FirstPage;
        }

        public void Launch()
        {
			ConnectionExperience ce = new ConnectionExperience();
			App.FirstPage = new NavigationPage(ce);
			MainPage = App.FirstPage;
			ce.NavInDelay(3000);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static Page GetMainPage()
        {
            return App.FirstPage;
        }

        public static INavigation GetNavigation()
        {
            return App.FirstPage.Navigation;
        }

        public static UserData Get_Data_Store()
        {
            return dataStore;
        }
    }
}
