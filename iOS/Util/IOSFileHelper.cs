using System;
using System.IO;
using Isogramd.iOS.Util;
using Isogramd.Util;
using static System.IO.Path;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace Isogramd.iOS.Util
{
	public class FileHelper : IFileHelper
	{
		public string GetLocalFilePath(string filename)
		{
			string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string libFolder = Combine(docFolder, "..", "Library", "Databases");

			if (!Directory.Exists(libFolder))
			{
				Directory.CreateDirectory(libFolder);
			}

			return Combine(libFolder, filename);
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
