/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using PropertyChanged;

namespace ZoomCloser.Services.Settings
{
    /// <summary>
    /// An abstract class that provides a base implementation for a service that can be used to read and write data.
    /// Inherit from this class to create a service that can read and write settings.
    /// </summary>
    /// <typeparam name="T">Settings type.</typeparam>
    public abstract class SettingsBase<T> : DeserializableNotifyPropertyChangedBase where T : SettingsBase<T>, new()
    {
        #region Paths

        /// <summary>
        /// The name of the file that contains the settings.
        /// </summary>
        /// This class uses a lot of static properties. It is frequently said that this is a bad practice, but it is inavoidable because of the way the class is designed. 
        /// In other words, this class is a singleton. If there were more than one path to the settings file, this would be a problem.
        public static string FileName { get; protected set; } = $"{typeof(T)}.json";
        /// <summary>
        /// The path of the directory where the settings file is stored.
        /// </summary>
        public static string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetName().Name);
        /// <summary>
        /// The full path to the file that contains the settings.
        /// </summary>
        public static string FilePath => Path.Combine(DirectoryPath, FileName);
        #endregion Paths

        #region Singleton
        private static T instance;
        /// <summary>
        /// The instance of the settings.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Read();
                }
                return instance;
            }
        }
        #endregion Singleton
        public SettingsBase()
        {
            this.PropertyChanged += OnThisPropertyChanged;
        }



        #region AutoSave

        /// <summary>
        /// Whether to automatically save the settings when <see cref="DeserializableNotifyPropertyChangedBase.OnPropertyChanged"/> is called.
        /// </summary>
        [DoNotNotify]
        [JsonIgnore]
        public bool AutoSave { get; set; } = true;
        [SuppressPropertyChangedWarnings]
        private static void OnThisPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var settingsBase = (T)sender;
            if (settingsBase.AutoSave)
            {
                Save(settingsBase);
            }
        }
        #endregion AutoSave

        #region I/O

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
        #endregion I/O
    }
}
