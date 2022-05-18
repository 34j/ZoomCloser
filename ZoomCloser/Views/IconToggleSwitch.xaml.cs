using Bindables;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        [DependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
        public bool IsOn { get; set; }
        public string Header { get; set; }
        public ICommand Command { get; set; }

        public IconToggleSwitch()
        {
            InitializeComponent();
        }

    }
}
