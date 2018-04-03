using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Isogramd.Util
{
    public static class CommonTools
    {
		public static async Task<Dictionary<string, object>> Get_Messages(string server, string pid)
		{
			using (var client = new HttpClient())
			{
				string url = @"http://" + server + @"/" + @"get_messages?pid=" + pid;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();

				Dictionary<string, object> json =
					(System.Collections.Generic.Dictionary<string, object>)
					JsonConvert.DeserializeObject<IDictionary<string, object>>(
						result, new JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				// This works!
				Dictionary<string, object> payload =
					(Dictionary<string, object>)json["payload"];

				Dictionary<string, object> messages =
					(Dictionary<string, object>)payload["messages"];

				return messages;
			}
		}

		public static async Task<Dictionary<string, object>> Get_Postgame(string server, string pid)
		{
			using (var client = new HttpClient())
			{
				string url = @"http://" + server + @"/" + @"get_postgame?pid=" + pid;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();

				Dictionary<string, object> json =
					(System.Collections.Generic.Dictionary<string, object>)
					JsonConvert.DeserializeObject<IDictionary<string, object>>(
						result, new JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				// This works!
				Dictionary<string, object> payload =
					(Dictionary<string, object>)json["payload"];

				Dictionary<string, object> messages =
					(Dictionary<string, object>)payload["messages"];

				return messages;
			}
		}

		public static async Task<Dictionary<string, object>> Get_Opponents(string server, string pid, string gameid)
		{
			using (var client = new HttpClient())
			{
				string url = @"http://" + server + @"/" + @"get_opponent_stats?pid=" + pid + @"&gameid=" + gameid;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();

				Dictionary<string, object> json =
					(System.Collections.Generic.Dictionary<string, object>)
					JsonConvert.DeserializeObject<IDictionary<string, object>>(
						result, new JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				// This works!
				Dictionary<string, object> payload =
					(Dictionary<string, object>)json["payload"];

				Boolean success = (Boolean)payload["success"];

				if (!success)
				{
					return new Dictionary<string, object>();
				}

				Dictionary<string, object> messages =
					(Dictionary<string, object>)payload["message"]; // Dictionary of opponents

				return messages;
			}
		}

		public static async Task<Dictionary<string, object>> Get_Game_Data(string server, string gameid)
		{
			using (var client = new HttpClient())
			{
				string url = @"http://" + server + @"/" + @"get_game_description?gameid=" + gameid;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();

				Dictionary<string, object> json =
					(System.Collections.Generic.Dictionary<string, object>)
					JsonConvert.DeserializeObject<IDictionary<string, object>>(
						result, new JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				// This works!
				Dictionary<string, object> payload =
					(Dictionary<string, object>)json["payload"];

				Boolean success = (Boolean)payload["success"];

				if (!success)
				{
					return new Dictionary<string, object>();
				}

				Dictionary<string, object> messages =
					(Dictionary<string, object>)payload["message"]; // Dictionary of game stuff

				return messages;
			}
		}

		public async static Task Wait_For_Release(string server, string pid)
        {
            Boolean keep_reading = true;
            while (keep_reading)
            {
                Dictionary<string, object> messages = await Get_Messages(server, pid);

                if (messages.Keys.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    continue;
                }

                foreach (KeyValuePair<string, object> entry in messages)
                {
                    string key = entry.Key;
                    List<string> keys = new List<string>();
                    if (App.Get_Data_Store().Has_Item("read_keys"))
                    {
                        if (((List<string>)App.Get_Data_Store().Fetch("read_keys")).Contains(key))
                        {
                            continue;
                        }
                        keys = (List<string>)App.Get_Data_Store().Fetch("read_keys");
                    }
                    keys.Add(key);
                    App.Get_Data_Store().Store("read_keys", keys);
                    Dictionary<string, object> message_json =
                        (Dictionary<string, object>)entry.Value;


					Boolean isJson = (bool)message_json["is_json"];

					if (isJson)
					{
						Dictionary<string, object> json_array = (Dictionary<string, object>)message_json["message"];
						foreach (KeyValuePair<string, object> keypair in json_array)
						{
							switch (keypair.Key)
							{
								case "word_size":
									App.Get_Data_Store().Store("word_size", keypair.Value.ToString());
									break;
							}
						}
					}
					else
					{
						string message = (string)message_json["message"];

						if (message.Contains("[ACTIVITY_CHECK]"))
						{
							using (var cl = new HttpClient())
							{
								string uls = @"http://" + server + @"/" + @"active?pid=" + pid;
								HttpResponseMessage response = await cl.GetAsync(uls);
								await response.Content.ReadAsStringAsync();
							}
						}
						else if (message.Contains("[RELEASE]"))
						{
							// We can proceed to the next area!
							keep_reading = false;
						}
					}
                }
            }
        }
    }
}
