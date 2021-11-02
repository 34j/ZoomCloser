using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Globalization;

namespace ZoomCloser.Services
{
    public static class SettingsService
    {
        public static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetName().Name);
       public static string FilePath => Path.Combine(DirectoryPath,  "settings.json");

        static Settings settings = null;
        public static Settings V
        {
            get
            {
                if (settings == null)
                {
                    settings = Read();
                }
                return settings;
            }
        }

        static Settings Read()
        {
            if (File.Exists(FilePath))
            {
                var text = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<Settings>(text);
            }
            settings = new Settings();
            Save();
            return settings;
        }

        public static void Save()
        {
            var text = JsonSerializer.Serialize(settings);
            Directory.CreateDirectory(DirectoryPath);
            using (var sw = File.CreateText(FilePath))
            {
                sw.Write(text);
            }
        }

    }

    public class Settings
    {
        public int BitRate { get; set; } = 3000 * 1000;
        public double Ratio { get; set; } = 0.7;
        public string Culture { get; set; } = "en";
    }
}
