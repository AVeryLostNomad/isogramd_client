using Isogramd.Carousel.Pages;
using Isogramd.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Isogramd.Carousel
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabbedGameExperience : ContentPage
	{
		View _tabs;

		RelativeLayout relativeLayout;

		CarouselLayout.IndicatorStyleEnum _indicatorStyle;

		SwitcherPageViewModel viewModel;

		List<BaseGameCarouselPage> carouselPageTypes = new List<BaseGameCarouselPage>
		{
			new SimpleGuesserPage(),
			new SimpleGameLoggerPage()
		};

        //Create a brand new, context free Game Experience
        public TabbedGameExperience()
        {
			_indicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs;

			List<HomeViewModel> pages = CreatePages().Result;
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

        public TabbedGameExperience(TabbedGameExperience previousContext)
		{
            System.Diagnostics.Debug.WriteLine("Doing previous init");
			_indicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs;

            List<HomeViewModel> pages = CreatePages().Result;
			viewModel = new SwitcherPageViewModel(pages);
            viewModel.CurrentPage = previousContext.Get_Current_Page(); //Line to duplicate current page selection.
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

        public HomeViewModel Get_Current_Page(){
            return ((SwitcherPageViewModel)BindingContext).CurrentPage;
        }

		async Task<List<HomeViewModel>> CreatePages()
		{
			List<HomeViewModel> to_return = new List<HomeViewModel>();

			System.Diagnostics.Debug.WriteLine("Starting pages");
			String pattern = "";
			await App.Get_Data_Store().UpdateUserData();
			System.Diagnostics.Debug.WriteLine("Done updating user data");
			if (App.Get_Data_Store().Has_Item("profile_dict"))
			{
				pattern = (string)((Dictionary<String, object>)App.Get_Data_Store().Fetch("profile_dict"))["game_pages"];
			}
			else
			{
				pattern = (string)((Dictionary<String, object>)App.Get_Data_Store().Fetch("initial_profile_dict"))["game_pages"];
			}
			System.Diagnostics.Debug.WriteLine("Pages: " + pattern);
			try
			{
				foreach (char c in pattern)
				{
					System.Diagnostics.Debug.WriteLine("Char: " + c);
					foreach (BaseGameCarouselPage page in carouselPageTypes)
					{
						if (page.Is_Code_Match(String.Join("", c)))
						{
							System.Diagnostics.Debug.WriteLine("Added page");
							page.Provide_Carousel_Instance(this);
							to_return.Add(page.Return_Model());
							break;
						}
					}
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.Message);
			}
			return to_return;
		}

		CarouselLayout CreatePagesCarousel()
		{
			var carousel = new CarouselLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Tabs,
				ItemTemplate = new DataTemplate(typeof(HomeView))
			};
			carousel.SetBinding(CarouselLayout.ItemsSourceProperty, "Pages");
			carousel.SetBinding(CarouselLayout.SelectedItemProperty, "CurrentPage", BindingMode.TwoWay);
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
}
