using System;
using Xamarin.Forms;
namespace Isogramd.UI.Animated
{
    public class AnimatedLabel : ContentView
    {
        private StackLayout _layout;
		private Label _embeddedLabel = new Label();

		new public double Opacity
		{
			get { return _embeddedLabel.Opacity; }
			set { _embeddedLabel.Opacity = value; }
		}

        public TextAlignment TextAlignmentHorizontal {
            get { return _embeddedLabel.HorizontalTextAlignment; }
            set { _embeddedLabel.HorizontalTextAlignment = value; }
        }
			
		public string Text
		{
			get { return _embeddedLabel.Text; }
			set { _embeddedLabel.Text = value; }
		}

		public LayoutOptions HorizontalLayout
		{
			get { return _embeddedLabel.HorizontalOptions; }
			set { _embeddedLabel.HorizontalOptions = value; }
		}

		public double FontSize
		{
			get { return _embeddedLabel.FontSize; }
			set { _embeddedLabel.FontSize = value; }
		}

		public FontAttributes FontAttributes
		{
			get { return _embeddedLabel.FontAttributes; }
			set { _embeddedLabel.FontAttributes = value; }
		}

		public Color FontColor
		{
			get { return _embeddedLabel.TextColor; }
			set { _embeddedLabel.TextColor = value; }
		}

        public AnimatedLabel()
        {
			_layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Horizontal,
				Padding = 5,
			};

			_embeddedLabel = new Label
			{
				Text = this.Text,
				FontSize = this.FontSize,
				FontAttributes = this.FontAttributes,
	            HorizontalTextAlignment = TextAlignment.Start,
            };
            _layout.Children.Add(_embeddedLabel);

			// set the content
			this.Content = _layout;
		}

        public void AnimatePop() 
        {
            var a = new Animation();

            var pop = new Animation((v) =>
            {
                this.Scale = v;
            }, 1.0, 1.10);

            var pull = new Animation((v) =>
            {
                this.Scale = v;
            }, 1.10, 1.0);

            a.Add(0.0, 0.75, pop);
            a.Add(0.75, 1.0, pull);
            a.Commit(this, "popanimation", 8, 800, null, null);
        }

		public void AnimateFadeIn()
		{
			var a = new Animation();

			var fadeIn = new Animation((v) =>
			{
				this.Opacity = v;
			}, 0.00, 1.0, Easing.CubicIn);

			a.Add(0.0, 1.00, fadeIn);
			a.Commit(this, "fianimation", 8, 800, null, null);
		}

		public void AnimateFadeOut()
		{
			var a = new Animation();

			var fadeOut = new Animation((v) =>
			{
				this.Opacity = v;
			}, 1.00, 0.0, Easing.CubicIn);

			a.Add(0.0, 1.00, fadeOut);
			a.Commit(this, "foanimation", 8, 1000, null, null);
		}
	    
	    public void AnimatePulse()
	    {
		    var a = new Animation();

		    var scaleUp = new Animation((v) =>
		    {
			    this.Scale = v;
		    }, 1.0, 1.05, Easing.SinInOut);

		    var scaleDown = new Animation((v) =>
		    {
			    this.Scale = v;
		    }, 1.05, 1.0, Easing.SinInOut);
		    
		    a.Add(0.0, 0.5, scaleUp);
		    a.Add(0.5, 1.00, scaleDown);
		    a.Commit(this, "animation", 8, 800, null, (d, f) =>
		    {
			    this.Scale = 1.0;
			    //System.Diagnostics.Debug.WriteLine("ANIMATION ALL");
			    AnimatePulse();
		    });
	    }

		public void ResetFade()
		{
			_embeddedLabel.Opacity = 1.0;
		}
    }
}
