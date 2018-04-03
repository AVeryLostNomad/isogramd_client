using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Isogramd.Carousel
{
	public class HomeView : ContentView
	{
		public HomeView()
		{
			BackgroundColor = Color.White;
			
			this.SetBinding(ContentView.BackgroundColorProperty, "Background");

			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = {
				}
			};

			this.SetBinding(ContentView.ContentProperty, "Content");
		}
	}
}
