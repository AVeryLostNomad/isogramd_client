using System;
using System.Collections.Generic;
using System.Linq;
using Isogramd.Carousel.Pages;
using Xamarin.Forms;

namespace Isogramd.Carousel
{
	public class SwitcherPageViewModel : BaseViewModel
	{
		public SwitcherPageViewModel(List<HomeViewModel> pages)
		{
			Pages = pages;

			CurrentPage = Pages.First();
		}

		IEnumerable<HomeViewModel> _pages;
		public IEnumerable<HomeViewModel> Pages
		{
			get
			{
				return _pages;
			}
			set
			{
				SetObservableProperty(ref _pages, value);
                CurrentPage = Pages.FirstOrDefault();
			}
		}

		HomeViewModel _currentPage;
		public HomeViewModel CurrentPage
		{
			get
			{
				return _currentPage;
			}
			set
			{
				SetObservableProperty(ref _currentPage, value);
			}
		}
	}

    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel() { }

        public Color Background { get; set; }
        public string ImageSource { get; set; }
        public Layout Content { get; set; }
        public string TabText { get; set; }
        public BaseCarouselPage PageType {get; set;}
	}
}
