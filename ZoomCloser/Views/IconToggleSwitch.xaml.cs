using Bindables;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoomCloser.Views
{
    /// <summary>
    /// IconToggleSwitch.xaml の相互作用ロジック DependencyProperty not working with new keyword.
    /// </summary>
    [DependencyProperty]
    public partial class IconToggleSwitch : UserControl
    {
        public PackIconBoxIconsKind OnKind { get; set; }
        public PackIconBoxIconsKind OffKind { get; set; }
        public string OnText { get; set; } = "On";
        public string OffText { get; set; } = "Off";
        [DependencyProperty(OnPropertyChanged = nameof(OnIsOnChanged), Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public bool IsOn { get; set; }
        public string Header { get; set; }
        [DependencyProperty(OnPropertyChanged =nameof(OnCommandChanged))]
        public ICommand Command { get; set; }

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /*
            var control = d as IconToggleSwitch;
            var command = control.Command;
            Debug.WriteLine(control.Header + ": " + control.IsOn + command);
            if (command != null && command.CanExecute(null))
            {
                command?.Execute(null);

            }*/
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("command changed!");
        }            

        /*
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
            typeof(ICommand),
            typeof(IconToggleSwitch), null);
        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as IconToggleSwitch).ToggleSwitch.Command = (ICommand)e.NewValue;
        }
        mainwindowviewmodel->this binding notworking
        */

        public ToggleSwitch ToggleSwitch { get; init; }


        public IconToggleSwitch()
        {

            InitializeComponent();
            if (this.HasContent)
            {
                ToggleSwitch = (ToggleSwitch)this.Content;
                //(this.Content as FrameworkElement).DataContext = this;this is fucking
            }
            else
            {
                throw new NotImplementedException();
            }
            //this.DataContext = this;



            //IsOnProperty.
            /*var on = this.OnContent as FrameworkElement;
            var off = this.OffContent as FrameworkElement;
            foreach (var content in new FrameworkElement[] {on, off })
            {
                if(content != null)
                {
                    content.DataContext = this;
                }
            }*/
        }

    }
}
