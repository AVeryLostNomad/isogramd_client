using System;
using System.IO;
using Isogramd.Droid.Util;
using Isogramd.Util;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace Isogramd.Droid.Util
{
	public class FileHelper : IFileHelper
	{
		public string GetLocalFilePath(string filename)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(path, filename);
		}
		
		public void Save_String(string filename, string text)
		{
			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			System.IO.File.WriteAllText (filePath, text);
		}

		public string Read_String(string filename)
		{
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			var filePath = Path.Combine (documentsPath, filename);
			try
			{
				return System.IO.File.ReadAllText(filePath);
			}
			catch (Exception e)
			{
				return "ERROR";
			}
		}
	}
	
}
