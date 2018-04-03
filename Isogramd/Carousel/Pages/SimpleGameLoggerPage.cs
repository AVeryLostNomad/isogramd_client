using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isogramd.UI;
using Xamarin.Forms;

namespace Isogramd.Carousel.Pages
{

    class SimpleMove {
        private string guess;
        public string Guess {
            get { return guess; }
            set { guess = value; }
        }

        private int bulls;
        public int Bulls {
            get { return bulls; }
            set { bulls = value; }
        }

        private int cows;
        public int Cows {
            get { return cows; }
            set { cows = value; }
        }

        private string statusMsg;
        public string Status_Message {
            get { return "Bulls: " + bulls.ToString() + " Cows: " + cows.ToString(); }
        }


    }

	class SimpleGameLoggerPage : BaseGameCarouselPage
	{
		public override bool Is_Code_Match(string codeLetter)
		{
			return codeLetter.Equals("L");
		}

		public override HomeViewModel Return_Model()
        {
            System.Diagnostics.Debug.WriteLine("Made new logger page.");
			RelativeLayout layout = new RelativeLayout();

            Add_Outer_Border(layout); //Add a border to this page

            List<SimpleMove> move_list = App.Get_Data_Store().Has_Item("move_list") ? (List<SimpleMove>)App.Get_Data_Store().Fetch("move_list") : new List<SimpleMove>();

			var moveListView = new ListView
			{
                ItemsSource = move_list,
				RowHeight = 50,
				HasUnevenRows = true
			};

            moveListView.ItemTemplate = new DataTemplate(() => {
                LoggerMoveCell cell = new LoggerMoveCell();
                cell.SelectedBackgroundColor = Color.LightGray;

                cell.Tapped += (sender, args) =>
                {
					//await OnListViewTextCellTapped(cell); //We could do things with this tap
				};

                return cell;
            });
			moveListView.ItemTemplate.SetBinding(TextCell.TextProperty, "Guess");
			moveListView.ItemTemplate.SetBinding(TextCell.DetailProperty, "Status_Message");

            layout.Children.Add(moveListView, Constraint.Constant(20), Constraint.Constant(20), Constraint.RelativeToParent((p) => { return p.Width - 40; }), Constraint.RelativeToParent((p) => { return p.Height - 40; }));

			return new HomeViewModel
            {
                Background = Color.WhiteSmoke,
                ImageSource = "Isogramd.Images.Book.png",  // The tab icon
                Content = layout,
                TabText = "Log",
                PageType = this
			};
		}
	}
}
