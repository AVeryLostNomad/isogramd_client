using System;
using Xamarin.Forms;

namespace Isogramd.UI
{
    public class LoggerMoveCell : TextCell
    {
        // <summary>
        // The SelectedBackgroundColor property.
        // </summary>
        public static readonly BindableProperty SelectedBackgroundColorProperty =
            BindableProperty.Create("SelectedBackgroundColor", typeof(Color), typeof(LoggerMoveCell), Color.Default);

        // <summary>
        // Gets or sets the SelectedBackgroundColor.
        // </summary>
        public Color SelectedBackgroundColor
        {
            get { return (Color)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }
    }
}
