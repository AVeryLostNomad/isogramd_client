using System;
using Android.App;
using Isogramd.Droid.Util;
using Isogramd.Util;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidCloseApplication))]
namespace Isogramd.Droid.Util
{
    public class AndroidCloseApplication : ICloseApplication
    {
        public AndroidCloseApplication(){}

        public void closeApplication()
        {
			Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}
