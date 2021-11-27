using Bindables;
using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZoomCloser.Views
{
    /// <summary>
    /// TwoIconButton.xaml の相互作用ロジック
    /// </summary>
    [DependencyProperty]
    public partial class TwoIconButton : UserControl
    {
        public bool Value { get; set; }
        public ICommand Command { get; set; }
        public PackIconBoxIconsKind TrueKind { get; set; }
        public PackIconBoxIconsKind FalseKind { get; set; }
        public TwoIconButton()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}