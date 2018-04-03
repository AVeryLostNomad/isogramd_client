using System;
using System.Threading;
using Isogramd.iOS.Util;
using Isogramd.Util;


[assembly: Xamarin.Forms.Dependency(typeof(IOSCloseApplication))]
namespace Isogramd.iOS.Util
{
    public class IOSCloseApplication : ICloseApplication
    {
        public IOSCloseApplication(){}

        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}
