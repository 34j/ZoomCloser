/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System.Reflection;
using System.IO;
using Syroot.Windows.IO;
using System;

namespace ZoomCloser.Modules
{
    internal static class StartUpHandler
    {
        private static string StartUpFolderPath => KnownFolders.RoamingAppData.Path + @"\Microsoft\Windows\Start Menu\Programs\Startup";
        public static string ShortcutPath(bool isUrl = false) => isUrl 
            ? Path.Combine(StartUpFolderPath, ApplicationName + ".url")
            : Path.Combine(StartUpFolderPath, ApplicationName + ".lnk");
        private static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name;
        private static string ApplicationPath
        {
            get
            {
                string path = Environment.ProcessPath;
                if (Path.GetExtension(path) != ".exe")
                {
                    throw new Exception("Application path is not an exe");
                }
                return path;
            }
        }
        /// <summary>
        /// Registers this application to start up on windows startup.
        /// </summary>
        public static void RegisterThisToStartUpUrl()
        {
            using (StreamWriter sw = new(ShortcutPath(true)))
            {
                sw.WriteLine("[InternetShortcut]");
                sw.WriteLine($"URL={ApplicationPath}");
            }
        }
        /// <summary>
        /// Unregisters this application from start up.
        /// </summary>
        public static void UnregisterThisFromStartUpUrl()
        {
            string path = ShortcutPath(true);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        /// <summary>
        /// Registers this application to start up on windows startup.
        /// </summary>
        public static void RegisterThisToStartUp()
        {
            var wsh = new IWshRuntimeLibrary.IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(ShortcutPath(false));
            shortcut.TargetPath = ApplicationPath;
            shortcut.IconLocation = ApplicationPath + ",0";
            shortcut.Save();
        }
        /// <summary>
        /// Unregisters this application from start up.
        /// </summary>
        public static void UnregisterThisFromStartUp()
        {
            string path = ShortcutPath(false);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
