using Isogramd.Carousel.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Isogramd.Util;
using System.Reflection;

namespace Isogramd.Carousel
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabbedCarouselExperience : ContentPage
	{
		View _tabs;

		RelativeLayout relativeLayout;

		CarouselLayout.IndicatorStyleEnum _indicatorStyle;

		SwitcherPageViewModel viewModel;

		List<BaseCarouselPage> carouselPageTypes = new List<BaseCarouselPage>
		{
			new AdminCarouselPage(),
            new ProfileCarouselPage()
		};


		public TabbedCarouselExperience()
		{
			System.Diagnostics.Debug.WriteLine("Doing Carousel");
			_indicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs;

			List<HomeViewModel> pages = CreatePages().Result;
			System.Diagnostics.Debug.WriteLine("Did pages");
			viewModel = new SwitcherPageViewModel(pages);
			BindingContext = viewModel;

			Title = _indicatorStyle.ToString();

			relativeLayout = new RelativeLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			System.Diagnostics.Debug.WriteLine("Pre-Carousel");
			var pagesCarousel = CreatePagesCarousel();
			System.Diagnostics.Debug.WriteLine("Post-Carousel");
			_tabs = CreateTabs();

			var tabsHeight = 50;
            var offsetTabsUp = 15;
			relativeLayout.Children.Add(_tabs,
				Constraint.Constant(0),
                Constraint.RelativeToParent((parent) => { return parent.Height - tabsHeight - offsetTabsUp; }),
				Constraint.RelativeToParent(parent => parent.Width),
				Constraint.Constant(tabsHeight)
			);

			relativeLayout.Children.Add(pagesCarousel,
				Constraint.RelativeToParent((parent) => { return parent.X; }),
				Constraint.RelativeToParent((parent) => { return parent.Y; }),
				Constraint.RelativeToParent((parent) => { return parent.Width; }),
                Constraint.RelativeToView(_tabs, (parent, sibling) => { return parent.Height - (sibling.Height) - offsetTabsUp; })
			);
            
			ShapeView navigationBarBottom = new ShapeView
			{
				ShapeType = ShapeType.Box,
				StrokeColor = Color.LightGray,
				StrokeWidth = 1
			};
			relativeLayout.Children.Add(navigationBarBottom, Constraint.Constant(0), Constraint.RelativeToParent((p) => { return p.Height - offsetTabsUp + 1; }), Constraint.RelativeToParent((p) => { return p.Width; }), Constraint.Constant(1));

			ShapeView navigationBarTop = new ShapeView
			{
				ShapeType = ShapeType.Box,
				StrokeColor = Color.LightGray,
				StrokeWidth = 1
			};
            relativeLayout.Children.Add(navigationBarTop, Constraint.Constant(0), Constraint.RelativeToParent((p) => { return p.Height - offsetTabsUp - _tabs.Height - 1; }), Constraint.RelativeToParent((p) => { return p.Width; }), Constraint.Constant(1));

            System.Diagnostics.Debug.WriteLine("Content loaded");
            this.Content = relativeLayout;
        }

		async Task<List<HomeViewModel>> CreatePages()
		{
			List<HomeViewModel> to_return = new List<HomeViewModel>();

			System.Diagnostics.Debug.WriteLine("Starting pages");
			String pattern = "";
			await App.Get_Data_Store().UpdateUserData();
			System.Diagnostics.Debug.WriteLine("Done updating user data");
            if(App.Get_Data_Store().Has_Item("profile_dict")){
                pattern = (string) ((Dictionary<String, object>)App.Get_Data_Store().Fetch("profile_dict"))["carousel_pages"];
            }else{
                pattern = (string)((Dictionary<String, object>)App.Get_Data_Store().Fetch("initial_profile_dict"))["carousel_pages"];
			}
			System.Diagnostics.Debug.WriteLine("Pages: " + pattern);
            try
            {
                foreach (char c in pattern)
                {
                    System.Diagnostics.Debug.WriteLine("Char: " + c);
                    foreach (BaseCarouselPage page in carouselPageTypes)
                    {
                        if (page.Is_Code_Match(String.Join("", c)))
                        {
                            System.Diagnostics.Debug.WriteLine("Added page");
                            to_return.Add(page.Return_Model());
                            break;
                        }
                    }
                }
            }catch(Exception e){
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
			return to_return;
		}

        int Index_For_Profile_Page(){
            string pattern = "";
			if (App.Get_Data_Store().Has_Item("profile_dict"))
			{
				pattern = (string)((Dictionary<String, object>)App.Get_Data_Store().Fetch("profile_dict"))["carousel_pages"];
			}
			else
			{
				pattern = (string)((Dictionary<String, object>)App.Get_Data_Store().Fetch("initial_profile_dict"))["carousel_pages"];
			}
			try
			{
                int index = 0;
				foreach (char c in pattern)
				{
                    if(c == 'P')
                    {
                        return index;
                    }
                    index+=1;
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
            return -1;
        }

		CarouselLayout CreatePagesCarousel()
		{
            var carousel = new CarouselLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs,
                ItemTemplate = new DataTemplate(typeof(HomeView)),
                SelectedIndex = Index_For_Profile_Page()
			};
			carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
			carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);
            System.Diagnostics.Debug.WriteLine("Selected page: " + Index_For_Profile_Page());
			return carousel;
		}

		View CreateTabsContainer()
		{
			return new StackLayout
			{
				Children = { CreateTabs() }
			};
		}

		View CreateTabs()
		{
			var pagerIndicator = new PagerIndicatorTabs() { HorizontalOptions = LayoutOptions.CenterAndExpand };
			pagerIndicator.RowDefinitions.Add(new RowDefinition() { Height = 50 });
			pagerIndicator.SetBinding(PagerIndicatorTabs.ItemsSourceProperty, "Pages");
			pagerIndicator.SetBinding(PagerIndicatorTabs.SelectedItemProperty, "CurrentPage");

			return pagerIndicator;
		}
	}

	public class SpacingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var items = value as IEnumerable<HomeViewModel>;

			var collection = new ColumnDefinitionCollection();
			foreach (var item in items)
			{
				collection.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}
			return collection;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
