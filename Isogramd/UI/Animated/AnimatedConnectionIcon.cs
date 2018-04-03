using System;
using Xamarin.Forms;

namespace Isogramd.UI.Animated
{
    public class AnimatedConnectionIcon : ContentView
    {
		private StackLayout _layout;
        private Image _embeddedImage;

        double startRotation = - 45;
        double startScale = 1.0;

        //Constructor for the main icon. 
		public AnimatedConnectionIcon(String imageSource)
		{
			_layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Horizontal,
				Padding = 5,
			};

			_embeddedImage = new Image { Aspect = Aspect.AspectFit };
            _embeddedImage.Source = ImageSource.FromResource(imageSource);
            _layout.Children.Add(_embeddedImage);

            AnchorX = 0.1543;
            AnchorY = 0.8438;

            Rotation = startRotation;
            Scale = startScale;

			// set the content
			this.Content = _layout;
		}

		public void AnimateDawdle()
		{
			var a = new Animation();

            var rotateRight = new Animation((v) =>
            {
                this.Rotation = v;
            }, this.startRotation, this.startRotation + 45, Easing.BounceOut);

            var rotateBackToStartRight = new Animation((v) =>
            {
                this.Rotation = v;
            }, this.startRotation + 45, this.startRotation, Easing.SinInOut);

            var rotateLeft = new Animation((v) =>
            {
                this.Rotation = v;
			}, this.startRotation, this.startRotation - 45, Easing.BounceOut);

			var rotateBackToStartLeft = new Animation((v) =>
			{
				this.Rotation = v;
            }, this.startRotation - 45, this.startRotation, Easing.CubicInOut);

            var scaleUp = new Animation((v) =>
            {
                this.Scale = v;
            }, this.startScale, this.startScale + 0.2);
			
            var scaleDown = new Animation((v) =>
			{
				this.Scale = v;
            }, this.startScale + 0.2, this.startScale);

            a.Add(0.0, 0.26, rotateRight);
            a.Add(0.24, 0.51, rotateBackToStartRight);
            a.Add(0.49, 0.76, rotateLeft);
            a.Add(0.74, 1.00, rotateBackToStartLeft);
            a.Add(0.0, 0.5, scaleUp);
            a.Add(0.5, 1.00, scaleDown);
			a.Commit(this, "animation", 8, 4000, null, (d, f) =>
			{
				this.Scale = 1.0;
				//System.Diagnostics.Debug.WriteLine("ANIMATION ALL");
				AnimateDawdle();
			});
		}

    }
}
