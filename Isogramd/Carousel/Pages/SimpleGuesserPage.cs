using Isogramd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Isogramd.Carousel.Pages
{
	class SimpleGuesserPage : BaseGameCarouselPage
	{
		public override bool Is_Code_Match(string codeLetter)
		{
			return codeLetter.Equals("G");
		}

		Entry guessEntryForm;

		public override HomeViewModel Return_Model()
		{
			StackLayout layout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Padding = 10
			};

			Button sendGuess = new Button()
			{
				Text = "Guess",
				BackgroundColor = Color.WhiteSmoke,
				BorderColor = Color.PowderBlue,
				TextColor = Color.DimGray,
				BorderWidth = (Double)1.5,
				BorderRadius = 5,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};

			Button clearGuess = new Button()
			{
				Text = "Clear",
				BackgroundColor = Color.WhiteSmoke,
				BorderColor = Color.PowderBlue,
				TextColor = Color.DimGray,
				BorderWidth = (Double)1.5,
				BorderRadius = 5,
				Font = Font.SystemFontOfSize(NamedSize.Large),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};

			StackLayout buttonLayout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				Padding = 10,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			buttonLayout.Children.Add(clearGuess);
			buttonLayout.Children.Add(sendGuess);

			guessEntryForm = new Entry()
			{
				Placeholder = "",
				FontSize = 24,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				MinimumWidthRequest = 100,
				WidthRequest = 100
			};
			int maxSize = int.Parse(App.Get_Data_Store().Fetch("word_size").ToString());
			guessEntryForm.TextChanged += (sender, args) =>
			{
				string _text = guessEntryForm.Text;

				if (String.IsNullOrWhiteSpace(_text)) return;

				if (_text.Length != maxSize)
				{
					if (sendGuess.IsEnabled)
					{
						sendGuess.IsEnabled = false;
					}
				}
				else
				{
					//We are the proper size
					if (!sendGuess.IsEnabled)
					{
						sendGuess.IsEnabled = true;
					}
				}

				if (_text.Length > maxSize)
				{
					_text = _text.Remove(_text.Length - 1);
					guessEntryForm.Text = _text;
				}
			};
			layout.Children.Add(guessEntryForm);

			clearGuess.Clicked += (sender, e) =>
			{
				guessEntryForm.Text = "";
			};

			sendGuess.Clicked += (sender, e) =>
			{
				Send_Guess(guessEntryForm.Text);
			};

			layout.Children.Add(buttonLayout);

			return new HomeViewModel
			{
				Background = Color.WhiteSmoke,
				ImageSource = "Isogramd.Images.Controller.png",  // The tab icon
				Content = layout,
				TabText = "Guess"
			};
		}

		public async Task Send_Guess(String guess)
		{
			//Is this a valid guess? We know it's at least big enough, because this button is only pressable if the guess is big enough, but we'll check anyway.
			int desiredSize = int.Parse(App.Get_Data_Store().Fetch("word_size").ToString());

			if (guess.Length != desiredSize)
			{
				//await this.carouselInstance.DisplayAlert("Oops!", "That guess isn't the proper length (" + desiredSize.ToString() + ").", "Try again...");
				guessEntryForm.Text = "";
				return;
			}

			if (!TextUtil.IsIsogram(guess))
			{
				//This guess is not an isogram. TODO
				await this.carouselInstance.DisplayAlert("Oops!", "That's not an isogram.", "Try again...");
				guessEntryForm.Text = "";
				return;
			}

			//Heyyy! We've got valid input right here. Let's try something with that.
			string pid = (string)App.Get_Data_Store().Fetch("pid");
			string server = (string)App.Get_Data_Store().Fetch("server");
			string url = @"http://" + server + @"/guess?pid=" + pid + @"&game=" + (string)App.Get_Data_Store().Fetch("game_id") + @"&guess=" + guess;

			HttpResponseMessage resp = await new HttpClient().GetAsync(url);
			string result = await resp.Content.ReadAsStringAsync();

			var _keepPolling = true;
			while (_keepPolling)
			{
				Dictionary<string, object> messages = await CommonTools.Get_Messages(server, pid);

				if (messages.Keys.Count != 0)
				{
					//We have some message action!
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

						Dictionary<string, object> message_json = (Dictionary<string, object>)entry.Value;

						Boolean isJson = (bool)message_json["is_json"];

						if (isJson)
						{
                            //Parse message_json into specific message
                            Dictionary<string, object> msg = (Dictionary<string, object>) message_json["message"];

                            if(msg.ContainsKey("game_end")){
								//This indicates that we're ready to move on to the post_game. Whether or not the game as a whole is
								//"over", our personal game *is*.
                                //await App.GetNavigation().PushAsync(new Isogramd.Function.PostGameExperience()); 
								_keepPolling = false;
								break;
                            }

							//It's probably a bull or cow situation here.
							int bulls = int.Parse(msg["bulls"].ToString());
							int cows = int.Parse(msg["cows"].ToString());
							
                            //TODO come up with a better way to display the most recent guess
                            await this.carouselInstance.DisplayAlert("Guess Result", "Bulls: " + bulls.ToString() + " Cows: " + cows.ToString(), "Great!");

							//TODO figure out how to best do a log.
							List<SimpleMove> move_list = App.Get_Data_Store().Has_Item("move_list") ? (List<SimpleMove>)App.Get_Data_Store().Fetch("move_list") : new List<SimpleMove>();
                            move_list.Add(new SimpleMove
                            {
                                Guess = guess,
                                Bulls = bulls,
                                Cows = cows
                            });
                            App.Get_Data_Store().Store("move_list", move_list);

							await App.GetNavigation().PushAsync(new Isogramd.Carousel.TabbedGameExperience(carouselInstance)); //Navigate to a new one with this one as a basis.
                            _keepPolling = false;
                            break;
                        }
						else
						{
							//This is some other type of message. Perhaps an activity check, perhaps the end of the game?

                            string message = (string)message_json["message"];
                            
                            if (message.Contains("[ACTIVITY_CHECK]")){
								using (var cl = new HttpClient())
								{
									string uls = @"http://" + server + @"/" + @"active?pid=" + pid;
									HttpResponseMessage response = await cl.GetAsync(uls);
									await response.Content.ReadAsStringAsync();
								}
                            }
						}
					}
                }else{
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
			}
		}
	}
}
