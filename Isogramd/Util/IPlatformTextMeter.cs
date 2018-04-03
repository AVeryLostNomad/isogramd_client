using System;
namespace Isogramd.Util
{
    public interface IPlatformTextMeter
    {
        Xamarin.Forms.Size MeasureTextSize(string text, double width,
            double fontSize, string fontName = null);
    }
}
