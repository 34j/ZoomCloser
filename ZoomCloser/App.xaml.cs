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

namespace ZoomCloser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MetroWindow metroWindow; 
        protected override Window CreateShell()
        {
            metroWindow = Container.Resolve<MainWindow>();
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
            Modules.StartUpHandler.AddThisToStartUp();
            return metroWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
