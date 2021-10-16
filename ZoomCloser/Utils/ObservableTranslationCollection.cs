using Gu.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomCloser.Utils
{
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
                Items.Add(translation.Item2(translation.Item1.Translate(Translator.CurrentCulture)));
            }
        }

        public ObservableCollection<(ITranslation, Func<string, string>)> Translations { get; set; } = new ObservableCollection<(ITranslation, Func<string, string>)>();
        
    }
}
