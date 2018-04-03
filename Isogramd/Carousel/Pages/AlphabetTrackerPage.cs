using System;
using System.Collections.Generic;
using Isogramd.Util;
using Isogramd.UI;
using Xamarin.Forms;

namespace Isogramd.Carousel.Pages
{
	class AlphabetTrackerPage : BaseGameCarouselPage
	{
        public Dictionary<char, TappableLabel> letterLabels = new Dictionary<char, TappableLabel>();

		public override bool Is_Code_Match(string codeLetter)
		{
			return codeLetter.Equals("Z");
		}

        public override HomeViewModel Return_Model()
        {
            RelativeLayout layout = new RelativeLayout(){
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Add_Outer_Border(layout);

            StackLayout bigLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
            };
            var tierPadding = 20;
            StackLayout tierOne = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Padding = tierPadding
			};
			StackLayout tierTwo = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				Padding = tierPadding
			};
			StackLayout tierThree = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
                Padding = tierPadding
			};
            Dictionary<char, TappableLabel> labelDict = App.Get_Data_Store().Has_Item("alphabet_tracker") ? (Dictionary<char, TappableLabel>)App.Get_Data_Store().Fetch("alphabet_tracker") : new Dictionary<char, TappableLabel>();
            var index = 0;
            for (char c = 'A'; c <= 'Z'; c++){
                TappableLabel label = new TappableLabel()
                {
                    Text = "" + c,
                    FontSize = 26,
                };
                if(labelDict.ContainsKey(c)){
                    label.FontAttributes = labelDict[c].FontAttributes;
                    label.TextColor = labelDict[c].TextColor;
                }
				var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (sender, e) => {
                    if(label.TextColor == Color.Red){
                        label.TextColor = Color.Default;
                        label.FontAttributes = FontAttributes.None;
                    }else if (label.TextColor == Color.Default){
                        label.TextColor = Color.Green;
                        label.FontAttributes = FontAttributes.Bold;
                    }else if(label.TextColor == Color.Green){
						label.TextColor = Color.Red;
						label.FontAttributes = FontAttributes.Bold;
                    }
                    App.Get_Data_Store().Store("alphabet_tracker", letterLabels);
                };
				label.GestureRecognizers.Add(tapGestureRecognizer);
                letterLabels.Add(c, label);
                if(index < 10){
                    tierOne.Children.Add(label);
                }else if(index < 20){
                    tierTwo.Children.Add(label);
                }else{
                    tierThree.Children.Add(label);
                }
                index += 1;
			}
            bigLayout.Children.Add(tierOne);
            bigLayout.Children.Add(tierTwo);
            bigLayout.Children.Add(tierThree);

            layout.Children.Add(bigLayout, Constraint.Constant(20), Constraint.Constant(20), Constraint.RelativeToParent((p) => { return p.Width - 40; }), Constraint.RelativeToParent((p) => { return p.Height - 40; }));

            return new HomeViewModel
            {
                Background = Color.WhiteSmoke,
                ImageSource = "Isogramd.Images.Check.png",  // The tab icon
                Content = layout,
                TabText = "Letters"
            };
        }
    }
}
