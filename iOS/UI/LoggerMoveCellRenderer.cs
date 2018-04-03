using System;
using Isogramd.UI;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LoggerMoveCell), typeof(Isogramd.iOS.UI.LoggerMoveCellRenderer))]
namespace Isogramd.iOS.UI
{
	public class LoggerMoveCellRenderer : TextCellRenderer
	{
		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);
            var view = item as LoggerMoveCell;
			cell.SelectedBackgroundView = new UIView
			{
				BackgroundColor = view.SelectedBackgroundColor.ToUIColor(),
			};

			return cell;
		}
	}
}
