using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Globalization;
using System.ComponentModel;

namespace ZoomCloser.Services
{
    /// <summary>
    /// An abstract class that provides a base implementation for a service that can be used to read and write data.
    /// Inherit from this class to create a service that can read and write settings.
    /// </summary>
    /// <typeparam name="T">Settings type.</typeparam>
    public abstract class SettingsBase<T> : INotifyPropertyChanged where T : SettingsBase<T>, new()
    {
        public static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetName().Name);
        public static string FilePath => Path.Combine(DirectoryPath, "settings.json");

        public event PropertyChangedEventHandler PropertyChanged;
        protected SettingsBase()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        public bool AutoSave { get; set; } = true;

        public static T Instance => Read();

        protected static void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var settingsBase = (T)sender;
            if (settingsBase.AutoSave)
            {
                Save(settingsBase);
            }
        }

        private static T Read()
        {
            if (File.Exists(FilePath))
            {
                var text = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<T>(text);
            }
            var settings = new T();
            Save(settings);
            return settings;
        }

        private static void Save(T settings)
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
