using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Isogramd.Carousel;
using Isogramd.Util;
using Xamarin.Forms;

namespace Isogramd.Function
{
    public class PostGameExperience : ContentPage
    {
        public PostGameExperience()
        {
            StackLayout layout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            this.Content = layout;

            Check_Postgame_Repeat();
        }

        public async void Check_Postgame_Repeat()
        {
            string pid = (string)App.Get_Data_Store().Fetch("pid");
            string server = (string)App.Get_Data_Store().Fetch("server");
            var _keepPolling = true;
            while (_keepPolling)
            {
                Dictionary<string, object> messages = await CommonTools.Get_Postgame(server, pid);

                if (messages.Keys.Count != 0)
                {
                    //We have some message action!
                    foreach (KeyValuePair<string, object> entry in messages)
                    {
                        string key = entry.Key;
                        List<string> keys = new List<string>();
                        if (App.Get_Data_Store().Has_Item("post_read_keys"))
                        {
                            if (((List<string>)App.Get_Data_Store().Fetch("post_read_keys")).Contains(key))
                            {
                                continue;
                            }
                            keys = (List<string>)App.Get_Data_Store().Fetch("post_read_keys");
                        }
                        keys.Add(key);
                        App.Get_Data_Store().Store("post_read_keys", keys);

						Dictionary<string, object> message_json = (Dictionary<string, object>)entry.Value;

                        Boolean is_json = (bool)message_json["is_json"];

						if (is_json)
						{
							Dictionary<string, object> json_array = (Dictionary<string, object>)message_json["message"];
                            Label label = new Label();
                            foreach(KeyValuePair<string, object> item in json_array){
                                switch(item.Key){
                                    case "text":
                                        label.Text = (string)item.Value;
                                        break;
                                    case "size":
                                        label.FontSize = (int)item.Value;
                                        break;
                                    case "bold":
                                        label.FontAttributes = FontAttributes.Bold;
                                        break;
                                }
                            }
							((StackLayout)Content).Children.Add(label);
							this.ForceLayout();
                            continue;
                        }

						string message = (string)message_json["message"];

                        if (message.Contains("[DONE_BUTTON]")){
                            //TODO add the done button
                            Button doneButton = new Button()
                            {
                                Text = "Done",
                                BackgroundColor = Color.WhiteSmoke,
                                BorderColor = Color.PowderBlue,
                                TextColor = Color.DimGray,
                                BorderWidth = (Double)1.5,
                                BorderRadius = 5,
                                Font = Font.SystemFontOfSize(NamedSize.Large),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.CenterAndExpand
                            };
                            doneButton.Clicked += async (sender, e) =>
                            {
                                //This is the button that's pressed at the very end of the whole experience. We clear everything here?
                                App.Get_Data_Store().Clear("post_read_keys");
								App.Get_Data_Store().Clear("read_keys");
								App.Get_Data_Store().Clear("move_list");
                                App.Get_Data_Store().Clear("alphabet_tracker");
                                await App.Get_Data_Store().UpdateUserData();

                                using (var client = new HttpClient())
                                {
                                    string url = @"http://" + server + @"/" + @"clear_all_messages?pid=" + pid;
                                    HttpResponseMessage response = await client.GetAsync(url);
                                    string result = await response.Content.ReadAsStringAsync();
                                }
                                await App.GetNavigation().PushAsync
                                         (new TabbedCarouselExperience());
                                _keepPolling = false;
                            };
							((StackLayout)Content).Children.Add(doneButton);
							this.ForceLayout();
                        }else{
                            //Display the message
                            Label label = new Label
                            {
                                FontSize = 24,
                                Text = message,
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center
                            };
                            ((StackLayout)Content).Children.Add(label);
                            this.ForceLayout();
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}
