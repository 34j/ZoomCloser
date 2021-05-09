using Prism.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ZoomCloserJp.Models;

namespace ZoomCloserJp.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; } = "";
        public string NumberDisplayText { get; set; } = "0";
        public Models.Model model;

        public MainWindowViewModel()
        {
            model = new Models.Model();
            model.OnTimed += DisplayValues;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model.zoomHandler.CloseZoom();
        }

        private void Close()
        {
            model.zoomHandler.CloseZoom();
        }



        private ICommand closeCommand;

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = new DelegateCommand(Close);
                }

                return closeCommand;
            }
        }

        public void DisplayValues()
        {
            var zoomMode = model.zoomHandler.ZoomMode_;
            if (zoomMode == ZoomHandler.ZoomMode.E_NotRunning)
            {
                NumberDisplayText = "Zoomが実行中でないと考えられます";
            }
            else if (zoomMode == ZoomHandler.ZoomMode.E_NoMemberList)
            {
                NumberDisplayText = "参加者一覧を出してください。すく閉じて構いません。";
            }
            else if (zoomMode == ZoomHandler.ZoomMode.E_DifferentShortCuts)
            {
                NumberDisplayText = "Zoomのショートカットが想定（標準）と異なるため、使用を継続するには変更してください：参加者の管理(ALT+T)　または、その他のエラー";
            }
            else
            {
                int num = model.zoomHandler.ParticipantNumbers;
                NumberDisplayText = $"参加者：{num}人, 最大参加者数：{model.MaxNumber}人, ";
                if (model.CanExit)
                {
                    NumberDisplayText += $"参加者が{model.ExitNumber}人になったら退出";
                }
                else
                {
                    NumberDisplayText += $"最大参加者数が{model.leastMemberCount}人になるまでは何もしません";
                }
                Title = $"{num}/{model.MaxNumber}";
            }

            if(zoomMode != ZoomHandler.ZoomMode.OK)
            {
                Title = "";
            }

        }
    }
}
