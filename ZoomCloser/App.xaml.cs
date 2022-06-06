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
using System;

namespace ZoomCloser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override FrameworkElement CreateElement()
        {
            return Container.Resolve<MainTaskbarIcon>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CultureUtils.InitTranslator();
            
            ThemeService.Current.EnableUwpResoruces();
            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);

            Modules.StartUpHandler.RegisterThisToStartUp();        

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            foreach (var type in AllClasses.FromLoadedAssemblies())
            {
                foreach (Type interFace in WithMappings.FromAllInterfaces(type))
                {
                    containerRegistry.Register(interFace, type, WithName.Default(type));
                }
                containerRegistry.Register(type);
            }
        }
    }
}
