using System;
using Isogramd.Carousel;
using Xamarin.Forms;

namespace Isogramd.Function
{
    public class QueueFailedExperience : ContentPage
    {
        public QueueFailedExperience()
        {
            StackLayout layout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = 10
            };

            Label sorryLabel = new Label()
            {
                Text = "Sorry!",
                FontSize = 24,
                TextColor = Color.LightBlue,
                FontAttributes = FontAttributes.Bold,
            };
            layout.Children.Add(sorryLabel);

            Label explanation = new Label()
            {
                Text = "We couldn't find a match for you in ten minutes.",
                FontSize = 16,
            };
            layout.Children.Add(explanation);

            Label consequences = new Label()
            {
                Text = "You will be unable to queue for this gamemode for ten minutes, but a Marque has been distributed to you. In 24 hours, you will be able to redeem it for fairly high rewards. We appreciate your understanding.",
                FontSize = 16,
            };
            layout.Children.Add(consequences);

            Button understood = new Button()
            {
                Text = "I Understand",
                BackgroundColor = Color.WhiteSmoke,
                BorderColor = Color.PowderBlue,
                TextColor = Color.DimGray,
                BorderWidth = (Double)1.5,
                BorderRadius = 5,
                Font = Font.SystemFontOfSize(NamedSize.Large),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            understood.Clicked += (sender, e) =>
            {
                App.GetNavigation().PushAsync
                         (new TabbedCarouselExperience());
            };

            this.Content = layout;
        }
    }
}
