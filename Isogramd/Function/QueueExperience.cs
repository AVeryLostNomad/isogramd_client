using System;
using Isogramd.UI.Animated;
using Xamarin.Forms;
using System.Net.Http;
using Isogramd.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Isogramd.Carousel;

namespace Isogramd.Function
{
    public class QueueExperience : ContentPage
    {
        private long startedQueue;
        private AnimatedLabel queueingFor;

		private AnimatedLabel expectedQueueTime;
		private AnimatedLabel spentQueueTime;
		private AnimatedLabel rewardLabel;

		private Dictionary<string, View> key_pregame_dict;

		public QueueExperience()
        {
            RelativeLayout layout = new RelativeLayout();

            Label queueingForTitle = new Label()
            {
                Text = "Queuing for...",
				FontAttributes = FontAttributes.Italic,
				HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.Start,
                FontSize = 34,
            };
            layout.Children.Add(queueingForTitle, Constraint.RelativeToParent((p) => { return (p.Width / 5) - 50; }), Constraint.Constant(45), Constraint.RelativeToParent((p) => { return p.Width - (p.Width / 5); }), Constraint.Constant(50));

			queueingFor =
				new AnimatedLabel
				{
					Text = (string) App.Get_Data_Store().Fetch("queue_for"),
					FontSize = 42,
					FontAttributes = FontAttributes.Bold
				};
			layout.Children.Add(queueingFor, Constraint.RelativeToParent((p) => { return (p.Width / 4) * 3 - 100; }), Constraint.Constant(45 + 40), Constraint.RelativeToParent((p) => { return p.Width - ((p.Width / 4) * 3 - 100); }), Constraint.Constant(70));

            StackLayout waitLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            expectedQueueTime = new AnimatedLabel
            {
                Opacity = 0.0,
                Text = "Expected Queue Time: ",
                FontSize = 20,
            };
			waitLayout.Children.Add(expectedQueueTime);
            spentQueueTime = new AnimatedLabel
			{
				Opacity = 0.0,
                Text = "Time Spent in Queue: ",
				FontSize = 20,
			};
			waitLayout.Children.Add(spentQueueTime);
            rewardLabel = new AnimatedLabel
			{
				Opacity = 0.0,
				Text = "Reward: ",
				FontSize = 20,
			};
			waitLayout.Children.Add(rewardLabel);

            layout.Children.Add(waitLayout, Constraint.RelativeToParent((p) => { return 0; }), Constraint.RelativeToParent((p) => { return (p.Height * 3) / 4; }), Constraint.RelativeToParent((p) => { return p.Width; }), Constraint.Constant(30));

			key_pregame_dict = new Dictionary<string, View>();

			this.Content = layout;

			Check_Queue_Status_Until_Done();
		}

		public async void Check_Queue_Status_Until_Done()
		{
			string pid = (string)App.Get_Data_Store().Fetch("pid");
			string server = (string)App.Get_Data_Store().Fetch("server");
			string url = @"http://" + server + @"/queue?pid=" + pid + @"&for=" + (string)App.Get_Data_Store().Fetch("queue_for");

			HttpResponseMessage resp = await new HttpClient().GetAsync(url);
			string result = await resp.Content.ReadAsStringAsync();

			var _keepPolling = true;
			while (_keepPolling)
			{
				Dictionary<string, object> messages = await CommonTools.Get_Messages(server, pid);

				if(messages.Keys.Count != 0)
				{
					foreach(KeyValuePair<string, object> entry in messages)
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

						Dictionary<string, object> message_json = (Dictionary<string, object>)entry.Value;

						Boolean isJson = (bool) message_json["is_json"];

						if (isJson)
						{
							//See if the message is json, and handle it like that, if so.
							Dictionary<string, object> json_array = (Dictionary<string, object>)message_json["message"];
							foreach (KeyValuePair<string, object> keypair in json_array)
							{
								switch (keypair.Key)
								{
									case "word_size":
										App.Get_Data_Store().Store("word_size", keypair.Value.ToString());
										break;
									case "game_id":
										App.Get_Data_Store().Store("game_id", keypair.Value.ToString());
										_keepPolling = false;
										break;
									case "wait_seconds":
                                        System.Diagnostics.Debug.WriteLine("WAit: " + keypair.Value.ToString());
                                        string wait = ((double)keypair.Value).ToString("#.000");
										expectedQueueTime.Text = "Expected Queue Time: " + wait + " s";
										if (expectedQueueTime.Opacity != 0.0)
										{
											//We've already faded in
											spentQueueTime.AnimatePop();
											break;
										}
                                        expectedQueueTime.AnimateFadeIn();
										break;
									case "elapsed_seconds":
										string elapsed = ((double)keypair.Value).ToString("#.000");
                                        spentQueueTime.Text = "Time Spent in Queue: " + elapsed + " s";
                                        if(spentQueueTime.Opacity != 0.0){
                                            //We've already faded in
                                            spentQueueTime.AnimatePop();
                                            break;
                                        }
										spentQueueTime.AnimateFadeIn();
										break;
                                    case "reward":
                                        Dictionary<string, object> reward_json = (Dictionary<string, object>)keypair.Value;
										spentQueueTime.FontAttributes = FontAttributes.Bold; //Mark it bold, if this time is greater than expected
                                        rewardLabel.Text = "+ " + (string)reward_json["grams"] + " grams and + " + (string)reward_json["starcoin"] + " SC";
                                        if(rewardLabel.Opacity != 0.0){
                                            //Reward label is already faded in
                                            rewardLabel.AnimatePop();
                                            break;
                                        }
                                        rewardLabel.AnimateFadeIn();
										break;
									case "drop_queue":
                                        //Display some sort of information about being kicked out of the queue. 
                                        //Cannot re-queue for the next ten minutes
//										await App.GetNavigation().PushAsync
//											(new QueueFailedExperience());
                                        break;
								}
							}
						}
						else
						{
							string message = (string)message_json["message"];
							
							if (message.Contains("[ACTIVITY_CHECK]"))
							{
								//Mark activity
								using (var cl = new HttpClient())
								{
									string uls = @"http://" + server + @"/" + @"active?pid=" + pid;
									HttpResponseMessage response = await cl.GetAsync(uls);
									await response.Content.ReadAsStringAsync();
								}
							}

                            if(message.Contains("[CANNOT_QUEUE]"))
                            {
                                await DisplayAlert("Error!", "Queue impossible, blocked from all queue options for ten minutes.", "Okay");
                                await App.GetNavigation().PushAsync
                                         (new TabbedCarouselExperience());
                                return;
                            }
						}
					}
					
				}
				if (_keepPolling)
				{
					await Task.Delay(TimeSpan.FromSeconds(5));
				}
			}

			await Finalize_Queue();
		}

		private async Task Finalize_Queue()
		{
			var currentPosY = 200;
			// Get the relevant information from the data store. This should have been filled out
			// While we were waiting for a queue.
			string pid = (string)App.Get_Data_Store().Fetch("pid");
			string server = (string)App.Get_Data_Store().Fetch("server");
			string game_id = (string)App.Get_Data_Store().Fetch("game_id");
			//Fetch any pregame elements that need-be filled out
			Dictionary<string, object> elements = await Get_Pregame(server, pid, game_id);
			
			// Pregame layout
			StackLayout pregameLayout = new StackLayout()
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			//Add a black screen over the page for pregame information. If none is needed, then continue on.
			StackLayout blackScreen = new StackLayout
			{
				BackgroundColor = Color.Black,
				Opacity = 0.4,
			};
			((RelativeLayout)this.Content).Children.Add(blackScreen, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((p) => { return p.Width; }), Constraint.RelativeToParent((p) => { return p.Height; }));

			if(elements.Count == 0)
			{
				//There is no needed pre-game. Continue on as if there isn't one.
				await App.GetNavigation().PushAsync(new Isogramd.Function.GameIntroExperience());
				return;
			}

			foreach(KeyValuePair<string, object> entry in elements)
			{
				string key = entry.Key;
				Dictionary<string, object> element_json = (Dictionary<string, object>)entry.Value;

				// Get information about this type of pre-game item.
				string title = (string)element_json["title"];
				string description = (string)element_json["description"];
				string type = (string)element_json["type"];
				Dictionary<string, object> attributes = (Dictionary<string, object>)element_json["attributes"];

				/*
				 * "attributes": {
                        "length": 1,
                        "allowed_char_types": ["Integer"],
                        "allowed_values": game_util.range_inclusive(3,11)
                    }
				*/

				StackLayout labelLayout = new StackLayout()
				{
					Padding = 10,
					Orientation = StackOrientation.Vertical
				};

				Label desc = new Label
				{
					Text = description,
					FontSize = 18,
					HorizontalOptions = LayoutOptions.FillAndExpand,
				};
				labelLayout.Children.Add(desc);

				pregameLayout.Children.Add(labelLayout);

				switch (type)
				{
					case "text_field":
						Entry item_entry = new Entry
						{
							Placeholder = "",
							FontSize = 24,
							HorizontalTextAlignment = TextAlignment.Center,
						};
						if (attributes.ContainsKey("length"))
						{
							//Add a length listener
							item_entry.TextChanged += (sender, args) =>
							{
								string _text = item_entry.Text;
								string lengthString = attributes["length"].ToString();
								int lengthValue = int.Parse(lengthString);
								if (_text.Length > lengthValue)
								{
									_text = _text.Remove(_text.Length - 1);
									item_entry.Text = _text;
								}
								
							};
						}
						if (attributes.ContainsKey("allowed_char_types"))
						{
							String typesAllowed = (string)attributes["allowed_char_types"];
							if (typesAllowed.Equals("Integer"))
							{
								item_entry.Keyboard = Keyboard.Numeric;
							}
						}
						if (attributes.ContainsKey("allowed_values"))
						{
							String allowedValues = (string)attributes["allowed_values"];
							String[] values = allowedValues.Split(',');
							item_entry.TextChanged += (sender, args) =>
							{
								string _text = item_entry.Text;

								if (String.IsNullOrWhiteSpace(_text)) return;

								Boolean found = false;
								foreach(String val in values)
								{
									if (val.Equals(_text))
									{
										found = true;
										break;
									}
								}
								if (!found)
								{
									//We could not find a matching value
									_text = _text.Remove(_text.Length - 1);
									item_entry.Text = _text;
								}
							};
						}
						pregameLayout.Children.Add(item_entry);
						key_pregame_dict.Add(key, item_entry);
						break;
				}
				currentPosY += 32;
			}

			StackLayout buttonLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Padding = 10
			};

			Button clear_all = new Button
			{
				Text = "Clear",
				BackgroundColor = Color.WhiteSmoke,
				BorderColor = Color.PowderBlue,
				TextColor = Color.DimGray,
				BorderWidth = (Double)1.5,
				BorderRadius = 5,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			clear_all.Clicked += (sender, e) =>
			{
				foreach(KeyValuePair<string, View> entry in key_pregame_dict)
				{
					View view = entry.Value;
					if(view.GetType() == typeof(Entry))
					{
						Entry ef = (Entry)view;
						ef.Text = "";
					}else if(view.GetType() == typeof(Picker))
					{
						Picker pick = (Picker)view;
						pick.SelectedIndex = 0;
					}
				}
			};
			buttonLayout.Children.Add(clear_all);

			Button send_all = new Button
			{
				Text = "Send Pregame",
				BackgroundColor = Color.WhiteSmoke,
				BorderColor = Color.PowderBlue,
				TextColor = Color.DimGray,
				BorderWidth = (Double)1.5,
				BorderRadius = 5,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};
			send_all.Clicked += (sender, e) =>
			{
				Dictionary<string, string> to_send = new Dictionary<string, string>();
				foreach (KeyValuePair<string, View> entry in key_pregame_dict)
				{
					View view = entry.Value;
					if (view.GetType() == typeof(Entry))
					{
						Entry ef = (Entry)view;
						if (String.IsNullOrWhiteSpace(ef.Text))
						{
							//Don't send, but alert the player.
							DisplayAlert("Oops!", "You haven't filled out a value", "Okay");
							return;
						}
						to_send.Add(entry.Key, ef.Text);
					}
					else if (view.GetType() == typeof(Picker))
					{
						Picker pick = (Picker)view;
						if (String.IsNullOrWhiteSpace(pick.SelectedItem.ToString()))
						{
							//Don't send, but alert the player.
							DisplayAlert("Oops!", "You haven't filled out a value", "Okay");
							return;
						}
						to_send.Add(entry.Key, pick.SelectedItem.ToString());
					}
				}
				Send_Pregame(server, pid, game_id, to_send);
			};
			buttonLayout.Children.Add(send_all);

			pregameLayout.Children.Add(buttonLayout);
			
			StackLayout whiteScreen = new StackLayout
			{
				BackgroundColor = Color.White,
			};
			((RelativeLayout)this.Content).Children.Add(whiteScreen, Constraint.Constant(19), Constraint.Constant(19), Constraint.RelativeToParent((p) => { return p.Width - 38; }), Constraint.RelativeToParent((p) => { return p.Height - 38; }));

			ShapeView outerPanel = new ShapeView
			{
				ShapeType = ShapeType.Box,
				StrokeColor = Color.Black,
				StrokeWidth = 1,
				CornerRadius = 3
			};
			((RelativeLayout)this.Content).Children.Add(outerPanel, Constraint.Constant(20), Constraint.Constant(20), Constraint.RelativeToParent((p) =>
			{
				return p.Width - 40;
			}), Constraint.RelativeToParent((p) =>
			{
				return p.Height - 40;
			}));

			((RelativeLayout)this.Content).Children.Add(pregameLayout, Constraint.Constant(20), Constraint.Constant(20), Constraint.RelativeToParent((p) => { return p.Width - 40; }), Constraint.RelativeToParent((p) => { return p.Height - 40; }));

			this.ForceLayout();
		}

		protected override void OnAppearing()
        {
            base.OnAppearing();
            queueingFor.AnimatePulse();
        }

		static async void Send_Pregame(string server, string pid, string gameid, Dictionary<string, string> data)
		{
			using (var client = new HttpClient())
			{
				String json_string = JsonConvert.SerializeObject(
						data, new Isogramd.Util.JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				string url = @"http://" + server + @"/" + @"pregame?pid=" + pid + @"&game=" + gameid + @"&result=" + json_string;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();
			}
			await CommonTools.Wait_For_Release(server, pid);

			App.Get_Data_Store().Store("opponents", await CommonTools.Get_Opponents(server, pid, gameid));
			App.Get_Data_Store().Store("game_data", await CommonTools.Get_Game_Data(server, gameid));

			await App.GetNavigation().PushAsync(new Isogramd.Function.GameIntroExperience());
		}

		static async Task<Dictionary<string, object>> Get_Pregame(string server, string pid, string gameid)
		{
			using (var client = new HttpClient())
			{
				string url = @"http://" + server + @"/" + @"pregame?pid=" + pid + @"&game=" + gameid;
				HttpResponseMessage response = await client.GetAsync(url);
				string result = await response.Content.ReadAsStringAsync();

				Dictionary<string, object> json =
					(System.Collections.Generic.Dictionary<string, object>)
					JsonConvert.DeserializeObject<IDictionary<string, object>>(
						result, new Isogramd.Util.JsonConverter[] {
					new Isogramd.Util.JsonConverter()
					});

				// This works!
				Dictionary<string, object> payload =
					(Dictionary<string, object>)json["payload"];

				Dictionary<string, object> messages =
					(Dictionary<string, object>)payload["message"];

				return messages;
			}
		}
	}
}
