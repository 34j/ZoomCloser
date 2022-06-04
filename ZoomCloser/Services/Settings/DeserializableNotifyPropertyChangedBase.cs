/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using PropertyChanged;

namespace ZoomCloser.Services.Settings
{
    /// <summary>
    /// Base class for deserializable class implementing <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public abstract class DeserializableNotifyPropertyChangedBase : INotifyPropertyChanged, IJsonOnDeserializing, IJsonOnDeserialized
    {
        /// <summary>
        /// Whether the class is currently being deserialized.
        /// </summary>
        [JsonIgnore]
        [DoNotNotify]
        public bool IsDeserializing { get; set; }
        public void OnDeserializing()
        {
            IsDeserializing = true;
            Debug.WriteLine(IsDeserializing);
        }

        public void OnDeserialized()
        {
            IsDeserializing = false;
            Debug.WriteLine(IsDeserializing);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (IsDeserializing)
            {
                return;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}