using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isogramd.Carousel.Pages
{
	abstract class BaseGameCarouselPage : BaseCarouselPage
	{

		protected TabbedGameExperience carouselInstance;

		public void Provide_Carousel_Instance(TabbedGameExperience instance)
		{
			this.carouselInstance = instance;
		}

	}
}
