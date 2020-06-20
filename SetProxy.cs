using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

namespace SetProxy
{

    class Program
    {

        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;


        static void setProxy(string proxyhost, bool proxyEnabled)
        {
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings";
            const string keyName = userRoot + "\\" + subkey;

            Registry.SetValue(keyName, "ProxyServer", proxyhost);
            Registry.SetValue(keyName, "ProxyEnable", proxyEnabled ? 1 : 0);

            // These lines implement the Interface in the beginning of program 
            // They cause the OS to refresh the settings, causing IP to realy update
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                setProxy("", false);
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    Log("Set Proxy OFF", w);
                }
                return;
            }

            setProxy(args[0], true);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log("Set proxy ON", w);
            }
        }



        public static void Log(string logMessage, TextWriter w)
        {
            // w.Write("\r\nLog Entry : ");
            w.Write($"{DateTime.Now.ToString() }");
            w.Write($"  :{logMessage}");
            w.WriteLine(" ");
        }
    }
}
