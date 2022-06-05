/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using ZoomCloser.Views;
using Prism.Ioc;
using System.Windows;
using MetroRadiance.UI;
using ZoomCloser.Utils;
using System.Reflection;
using Unity.RegistrationByConvention;
using Unity;

namespace ZoomCloser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected UIElement MainElement { get; private set; }
        protected IUnityContainer Container { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            CultureUtils.InitTranslator();
            ThemeService.Current.EnableUwpResoruces();
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

            Modules.StartUpHandler.RegisterThisToStartUp();
            
            CreateContainer();
            this.MainElement = Container.Resolve<MainWindow>();

            base.OnStartup(e);
        }

        /// <summary>
        /// Create Container.
        /// </summary>
        private void CreateContainer()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            Container = new UnityContainer();
            Container.RegisterTypes(AllClasses.FromLoadedAssemblies(), WithMappings.FromMatchingInterface, WithName.Default);
        }
    }
}
