using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ZoomCloser.Services
{
    /// <summary>
    /// An abstract class that provides a base implementation for a service that can be used to read and write data.
    /// Inherit from this class to create a service that can read and write settings.
    /// </summary>
    /// <typeparam name="T">Settings type.</typeparam>
    public abstract class SettingsBase<T> : INotifyPropertyChanged where T : SettingsBase<T>, new()
    {
        #region Paths
        public string DirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetName().Name);
        public string FileName { get; protected set; } = "settings.json";
        public string FilePath => Path.Combine(DirectoryPath, FileName);
        #endregion Paths
        
        public event PropertyChangedEventHandler PropertyChanged;
        public SettingsBase()
        {
            //this.PropertyChanged += OnPropertyChanged;
        }

        public bool AutoSave { get; set; } = true;

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T().Read();
                }
                return instance;
            }
        }

        protected static void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var settingsBase = (T)sender;
            if (settingsBase.isDeserializing)
            {
                return;
            }
            if (settingsBase.AutoSave)
            {
                settingsBase.Save(settingsBase);
            }
        }

        private T Read()
        {
            if (File.Exists(FilePath))
            {
                var text = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<T>(text);
            }
            var settings = new T();
            Save(settings);
            return settings;
        }
        
        private void Save(T settings)
        {
            var text = JsonConvert.SerializeObject(settings);
            Directory.CreateDirectory(DirectoryPath);
            using (StreamWriter sw = File.CreateText(FilePath))
            {
                sw.Write(text);
            }
        }
        private bool isDeserializing = false;
        [OnDeserializing]        
        internal void OnThisDeserializing(StreamingContext context)
        {
            isDeserializing = true;
        }
        [OnDeserialized]
        internal void OnThisDeserialized(StreamingContext context)
        {
            isDeserializing = false;
        }
    }
}
