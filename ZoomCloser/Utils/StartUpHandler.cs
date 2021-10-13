using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace ZoomCloser.Modules
{
    internal static class StartUpHandler
    {
        public static string StartUpFolderPath => KnownFolders.RoamingAppData.Path + @"\Microsoft\Windows\Start Menu\Programs\Startup";
        public static void AddThisToStartUp()
        {
            var assembly = Assembly.GetEntryAssembly();
            string applicationName = assembly.GetName().Name;
            string filePath = StartUpFolderPath + @"\" + applicationName + ".url";
            string applicationPath = assembly.Location;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("[InternetShortcut]");
                sw.WriteLine("URL=" + applicationPath);
            }
        }
    }
}
