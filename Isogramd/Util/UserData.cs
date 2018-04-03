using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Isogramd.Util
{
    public class UserData
    {
	    private static Boolean firstUpdate = true;   
	    
        private Dictionary<string, object> dataStore;

        public UserData()
        {
            dataStore = new Dictionary<string, object>();
        }

        public void Store(string key, object val)
        {
            dataStore[key] = val;
        }

        public object Fetch(string key)
        {
            return dataStore[key];
        }

        public Boolean Has_Item(string key)
        {
            return dataStore.ContainsKey(key);
        }

        public void Clear(string key){
            dataStore.Remove(key);
        }

		public async Task UpdateUserData()
		{

            System.Diagnostics.Debug.WriteLine("Before request");
			string p_status_url = "http://" + App.Get_Data_Store().Fetch("server") +
										  "/my_profile_status?pid=" +
										  App.Get_Data_Store().Fetch("pid");
            
			System.Diagnostics.Debug.WriteLine("URL Done");
			HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(p_status_url).Result;
			System.Diagnostics.Debug.WriteLine("Request made");
			string result = await response.Content.ReadAsStringAsync();

			System.Diagnostics.Debug.WriteLine("Result read");
			Dictionary<string, object> json =
				(System.Collections.Generic.Dictionary<string, object>)
				JsonConvert.DeserializeObject<IDictionary<string, object>>(
					result, new JsonConverter[] {
					new Isogramd.Util.JsonConverter()
				});

			// This works!
			Dictionary<string, object> payload =
				(Dictionary<string, object>)json["payload"];

            System.Diagnostics.Debug.WriteLine("Payload got.");

			Dictionary<string, object> player_profile_dict = (Dictionary<string, object>)payload["profile"];
			if (firstUpdate)
			{
				App.Get_Data_Store()
					.Store("initial_profile_dict",
						player_profile_dict); //Since we also have an initial one, this will allow us to do a "this visit" statistic
				firstUpdate = false;
			}
			else
			{
				App.Get_Data_Store().Store("profile_dict", player_profile_dict); //Since we also have an initial one, this will allow us to do a "this visit" statistic
			}
		}
    }
}
