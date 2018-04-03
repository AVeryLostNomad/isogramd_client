using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Isogramd.Util;

[assembly: ExportRenderer(typeof(ShapeView), typeof(Isogramd.Droid.UI.ShapeRenderer))]
namespace Isogramd.Droid.UI
{
	public class ShapeRenderer : ViewRenderer<ShapeView, Shape>
	{
		public ShapeRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<ShapeView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;

			SetNativeControl(new Shape(Resources.DisplayMetrics.Density, Context)
			{
				ShapeView = Element
			});
		}
	}
}