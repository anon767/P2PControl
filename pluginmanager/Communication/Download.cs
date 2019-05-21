using System;
using System.Runtime.InteropServices;

namespace pluginmanager.Communication
{
    public class Download
    {

        [DllImport("URLMON.DLL", EntryPoint = "URLDownloadToFileW", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        public static extern int URLDownloadToFile(int pCaller, string srcURL,
            string dstFile, int Reserved, int CallBack);
            
    }
}
