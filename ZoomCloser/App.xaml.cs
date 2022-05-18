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
using Unity;
using Unity.RegistrationByConvention;
using ZoomCloser.Utils;

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
            CultureUtils.InitTranslator();
            base.OnStartup(e);
            ThemeService.Current.EnableUwpResoruces();
            ThemeService.Current.Register(this, Theme.Dark, Accent.Windows);
        }

        protected override Window CreateShell()
        {
            /*   var container = new UnityContainer();
               container.RegisterTypes(
   AllClasses.FromLoadedAssemblies(),
   WithMappings.FromMatchingInterface,
   WithName.Default);*/

            metroWindow = Container.Resolve<MainWindow>();
            Modules.StartUpHandler.AddThisToStartUp();
            return metroWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            foreach (var type in AllClasses.FromLoadedAssemblies())
            {
                foreach (Type interFace in WithMappings.FromAllInterfaces(type))
                {
                    containerRegistry.Register(interFace, type, WithName.Default(type));
                    //Debug.WriteLine(type.Name + interFace.Name);
                }
                containerRegistry.Register(type);
                //Debug.WriteLine(type.Name);
            }
        }


    }
}
