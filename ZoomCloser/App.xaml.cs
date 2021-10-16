/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using ZoomCloser.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using MetroRadiance.UI;
using MetroRadiance.UI.Controls;

using ZoomCloser.Services;
using Gu.Localization;
using System.Globalization;
using ZoomCloser.Services.Audio;

namespace ZoomCloser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MetroWindow metroWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Translator.Cultures.Add(new CultureInfo("en"));
            Translator.Cultures.Add(new CultureInfo("ja"));
            ThemeService.Current.EnableUwpResoruces();
            ThemeService.Current.Register(this, Theme.Dark, Accent.Windows);

        }

        protected override Window CreateShell()
        {
            metroWindow = Container.Resolve<MainWindow>();
            Modules.StartUpHandler.AddThisToStartUp();
            return metroWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IZoomHandlingService, ZoomHandlingService>()
                            .Register<IReadOnlyZoomHandlingService, ZoomHandlingService>()
                            .Register<IZoomExitService, ZoomExitService>()
                            .Register<IZoomExitByRatioService, ZoomExitByRatioService>()
                            .Register<IJudgingWhetherToExitByRatioService, JudgingWhetherToExitByRatioService>()
                            .Register<IJudgingWhetherToExitService, JudgingWhetherToExitByRatioService>()
                            .Register<IAudioService, AudioService>();
        }
    }
}
