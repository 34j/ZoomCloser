using Bindables;
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
    /// IconToggleSwitch.xaml の相互作用ロジック
    /// </summary>
    [DependencyProperty]
    public partial class IconToggleSwitch : UserControl
    {
        public bool Value { get; set; }
        public ICommand Command { get; set; }
        public PackIconBoxIconsKind OnKind { get; set; }
        public PackIconBoxIconsKind OffKind { get; set; }
        public string OnText { get; set; } = "On";
        public string OffText { get; set; } = "Off";
        public string Header { get; set; } = "";
        public IconToggleSwitch()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
