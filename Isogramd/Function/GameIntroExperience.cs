using Isogramd.UI.Animated;
using Isogramd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Isogramd.Function
{
	class GameIntroExperience : ContentPage
	{
		List<AnimatedLabel> toFadeOut = new List<AnimatedLabel>();
		List<AnimatedLabel> toFadeIn = new List<AnimatedLabel>();

		Dictionary<int, Entry> entryDict = new Dictionary<int, Entry>();

		public GameIntroExperience()
		{
			Random rand = new Random();

			StackLayout startingLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			AnimatedLabel statusLabel = new AnimatedLabel
			{
				Text = "Welcome to Game " + (string)App.Get_Data_Store().Fetch("game_id"),
				FontSize = 36,
				FontAttributes = FontAttributes.None,
				Padding = 10,
                TextAlignmentHorizontal = TextAlignment.Center
			};
			toFadeOut.Add(statusLabel);
			startingLayout.Children.Add(statusLabel);

			StackLayout opponentInfo = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			AnimatedLabel opponentLabel = new AnimatedLabel
			{
				Text = "Opponent: ",
				FontSize = 22,
				FontAttributes = FontAttributes.Bold,
			};
			toFadeOut.Add(opponentLabel);
			opponentInfo.Children.Add(opponentLabel);

			//All this is to identify what opponents we are about to face in our match. It's very exciting stuff here.

			//Dictionary of <index>, <opponent profile dict - personal details>
			Dictionary<string, object> opponents = (Dictionary<string, object>) App.Get_Data_Store().Fetch("opponents");
			// The number of opponents
			int count = 0;
			//The string containing your opponents' names.
			String opponentString = "";
			//Label to hold the catchphrase of the opponent. Might feasibly be empty if they haven't equipped one. 
			AnimatedLabel opponentCatchphrase = new AnimatedLabel
			{
				FontSize = 18,
				FontAttributes = FontAttributes.Italic,
				Padding = 20
			};
			toFadeOut.Add(opponentCatchphrase);
			//The catchphrase to display to the player
			String chosenCatchphrase = "";
			//List to hold all opponent catchphrases (in case of multiple, we pick one)
			List<String> catchphraseList = new List<String>();
			//Go through each opponent, increment, and get their name
			foreach(KeyValuePair<string, object> entry in opponents)
			{
				count++;
				Dictionary<string, object> profile = (Dictionary<string, object>)entry.Value;
				opponentString = opponentString + (string) profile["name"] + ", ";
				string catchphrase = (string) profile["catchphrase"];
				if (!String.IsNullOrWhiteSpace(catchphrase)){
					catchphraseList.Add(catchphrase);
				}
			}
			//We have opponents (not a solo game)
			if(count > 0)
			{
				//Change label to indicate multiple
				opponentLabel.Text = "Opponents: ";
				//Remove the trailing comma and space
				opponentString = opponentString.Substring(0, opponentString.Length - 2);
				chosenCatchphrase = catchphraseList[rand.Next(0, catchphraseList.Count)];
				opponentCatchphrase.Text = chosenCatchphrase;
			}
			else
			{
				//There are no opponents
				opponentString = "Yourself";
			}

			AnimatedLabel opponentName = new AnimatedLabel
			{
				Text = opponentString,
				FontSize = 22
			};
			toFadeOut.Add(opponentName);
			opponentInfo.Children.Add(opponentName);

			startingLayout.Children.Add(opponentInfo);

			if (!String.IsNullOrWhiteSpace(chosenCatchphrase))
			{
				startingLayout.Children.Add(opponentCatchphrase);
			}

			this.Content = startingLayout;

			Display_UI();
		}

		public async void Display_UI()
		{
			await Do_Fadeout(10.0);
			await Display_Game_Info();
			await Do_Fadein(.5);
			await Do_Fadeout(15);
			//Navigate to the actual game controls.
			await Task.Delay(TimeSpan.FromSeconds(1.5)); // Wait for the final fade out

			await App.GetNavigation().PushAsync(new Isogramd.Carousel.TabbedGameExperience());
		}

		public async Task Do_Fadeout(double wait)
		{
			await Task.Delay(TimeSpan.FromSeconds(wait));
			System.Diagnostics.Debug.WriteLine("Got here");
			foreach(AnimatedLabel label in toFadeOut)
			{
				label.AnimateFadeOut();
			}

			toFadeOut.Clear();
		}

		public async Task Do_Fadein(double wait)
		{
			await Task.Delay(TimeSpan.FromSeconds(wait));

			foreach (AnimatedLabel label in toFadeIn)
			{
				label.AnimateFadeIn();
			}

			toFadeIn.Clear();
		}

		public async Task Display_Game_Info()
		{
			await Task.Delay(TimeSpan.FromSeconds(1.5));

			StackLayout infoLayout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
				Padding = 10
			};

			Dictionary<string, object> game_data = (Dictionary<string, object>) App.Get_Data_Store().Fetch("game_data");
			//name, desc

			AnimatedLabel subtextLabel = new AnimatedLabel
			{
				Text = TextUtil.FirstLetterToUpperCase((string) game_data["name"]) + " Mode",
				FontSize = 32,
				FontAttributes = FontAttributes.Bold,
				HorizontalLayout = LayoutOptions.Center,
				Opacity = 0.0,
			};
			toFadeIn.Add(subtextLabel);
			toFadeOut.Add(subtextLabel);
			infoLayout.Children.Add(subtextLabel);

			AnimatedLabel descriptionLabel = new AnimatedLabel
			{
				Text = (string) game_data["desc"],
				HorizontalLayout = LayoutOptions.Center,
				FontSize = 18,
				Opacity = 0.0,
			};
			toFadeIn.Add(descriptionLabel);
			toFadeOut.Add(descriptionLabel);
			infoLayout.Children.Add(descriptionLabel);

			AnimatedLabel timerStartLabel = new AnimatedLabel
			{
				Text = "Time starts when your first guess is made",
				HorizontalLayout = LayoutOptions.Center,
				FontSize = 18,
				Opacity = 0.0,
			};
			toFadeIn.Add(timerStartLabel);
			toFadeOut.Add(timerStartLabel);
			infoLayout.Children.Add(timerStartLabel);

			AnimatedLabel ngramLengthLabel = new AnimatedLabel
			{
				Text = "You are searching for a " + (string)App.Get_Data_Store().Fetch("word_size") + "-gram",
				FontSize = 24,
				FontAttributes = FontAttributes.Bold,
				HorizontalLayout = LayoutOptions.Center,
				Opacity = 0.0,
			};
			toFadeIn.Add(ngramLengthLabel);
			toFadeOut.Add(ngramLengthLabel);
			infoLayout.Children.Add(ngramLengthLabel);

			this.Content = infoLayout;
			this.ForceLayout();
		}

	}
}
