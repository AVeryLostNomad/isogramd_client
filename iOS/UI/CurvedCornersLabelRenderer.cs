using System;
using Isogramd.UI;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CurvedCornersLabel), typeof(Isogramd.iOS.UI.CurvedCornersLabelRenderer))]
namespace Isogramd.iOS.UI
{
    public class CurvedCornersLabelRenderer : LabelRenderer
    {
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
                var _xfViewReference = (CurvedCornersLabel)Element;

				// Radius for the curves
				this.Layer.CornerRadius = (float)_xfViewReference.CurvedCornerRadius;

				this.Layer.BackgroundColor = _xfViewReference.CurvedBackgroundColor.ToCGColor();
			}
		}
    }
}
