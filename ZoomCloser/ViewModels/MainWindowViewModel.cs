/*
MIT License
Copyright (c) 2021 34j and contributors
https://opensource.org/licenses/MIT
*/
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ZoomCloser.Services;
using ZoomCloser.Modules;

namespace ZoomCloser.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fody_Bindings
        public string Title { get; set; } = "";
        public string NumberDisplayText { get; set; } = "0";
        public ObservableCollection<string> LogListBoxItemsSource { get; set; } = new ObservableCollection<string>();
        #endregion Fody_Bindings

        public IZoomExitByRatioService zoomExitService;
        public MainWindowViewModel(IZoomExitByRatioService zoomClosingService)
        {
            zoomExitService = zoomClosingService;
            zoomExitService.OnRefreshed += (_, e) => DisplayValues();
            zoomExitService.ReadOnlyZoomHandlingService.OnEntered += (_, e) => Log("ミーティングに参加");
            zoomExitService.ReadOnlyZoomHandlingService.OnExit += (_, e) => Log("ミーティングから退出");
            zoomExitService.ReadOnlyZoomHandlingService.OnParticipantCountAvailable += (_, e) => Log("参加者数の取得を開始");
            zoomExitService.ReadOnlyZoomHandlingService.OnThisForcedExit += (_, e) => Log("このソフトが退出");
            zoomExitService.ReadOnlyZoomHandlingService.OnNotThisForcedExit += (_, e) => Log("ユーザーが手動で退出");
            BindingOperations.EnableCollectionSynchronization(LogListBoxItemsSource, new object());
        }

        #region ListBox_Functions
        private string NowLongTimeString => System.DateTime.Now.ToLongTimeString();
        private void Log(string text)
        {
            LogListBoxItemsSource.Add(NowLongTimeString + " " + text);
        }

        #endregion ListBox_Functions

        private async Task Close()
        {
            await zoomExitService.ExitManually().ConfigureAwait(false);
        }

        private DelegateCommand closeCommand;
        public DelegateCommand CloseCommand =>
            closeCommand ?? (closeCommand = new DelegateCommand(async () => await Close().ConfigureAwait(false)));

        private DelegateCommand<Window> applicationExitCommand;
        public DelegateCommand<Window> CloseWindowCommand =>
            applicationExitCommand ?? (applicationExitCommand = new DelegateCommand<Window>(ExitApplication));

        private void ExitApplication(Window window)
        {
            window?.Close();
            Environment.Exit(0);
        }

        public void DisplayValues()
        {
            var zoomMode = zoomExitService.ReadOnlyZoomHandlingService.ZoomMode;
            var exitService = zoomExitService.JudgingWhetherToExitByRatioService;
            if (zoomMode == ZoomHandlingServiceState.E_NotRunning)
            {
                NumberDisplayText = "Zoomは実行中ではありません。";
            }
            else if (zoomMode == ZoomHandlingServiceState.E_NoMemberList)
            {
                NumberDisplayText = "参加者一覧を出してください。すく閉じて構いません。";
            }
            else if (zoomMode == ZoomHandlingServiceState.E_DifferentShortCuts)
            {
                NumberDisplayText = "Zoomのショートカットが想定（標準）と異なるため、使用を継続するには変更してください：参加者の管理(ALT+T)　または、その他のエラー";
            }
            else if (zoomMode == ZoomHandlingServiceState.E_UnableToParse)
            {
                NumberDisplayText = "バグまたはZoomの仕様変更等の理由により参加者数を取得できません";
            }
            else
            {
                NumberDisplayText = $"参加者数　現在値：{exitService.CurrentCount}人, 過去最大値：{exitService.MaximumCount}人, ";
                if (exitService.IsOverThresholdToActivation)
                {
                    NumberDisplayText += $"参加者が{exitService.MaximumCountToExit}人になったら退出します。";
                }
                else
                {
                    NumberDisplayText += $"参加者数の過去最大値が{exitService.ThresholdToActivation}人を超えるまでは自動で退出しません。";
                }
                Title = $"{exitService.CurrentCount}/{exitService.MaximumCount}";
            }

            if (zoomMode != ZoomHandlingServiceState.OK)
            {
                Title = "";
            }
        }
    }
}
