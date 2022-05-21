/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using ZoomCloser.Views;
using Prism.Ioc;
using System.Windows;
using MetroRadiance.UI;
using MetroRadiance.UI.Controls;
using System;
using ZoomCloser.Utils;
using Autofac;
using System.Reflection;

namespace ZoomCloser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected UIElement MainElement { get; private set; }
        protected IContainer Container { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            CultureUtils.InitTranslator();
            ThemeService.Current.EnableUwpResoruces();
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

            Modules.StartUpHandler.AddThisToStartUp();
            
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
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(executingAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces();
            this.Container = builder.Build();
        }
    }
}
