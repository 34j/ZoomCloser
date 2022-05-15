using Gu.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using ZoomCloser.Properties;
using ZoomCloser.Services;

namespace ZoomCloser.Utils
{
    public static class CultureUtils
    {
        public static IEnumerable<CultureInfo> GetAvailableCultures()
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

        public static void InitTranslator()
        {
            foreach (CultureInfo culture in GetAvailableCultures())
            {
                _ = Translator.Cultures.Add(culture);
            }

            var settingCulture = new CultureInfo(BasicSettings.Instance.Culture);
            if (!Translator.Cultures.Contains(settingCulture))
            {
                settingCulture = Translator.Cultures.First();
                BasicSettings.Instance.Culture = settingCulture.Name;
            }
            Translator.Culture = settingCulture;

            Translator.CurrentCultureChanged += (sender, e) =>
            {
                BasicSettings.Instance.Culture = e.Culture.Name;
            };
        }
    }
}
