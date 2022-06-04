/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using Gu.Localization;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZoomCloser.Utils
{
    /// <summary>
    /// you can use as ReadOnlyObservableCollection<string>
    /// </summary>
    public class ReadOnlyObservableTranslationCollection : ReadOnlyObservableCollection<string>
    {
        public ReadOnlyObservableTranslationCollection() : base(new ObservableCollection<string>())
        {
            Translator.CurrentCultureChanged += (sender, e) => Reset();
            Translations.CollectionChanged += (sender, e) => Reset();
        }

        private void Reset()
        {
            Items.Clear();
            foreach(var translation in Translations)
            {
                Items.Add(translation.Item2(string.Format(translation.Item1.Translate(Translator.CurrentCulture), translation.Item3)));
            }
        }

        /// <summary>
        /// internal translation collection
        /// </summary>
        public ObservableCollection<(ITranslation, Func<string, string>, object[])> Translations { get; set; } = new ObservableCollection<(ITranslation, Func<string, string>, object[])>();
        public void Add(ITranslation translation, Func<string, string> translationToDisplayFunc, params object[] translationParams)
        {
            Translations.Add((translation, translationToDisplayFunc, translationParams.Select(s => s.ToString()).ToArray()));
        }
    }
}
