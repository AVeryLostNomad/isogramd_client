using System;
using System.Collections.Generic;
using System.Net.Http;
using Isogramd.Util;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using Xamarin.Forms;
using JsonConverter = Isogramd.Util.JsonConverter;

namespace Isogramd.Carousel.Pages
{ 
    public class ProfileCarouselPage : BaseCarouselPage
    {

        Picker queuePicker;

        public override bool Is_Code_Match(string codeLetter)
        {
            return codeLetter.Equals("P");
        }

        public override HomeViewModel Return_Model()
        {
			RelativeLayout relativeLayout = new RelativeLayout();

			Label pageLayout = new Label
			{
				Text = "Profile",
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = 20,
				FontAttributes = FontAttributes.Bold,
				Opacity = 0.8,
				TextColor = Color.LightBlue,
			};
			relativeLayout.Children.Add(pageLayout, Constraint.Constant(0), Constraint.Constant(25),
				Constraint.RelativeToParent((p) =>
				{
					return p.Width;
				}), Constraint.Constant(30));

            Add_Outer_Border(relativeLayout);

            ShapeView profileBorder = new ShapeView()
            {
                ShapeType = ShapeType.Box,
                StrokeColor = Color.LightBlue,
                StrokeWidth = 1,
            };
            ShapeView profileAccent = new ShapeView()
            {
                ShapeType = ShapeType.Box,
                StrokeColor = Color.LightGray,
                StrokeWidth = 1
            };
			relativeLayout.Children.Add(profileAccent, Constraint.Constant(26), Constraint.Constant(62), Constraint.Constant(128), Constraint.Constant(128));
			relativeLayout.Children.Add(profileAccent, Constraint.Constant(25), Constraint.Constant(61), Constraint.Constant(128), Constraint.Constant(128));
			relativeLayout.Children.Add(profileBorder, Constraint.Constant(24), Constraint.Constant(60), Constraint.Constant(128), Constraint.Constant(128));

            StackLayout profileInfoLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

	        Label playerNameLabel = new Label
	        {
				Text = TextUtil.FirstLetterToUpperCase((string) Get_Player_Dict()["name"]),
		        TextColor = Color.LightSkyBlue,
		        FontAttributes = FontAttributes.Bold,
		        FontSize = 16,
	        };
            profileInfoLayout.Children.Add(playerNameLabel);

            StackLayout notorietyLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Label notorietyLevel = new Label
            {
				Text = "notoriety",
				TextColor = Color.LightGray,
				FontAttributes = FontAttributes.Bold,
				FontSize = 14,
            };
            Label notorietyValue = new Label
            {
                Text = Get_Player_Dict()["level"].ToString(),
                TextColor = Color.LightSkyBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            };
            notorietyLayout.Children.Add(notorietyLevel);
            notorietyLayout.Children.Add(notorietyValue);
            profileInfoLayout.Children.Add(notorietyLayout);

            StackLayout iiqLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };
            Label iiqTitle = new Label
            {
                Text = "iiq",
                TextColor = Color.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 14
            };
            Label iiqValue = new Label
            {
                Text = Get_Player_Dict()["iiq"].ToString(),
                TextColor = Color.LightSkyBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
            };
            iiqLayout.Children.Add(iiqTitle);
            iiqLayout.Children.Add(iiqValue);
            profileInfoLayout.Children.Add(iiqLayout);

            StackLayout gramsLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            Label gramsTitle = new Label()
            {
                Text = "grams",
                TextColor = Color.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 14
            };

            Label gramsValue = new Label()
            {
                Text = Get_Player_Dict()["grams"].ToString(),
                TextColor = Color.LightSkyBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
            };
            gramsLayout.Children.Add(gramsTitle);
            gramsLayout.Children.Add(gramsValue);
            profileInfoLayout.Children.Add(gramsLayout);

            StackLayout scoinLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
            };
            Label scoinTitle = new Label()
            {
                Text = "star-coins",
                TextColor = Color.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 14
            };
            Label scoinValue = new Label()
            {
                Text = Get_Player_Dict()["scoins"].ToString(),
                TextColor = Color.LightSkyBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16
			};
            scoinLayout.Children.Add(scoinTitle);
            scoinLayout.Children.Add(scoinValue);
            profileInfoLayout.Children.Add(scoinLayout);

            relativeLayout.Children.Add(profileInfoLayout, Constraint.Constant(26 + 128 + 10), Constraint.Constant(62), Constraint.RelativeToParent((p) => { return p.Width - (26 + 128 + 10); }), Constraint.Constant(128));

			StackLayout queueMatchLayout = new StackLayout()
	        {
				Orientation = StackOrientation.Horizontal,
		        HorizontalOptions = LayoutOptions.StartAndExpand,
		        VerticalOptions = LayoutOptions.Center
	        };

	        queuePicker = new Picker()
	        {
				SelectedIndex = 0,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                MinimumWidthRequest = 150,
                WidthRequest = 150
	        };
	        Add_Items_For_Queue(queuePicker);
	        queueMatchLayout.Children.Add(queuePicker);

	        Button doQueue = new Button()
	        {
		        Text = "Queue",
		        BackgroundColor = Color.WhiteSmoke,
		        BorderColor = Color.PowderBlue,
		        TextColor = Color.DimGray,
		        BorderWidth = (Double) 1.5,
		        BorderRadius = 5,
		        Font = Font.SystemFontOfSize(NamedSize.Large),
		        VerticalOptions = LayoutOptions.CenterAndExpand
	        };
            doQueue.Clicked += (sender, e) =>
            {
				App.Get_Data_Store().Store("queue_for", queuePicker.SelectedItem.ToString());

				App.GetNavigation().PushAsync
						 (new Function.QueueExperience());
                return;
            };
	        queueMatchLayout.Children.Add(doQueue);
	        
	        relativeLayout.Children.Add(queueMatchLayout, Constraint.Constant(26), Constraint.RelativeToParent((p) =>
	        {
		        return p.Height / 2;
	        }), Constraint.RelativeToParent((p) =>
	        {
		        return p.Width - 40;
	        }), Constraint.Constant(40));
	        
			return new HomeViewModel
			{
				Background = Color.WhiteSmoke,
				ImageSource = "Isogramd.Images.Cursor.png",  // The tab icon
				Content = relativeLayout,
				TabText = "Profile"
			};
        }

	    public async void Add_Items_For_Queue(Picker picker)
	    {
		    string url = @"http://" + App.Get_Data_Store().Fetch("server") + @"/" + @"queue_options?pid=" +
		                 App.Get_Data_Store().Fetch("pid");
		    HttpClient client = new HttpClient();
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
			    (Dictionary<string, object>) json["payload"];

		    bool success = (bool)payload["success"];

            System.Diagnostics.Debug.WriteLine("This is happening...");

		    if(success)
            {
                string listString = payload["message"].ToString();
                while (listString.Contains("\""))
                {
                    listString = listString.Substring(listString.IndexOf("\"") + 1);
                    string[] parts = listString.Split('\"');
                    string word = parts[0];
                    picker.Items.Add(word);
                    listString = parts[1];
                }
                picker.SelectedIndex = 0;
		    }
		    else
		    {
			    //It didn't work
		    }
	    }
    }
}
