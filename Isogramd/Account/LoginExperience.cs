using System;
using System.Collections.Generic;
using System.Net.Http;
using Isogramd.Carousel;
using Newtonsoft.Json;
using Plugin.SecureStorage;
using Xamarin.Forms;

namespace Isogramd.Account
{
    public partial class LoginExperience : ContentPage
    {
        private Button login;
        private Button register;
        private Switch rememberMeSwitch;
        
        public LoginExperience()
        {
            StackLayout layout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Label emailLabel = new Label()
            {
                Text = "Email",
                TextColor = Color.LightBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 24,
                HorizontalTextAlignment = TextAlignment.Center
            };
            layout.Children.Add(emailLabel);

            Entry emailEntry = new Entry()
            {
                Text = "",
                Keyboard = Keyboard.Email,
                HorizontalTextAlignment = TextAlignment.Center,
                Placeholder = "<Enter email>"
            };
            layout.Children.Add(emailEntry);

            Label passwordLabel = new Label()
            {
                Text = "Password",
                TextColor = Color.LightBlue,
                FontAttributes = FontAttributes.Bold,
                FontSize = 24,
                HorizontalTextAlignment = TextAlignment.Center
            };
            layout.Children.Add(passwordLabel);
            
            Entry passwordEntry = new Entry()
            {
                Text = "",
                Keyboard = Keyboard.Default,
                HorizontalTextAlignment = TextAlignment.Center,
                Placeholder = "<Enter email>",
                IsPassword = true
            };
            layout.Children.Add(passwordEntry);
            
            StackLayout remember_me = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            
            rememberMeSwitch = new Switch()
            {
                IsToggled = false
            };
            remember_me.Children.Add(rememberMeSwitch);
            
            Label rememberMeLabel = new Label()
            {
                Text = "Remember me?",
                TextColor = Color.LightGray,
                FontSize = 15,
                HorizontalTextAlignment = TextAlignment.Center
            };
            remember_me.Children.Add(rememberMeLabel);
            
            layout.Children.Add(remember_me);

            StackLayout buttonLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            
            login = new Button
            {
                Text = "Login",
                BackgroundColor = Color.WhiteSmoke,
                BorderColor = Color.PowderBlue,
                TextColor = Color.DimGray,
                BorderWidth = (Double)1.5,
                BorderRadius = 5,
                Font = Font.SystemFontOfSize(NamedSize.Large),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            login.Clicked += (sender, e) =>
            {
                string server = (string) App.Get_Data_Store().Fetch("server");
                string email = emailEntry.Text;
                string password = passwordEntry.Text;
                Do_Login(server, email, password);
            };
            buttonLayout.Children.Add(login);

            register = new Button()
            {
                Text = "Register",
                BackgroundColor = Color.WhiteSmoke,
                BorderColor = Color.PowderBlue,
                TextColor = Color.DimGray,
                BorderWidth = (Double) 1.5,
                BorderRadius = 5,
                Font = Font.SystemFontOfSize(NamedSize.Large),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            register.Clicked += (sender, e) =>
            {
                string server = (string) App.Get_Data_Store().Fetch("server");
                string email = emailEntry.Text;
                string password = passwordEntry.Text;
                Do_Register(server, email, password);
            };
            buttonLayout.Children.Add(register);

            layout.Children.Add(buttonLayout);

            this.Content = layout;

			if (CrossSecureStorage.Current.HasKey("Email"))
			{
				Do_Login((string)App.Get_Data_Store().Fetch("server"), CrossSecureStorage.Current.GetValue("Email"), CrossSecureStorage.Current.GetValue("Password"));
				return;
			}
        }

        async void Do_Register(string server, string email, string password)
        {
            this.login.IsEnabled = false;
            this.register.IsEnabled = false;
			string url = @"http://" + server + @"/" + @"register?email=" + email +
						 @"&password=" + password;
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
				(Dictionary<string, object>)json["payload"];

			bool success = (bool)payload["success"];
            
            string message = (string)payload["message"];

			if (success)
			{
				//Because this is a login, we can fetch a pid and go from there.
			    await DisplayAlert("Success!", message, "Okay!");
			    this.login.IsEnabled = true;
			    this.register.IsEnabled = true;
			}
			else
			{
			    
				await DisplayAlert("Error Logging In...", message, "Try again");
				this.login.IsEnabled = true;
				this.register.IsEnabled = true;
			}
        }
        
        // Login to a server using an email and password
        async void Do_Login(string server, string email, string password)
        {
            this.login.IsEnabled = false;
            this.register.IsEnabled = false;
            //Todo validate login.
            //App.GetNavigation().PushAsync(TODO page here);
            // Create an HTTP web request using the URL:
            string url = @"http://" + server + @"/" + @"login?email=" + email +
                         @"&password=" + password;
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

            if(success)
            {
                //Because this is a login, we can fetch a pid and go from there.
                string pid = (string) payload["player_id"];
                App.Get_Data_Store().Store("pid", pid);
                
                //Before we go, let's check if the switch is turned on to remember this account. If so, let's save this data.
                if (rememberMeSwitch.IsToggled)
                {
					//Let's save this data.
					CrossSecureStorage.Current.SetValue("Email", email);
					CrossSecureStorage.Current.SetValue("Password", password);
				}
                
                
                await App.GetNavigation().PushAsync
                         (new TabbedCarouselExperience());
            }
            else
            {
                if (CrossSecureStorage.Current.HasKey("Email"))
                {
                    CrossSecureStorage.Current.DeleteKey("Email");
                    CrossSecureStorage.Current.DeleteKey("Password");
                }
                string message = (string)payload["message"];
                await DisplayAlert("Error Logging In...", message, "Try again");
                this.login.IsEnabled = true;
                this.register.IsEnabled = true;
            }
        }
    }
}
