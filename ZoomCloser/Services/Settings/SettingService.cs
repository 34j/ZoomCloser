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
        public static string FilePath => Path.Combine(DirectoryPath, "settings.json");

        private static Settings settings;
        public static Settings Instance => settings ?? (settings = Read());

        private static bool autoSave = false;
        public static bool AutoSave
        {
            set
            {
                if (value && !autoSave)
                {
                    Instance.PropertyChanged += Instance_PropertyChanged;
                }
                else if (!value && autoSave)
                {
                    Instance.PropertyChanged -= Instance_PropertyChanged;
                }
                autoSave = value;
            }
            get => autoSave;
        }

        private static void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Save();
        }

        private static Settings Read()
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
            using (StreamWriter sw = File.CreateText(FilePath))
            {
                sw.Write(text);
            }
        }
    }
}
