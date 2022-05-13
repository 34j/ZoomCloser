using Bindables;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
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
        public bool IsOn { get; set; }
        public string Header { get; set; }
        public ICommand Command { get; set; }

        
        public IconToggleSwitch()
        {

            InitializeComponent();
            if (this.HasContent)
            {
                (this.Content as FrameworkElement).DataContext = this;
            }
            this.DataContext = this;

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
