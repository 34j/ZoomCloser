/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using MetroRadiance.UI.Controls;
using ZoomCloser.ViewModels;

namespace ZoomCloser.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindowViewModel MainWindowViewModel { get; init; }
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            this.MainWindowViewModel = mainWindowViewModel;
            InitializeComponent();
        }
    }
}
