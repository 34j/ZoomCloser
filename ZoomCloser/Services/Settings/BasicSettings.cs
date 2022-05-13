using System.ComponentModel;

namespace ZoomCloser.Services
{
    public class BasicSettings : SettingsBase<BasicSettings>
    {
        public int BitRate { get; set; } = 3000 * 1000;
        public double Ratio { get; set; } = 0.7;
        public string Culture { get; set; } = "en";
    }
}
