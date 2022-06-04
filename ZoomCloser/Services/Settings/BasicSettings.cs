/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
namespace ZoomCloser.Services.Settings
{
    public class BasicSettings : SettingsBase<BasicSettings>
    {
        public int BitRate { get; set; } = 3000 * 1000;
        public double Ratio { get; set; } = 0.7;
        public string Culture { get; set; } = "en";
    }
}
