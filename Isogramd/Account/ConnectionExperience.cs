using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Isogramd.UI.Animated;
using Isogramd.Util;
using Xamarin.Forms;

namespace Isogramd.Account
{
    public partial class ConnectionExperience : ContentPage
    {
        private string SERVER_LIST_IP = "https://raw.githubusercontent.com/Varalore/Isogramd/master/ServerList.txt";
        private Dictionary<string, string> ALL_SERVER_IPS;
        private Random rand;

        AnimatedConnectionIcon aci;

        public ConnectionExperience()
        {

            aci = new AnimatedConnectionIcon("Isogramd.Images.broadcast.png");

            //Before we continue, check if we need to do this at all. Does the phone already have best servers identified?

            BackgroundColor = Color.WhiteSmoke;

            RelativeLayout pageLayout = new RelativeLayout();

            pageLayout.Children.Add(aci, 
            Constraint.RelativeToParent((p) => 
            {
                return ((p.Width / 2) - 30) + (60 * .1543) + 10;
            }), 
            Constraint.RelativeToParent((p) => 
            {
                return (p.Height / 2) - 42;       
            }),
            Constraint.RelativeToParent((p) =>
            {
                return 60;
            }),
            Constraint.RelativeToParent((p) =>
            {
                return 50;
            }));

            this.Content = pageLayout;

            rand = new Random();
        }

		public async Task NavInDelay(int delay)
		{
			await Task.Delay(delay);

			var server = DependencyService.Get<IFileHelper>().Read_String("server_pref.isog");
			if (!server.Equals("ERROR"))
			{
				App.Get_Data_Store().Store("server", server);
				Continue_To_Next_Page();
				return;
            }else{
                await Handle_Initialization();
            }
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
            aci.AnimateDawdle();
		}

        private async Task Handle_Initialization()
		{
            String ip = await Identify_Best_Server();
		    DependencyService.Get<IFileHelper>().Save_String("server_pref.isog", ip);
		    App.Get_Data_Store().Store("server", ip);
            await Continue_To_Next_Page();
        }

        async Task Continue_To_Next_Page() 
        {
			try
			{

				System.Diagnostics.Debug.WriteLine("Nav null?" + (App.GetNavigation() == null));
				await App.GetNavigation().PushAsync
					 (new Isogramd.Account.LoginExperience());

			}
			catch (Exception e)
			{
                System.Diagnostics.Debug.WriteLine(e.ToString());
			};

        }

        private async Task<String> Identify_Best_Server()
		{
			ALL_SERVER_IPS = await Get_Server_List();
			System.Diagnostics.Debug.WriteLine("Got server list");
			SortedDictionary<long, string> server_rankings = new SortedDictionary<long, string>();
            System.Diagnostics.Debug.WriteLine("Allserver null? " + (ALL_SERVER_IPS == null));
            foreach(KeyValuePair<string, string> server in ALL_SERVER_IPS)
			{
                //Key is server ip, value is name.
                server_rankings.Add(await Evaluate_Ping(server.Key), server.Key);
			}
            foreach(KeyValuePair<long, string> entry in server_rankings){
                return entry.Value;
			}
            return "";
        }

        private async Task<long> Evaluate_Ping(string serverAddress)
        {
            try
            {
                using (var client = new HttpClient())
                {
					var sw = Stopwatch.StartNew();
                    HttpResponseMessage response = await client.GetAsync("http://" + serverAddress);
					string result = await response.Content.ReadAsStringAsync();
                    sw.Stop();
                    long msPing = sw.ElapsedMilliseconds;
                    return msPing;
                }
            }catch(HttpRequestException connectionException){
				System.Diagnostics.Debug.WriteLine(connectionException.Message);
                return long.MaxValue; 
            }
        }

        private async Task<Dictionary<string, string>> Get_Server_List() 
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = SERVER_LIST_IP;
                    HttpResponseMessage response = await client.GetAsync(url);
                    string result = await response.Content.ReadAsStringAsync();

                    string[] lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    Dictionary<string, string> toReturn = new Dictionary<string, string>();
                    foreach (String line in lines)
                    {
                        if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase) || line.StartsWith(" ", StringComparison.OrdinalIgnoreCase) || line.Length == 0)
                        {
                            continue;
                        }
                        string[] parts = line.Split(new string[] { "=" }, StringSplitOptions.None);
                        toReturn.Add(parts[0], parts[1]);
					}
                    return toReturn;
                }
            }catch(HttpRequestException connectionException){
                System.Diagnostics.Debug.WriteLine("Exception: " + connectionException.ToString());
                await DisplayAlert("No Internet Connection", "Since this is a first time run, internet connection is required. App will now exit.", "OK");
				var closer = DependencyService.Get<ICloseApplication>();
				if (closer != null)
					closer.closeApplication();
                return null;
            }
        }
    }
}
