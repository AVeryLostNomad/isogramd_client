using System;
namespace Isogramd.Util
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
        void Save_String(string filename, string text);
        string Read_String(string filename);
    }
}
