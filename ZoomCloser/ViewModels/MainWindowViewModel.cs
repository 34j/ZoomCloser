/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ZoomCloser.Models;
using ZoomCloser.Modules;

namespace ZoomCloser.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Title { get; set; } = "";
        public string NumberDisplayText { get; set; } = "0";
        public Models.Model model;
        public ObservableCollection<string> LogListBoxItemsSource { get; set; } = new ObservableCollection<string>();
        public MainWindowViewModel()
        {
            model = new Models.Model();
            model.OnTimed += DisplayValues;
            model.OnEntered += Model_OnEntered;
            model.OnExit += Model_OnExit;
            model.OnParticipantsCountAvailbale += Model_OnParticipantsCountAvailbale;
            model.OnForcedExit += Model_OnForcedExit;
            BindingOperations.EnableCollectionSynchronization(LogListBoxItemsSource, new object());
        }

        private void Model_OnForcedExit(object sender, System.EventArgs e)
        {
            Log("forced exit");
        }

        private void Model_OnParticipantsCountAvailbale(object sender, System.EventArgs e)
        {
            Log("participant count available");
        }

        private void Model_OnExit(object sender, System.EventArgs e)
        {
            Log("exit");
        }

        private void Model_OnEntered(object sender, System.EventArgs e)
        {
            Log("entered");
        }

        private string time => System.DateTime.Now.ToLongTimeString();
        public void Log(string text) => LogListBoxItemsSource.Add(time + " " + text);

        private async Task Close()
        {
            await model.CloseZoom();
        }

        private DelegateCommand closeCommand;
        public DelegateCommand CloseCommand =>
            closeCommand ?? (closeCommand = new DelegateCommand(async() => { await Close(); }));

        public void DisplayValues()
        {
            var zoomMode = model.ZoomMode;
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
            else if (zoomMode == ZoomHandler.ZoomMode.E_UnableToParse)
            {
                NumberDisplayText = "バグまたはZoomの仕様変更等の理由により参加者数を取得できません";
            }
            else
            {
                NumberDisplayText = $"参加者数：{model.ParticipantCount}人, 最大参加者数：{model.MaxNumber}人, ";
                if (model.IsOverThreshold)
                {
                    NumberDisplayText += $"参加者が{model.ExitNumber}人になったら退出";
                }
                else
                {
                    NumberDisplayText += $"最大参加者数が{model.participantCountToThresholdValue}人になるまでは何もしません";
                }
                Title = $"{model.ParticipantCount}/{model.MaxNumber}";
            }

            if (zoomMode != ZoomHandler.ZoomMode.OK)
            {
                Title = "";
            }
        }
    }
}
