using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism;
using Prism.Common;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using Unity.RegistrationByConvention;

namespace ZoomCloser.Utils
{
    /// <summary>
    /// <br>Make sure that <see cref="Application.MainWindow"/> always returns null. Use <see cref="MainElement"/> instead.</br>
    /// <br>Make sure that <see cref="CreateShell"/> is sealed. Use <see cref="CreateElement"/> instead.</br>
    /// </summary>
    public abstract class GeneralizedPrismApplication : PrismApplication
    {
        protected sealed override Window CreateShell()
        {
            return null;
        }

        /// <summary>
        /// Creates the shell or main element of the application. Use this method instead of <see cref="CreateShell"/>.
        /// </summary>
        /// <returns>The shell of the application.</returns>
        protected abstract FrameworkElement CreateElement();

        /// <summary>
        /// Gets or sets the main element of the application. Use this property instead of <see cref="Application.MainWindow"/>.
        /// </summary>
        public FrameworkElement MainElement { get; protected set; }

        protected override void Initialize()
        {
            base.Initialize();
            var shell = CreateElement();
            if (shell != null)
            {
                var method = typeof(MvvmHelpers).GetMethod("AutowireViewModel", BindingFlags.Static | BindingFlags.NonPublic);
                method.Invoke(null, new object[] { shell });
                RegionManager.SetRegionManager(shell, this.Container.Resolve<IRegionManager>());
                RegionManager.UpdateRegions();
                InitializeShell(shell);
            }
            InitializeModules();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        /// <param name="shell"></param>
        protected virtual void InitializeShell(FrameworkElement shell)
        {
            MainElement = shell;
        }

        /// <summary>
        /// Do not override this method. Use <see cref="InitializeShell"/> instead.
        /// </summary>
        /// <param name="shell"></param>
        protected sealed override void InitializeShell(Window shell)
        {
            ;//Originally MainWindow = shell;
        }

        /// <summary>
        /// Contains actions that should occur last.
        /// </summary>
        protected new virtual void OnInitialized()
        {
            ;//Originally MainWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;//Without this, the application will exit.
            base.OnStartup(e);
        }
    }

    public class GeneralizedPrismApplication<T> : GeneralizedPrismApplication where T : FrameworkElement
    {
        protected override FrameworkElement CreateElement()
        {
            return Container.Resolve<T>();
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
