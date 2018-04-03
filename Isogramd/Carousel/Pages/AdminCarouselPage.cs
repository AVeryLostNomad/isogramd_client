using Isogramd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Isogramd.Carousel.Pages
{
    public class AdminCarouselPage : BaseCarouselPage
    {
        public override bool Is_Code_Match(string codeLetter)
        {
            return codeLetter.Equals("A");
        }

        public override HomeViewModel Return_Model()
        {
            RelativeLayout relativeLayout = new RelativeLayout()
            {
            };

            Label pageLayout = new Label
            {
                Text = "Admin Control Panel",
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

            Init_PlayerCommands();
            Add_Player_Command_Buttons(relativeLayout);
			
			return new HomeViewModel
			{
				Background = Color.WhiteSmoke,
				ImageSource = "Isogramd.Images.Denied.png",  // The tab icon
				Content = relativeLayout,
				TabText = "Admin CP"
			};
		}

        Dictionary<String, String> playerCommands;
        public void Init_PlayerCommands()
        {
	        string pid = (string)App.Get_Data_Store().Fetch("pid");
	        string server = (string)App.Get_Data_Store().Fetch("server");
			playerCommands = new Dictionary<String, String>()
			{
				{"Ban/Unban", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=ban&for="}, //Toggle the ban state of the player
				{"Silence", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=silence&for="}, //Toggle player's ability to use text chat in game
                {"Deafen", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=deafen&for="}, //Toggle player's ability to receive messages in game
                {"Freeze", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=freeze&for="}, //When on, player's values will not change, though functionality remains. (iiq will not lower, money does not decrease, etc...)
                {"Liquidate", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=liquidate&for="}, //Not a toggle, but will immediately drain all of the player's various currencies (set to zero)
                {"IIQ Reset", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=iiqreset&for="}, //Not a toggle, but will immediately reset the player's iiq back to pre-placement values (player would have to redo placements)
                {"Mod Level", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=modlevel&for="}, //Change a player's level
                {"Mod Funds", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=modfunds&for="}, //Change the amount of currency(ies) a player has
                {"Mod Rank", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=modrank&for="}, //Set a player's permission/rank level to any level *up to* one below the issuer.
                {"Mod IIQ", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=modiiq&for="}, //Set a player's iiq to any level
                {"LQQ Brand", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=brand&for="}, //Give the player a permanent mark on their account. Invisible to the player, this will auto-place them in low priority/low quality queues (can only play with other LQQ branded accounts, minimum 3 minute wait time on queues)
                {"Force Sim", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=forcesim&for="}, //Invisible to the player, this mark on an account will make all games (even ones that appear to publicly queue) be versus AI. Names will be chosen from the account database, everything will "seem" real.
                {"IP Ban", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=ipban&for="}, //No-one at this IP address will be able to play the game. A toggle. The warning screen lists all accounts currently associated with this IP address.
                {"Bad Luck", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=badluck&for="}, //The player will never roll more than the lowest tier items in loot boxes
                {"Scramble", @"http://" + server + @"/" + @"admin?pid=" + pid + @"&action=scramble&for="}, //Server confirmation of bulls/cows during game will be randomly generated. (Very rude punishment).
            };
        }

        public void Add_Player_Command_Buttons(RelativeLayout relativeLayout) 
        {
            Entry playerUsernameEntry = new Entry
            {
                Placeholder = "Player Username",
                PlaceholderColor = Color.LightBlue,
                FontSize = 16,
                HorizontalTextAlignment = TextAlignment.Center,
                Opacity = 0.75,
            };
            //relativeLayout.Children.Add(playerUsernameEntry, Constraint.Constant(30), Constraint.Constant(75), Constraint.RelativeToParent((p) => { return p.Width - 60; }), Constraint.Constant(50));

            Picker testPicker = new Picker
            {

            };
            testPicker.Items.Add("Test");
			testPicker.Items.Add("Woo"); 
            relativeLayout.Children.Add(testPicker, Constraint.Constant(30), Constraint.Constant(75), Constraint.RelativeToParent((p) => { return p.Width - 60; }), Constraint.Constant(50));

		}
	}
}