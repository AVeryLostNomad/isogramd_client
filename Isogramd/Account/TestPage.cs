using System;
using Xamarin.Forms;

namespace Isogramd.Account
{
    public class TestPage : ContentPage
    {
        public TestPage() 
        {
            StackLayout test = new StackLayout();

            Label testLabel = new Label()
            {
                Text = "TestLabel",
            };
            test.Children.Add(testLabel);
            this.Content = test;
        }
    }
}
