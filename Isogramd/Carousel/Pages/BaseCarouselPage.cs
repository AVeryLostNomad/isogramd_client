using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Isogramd.Util;
using Xamarin.Forms;

namespace Isogramd.Carousel.Pages
{
	public abstract class BaseCarouselPage
	{

		// Return the HomeViewModel representation of this page
		public abstract HomeViewModel Return_Model();

		//Is this page type the proper one for this page code? E.g., if the code is "A", is this page the Admin Page?
		public abstract Boolean Is_Code_Match(string codeLetter);

        public void Add_Outer_Border(RelativeLayout layout)
        {
			ShapeView outerPanel = new ShapeView
			{
				ShapeType = ShapeType.Box,
				StrokeColor = Color.LightGray,
				StrokeWidth = 1,
				CornerRadius = 3
			};
			layout.Children.Add(outerPanel, Constraint.Constant(20), Constraint.Constant(20), Constraint.RelativeToParent((p) =>
			{
				return p.Width - 40;
			}), Constraint.RelativeToParent((p) =>
			{
				return p.Height - 40;
			}));
        }

		public Dictionary<string, object> Get_Player_Dict()
		{
			if(App.Get_Data_Store().Has_Item("profile_dict"))
			{
				return ((Dictionary<String, object>) App.Get_Data_Store().Fetch("profile_dict"));
			}
			return ((Dictionary<String, object>)App.Get_Data_Store().Fetch("initial_profile_dict"));
		}
	}
}
