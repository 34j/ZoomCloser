using Gu.Localization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using ZoomCloser.Properties;
using ZoomCloser.Services;

namespace ZoomCloser.Utils
{
    public static class CultureUtils
    {
        /// <summary>
        /// Get all supported cultures.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CultureInfo> GetAllAvailableCultures()
        {
            List<CultureInfo> result = new List<CultureInfo>();

            ResourceManager rm = new ResourceManager(typeof(Resources));

            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                    {
                        //continue; //do not use "==", won't work
                    }

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        result.Add(culture);
                    }
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            return result;
        }

        /// <summary>
        /// Refresh <see cref="Translator.Cultures"/> and <see cref="Translator.CurrentCulture"/>, <see cref="BasicSettings.Culture"/>.
        /// </summary>
        public static void InitTranslator()
        {
            Translator.Cultures.Clear();
            foreach (CultureInfo culture in GetAllAvailableCultures())
            {
                Debug.WriteLine($"Adding {culture.Name}");
                _ = Translator.Cultures.Add(culture);
            }
            Debug.WriteLine(Translator.Cultures.ToString());


            CultureInfo settingCulture = null;

            // Try to get the culture from settings
            bool isCultureValid = true;
            try
            {
                settingCulture = new CultureInfo(BasicSettings.Instance.Culture);
            }
            catch (CultureNotFoundException)
            {
                isCultureValid = false;
            }
            
            if (!isCultureValid || !Translator.Cultures.Contains(settingCulture))
            {
                settingCulture = Translator.Cultures.First();
                BasicSettings.Instance.Culture = settingCulture.Name;
            }
            Translator.Culture = settingCulture;

            //Save to settings when current culture is changed
            Translator.CurrentCultureChanged += (sender, e) =>
            {
                BasicSettings.Instance.Culture = e.Culture.Name;
            };
        }
    }
}
