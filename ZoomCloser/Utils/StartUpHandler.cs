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
        public static void AddThisToStartUpUrl()
        {
            var assembly = Assembly.GetEntryAssembly();
            string applicationName = assembly.GetName().Name;

            string shortcutPath = StartUpFolderPath + @"\" + applicationName + ".url";
            string applicationPath = assembly.Location;
            using (StreamWriter sw = new StreamWriter(shortcutPath))
            {
                sw.WriteLine("[InternetShortcut]");
                sw.WriteLine("URL=" + applicationPath);
            }



        }

        public static void AddThisToStartUp()
        {
            var assembly = Assembly.GetEntryAssembly();
            string applicationName = assembly.GetName().Name;

            string shortcutPath = StartUpFolderPath + @"\" + applicationName + ".lnk";
            string applicationPath = assembly.Location;

            var wsh = new IWshRuntimeLibrary.IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(shortcutPath);
            shortcut.TargetPath = applicationPath;
            shortcut.IconLocation = applicationPath + ",0";
            shortcut.Save();

            var oldShortcutPath = StartUpFolderPath + @"\" + applicationName + ".url";
            if (File.Exists(oldShortcutPath))
            {
                File.Delete(oldShortcutPath);
            }
        }
    }
}
