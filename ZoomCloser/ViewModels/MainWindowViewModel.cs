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
using ZoomCloser.Utils;
using ZoomCloser.Services;
using ZoomCloser.Modules;
using Gu.Localization;
using System.Linq;
using ZoomCloser.Services.Audio;
using ZoomCloser.Services.Recording;
using System.Diagnostics;
using ZoomCloser.Services.ZoomHandling;

namespace ZoomCloser.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fody_Bindings
        public string Title { get; set; } = "";
        public string NumberDisplayText { get; set; } = "0";

        public bool IsMuted { get; private set; } = false;

        public bool IsRecording { get; private set; } = false;

        public bool IsVisible { get; private set; } = true;
        public ReadOnlyObservableTranslationCollection LogListBoxItemsSource { get; } = new ReadOnlyObservableTranslationCollection();
        #endregion Fody_Bindings

        public IZoomExitByRatioService zoomExitService;
        private readonly IAudioService audioService;
        private readonly IRecordingService recordingService;
        private IJudgingWhetherToExitByRatioService judgeService => zoomExitService.JudgingWhetherToExitByRatioService;

        public MainWindowViewModel(IZoomExitByRatioService zoomExitService, IAudioService audioService, IRecordingService recordingService)
        {
            this.audioService = audioService;
            IsMuted = audioService.GetMute();
            this.recordingService = recordingService;
            this.zoomExitService = zoomExitService;
            this.zoomExitService.OnRefreshed += (_, e) => DisplayValues();
            this.zoomExitService.ReadOnlyZoomHandlingService.OnEntered += (_, e) => Log("ParticipatedInMeeting");
            this.zoomExitService.ReadOnlyZoomHandlingService.OnExit += (_, e) => { Log("ExitMeeting"); /*Log("ParticipantCount", judgeService.CurrentCount, judgeService.MaximumCount);*/ };
            this.zoomExitService.ReadOnlyZoomHandlingService.OnExit += (_, e) => recordingService.StopRecording();
            this.zoomExitService.ReadOnlyZoomHandlingService.OnParticipantCountAvailable += (_, e) => Log("StartedCapturingTHeNumberOfParticipants");
            this.zoomExitService.ReadOnlyZoomHandlingService.OnThisForcedExit += (_, e) => Log("ThisSoftwareForcedToExitMeeting");
            //this.zoomExitService.ReadOnlyZoomHandlingService.OnNotThisForcedExit += (_, e) => Log("UserForcedToExitMeeting");
            BindingOperations.EnableCollectionSynchronization(LogListBoxItemsSource, new object());
        }
        private static ITranslation Trr(string key) => Translation.GetOrCreate(ZoomCloser.Properties.Resources.ResourceManager, key);
        private static string Tr(string key)
        {
            var result = Trr(key).Translated;
            if (result == null)
            {
                throw new Exception("key not registered");
            }
            return result;
        }
        private static string Tr(string key, params object[] args)
        {
            Validate.Format(Tr(key), args);
            return string.Format(Tr(key), args);
        }

        #region ListBox_Functions
        private string NowLongTimeString => System.DateTime.Now.ToLongTimeString();
        private void Log(string key, params object[] args)
        {
            string now = NowLongTimeString;
            LogListBoxItemsSource.Add(Trr(key), s => now + " " + s, args);
        }

        #endregion ListBox_Functions



        #region commands

        private DelegateCommand exitMeetingCommand;
        public DelegateCommand ExitMeetingCommand =>
            exitMeetingCommand ?? (exitMeetingCommand = new DelegateCommand(ExecuteExitMeetingCommand));

        private async void ExecuteExitMeetingCommand()
        {
            await zoomExitService.ExitManually().ConfigureAwait(false);
        }

        private DelegateCommand<Window> applicationExitCommand;
        public DelegateCommand<Window> ApplicationExitCommand =>
            applicationExitCommand ?? (applicationExitCommand = new DelegateCommand<Window>(ExecuteApplicationExitCommand));

        private void ExecuteApplicationExitCommand(Window window)
        {
            Task.Run(() =>
            {
                window?.Close();
                Environment.Exit(0);
            });
        }

        private DelegateCommand muteCommand;
        public DelegateCommand MuteCommand =>
            muteCommand ?? (muteCommand = new DelegateCommand(ExecuteMuteCommand));

        private DelegateCommand recordCommand;
        private void ExecuteMuteCommand()
        {
            IsMuted = !IsMuted;
            audioService.SetMute(IsMuted);
        }
        public DelegateCommand RecordCommand =>
   recordCommand ?? (recordCommand = new DelegateCommand(ExecuteRecordCommand));

        void ExecuteRecordCommand()
        {
            IsRecording = !IsRecording;
            if (IsRecording)
            {
                recordingService.StartRecording();
            }
            else
            {
                recordingService.StopRecording();
            }
        }

        private DelegateCommand changeVisiblityCommand;
        public DelegateCommand ChangeVisiblityCommand =>
            changeVisiblityCommand ?? (changeVisiblityCommand = new DelegateCommand(ExecuteChangeVisiblityCommand));

        void ExecuteChangeVisiblityCommand()
        {
            IsVisible = !IsVisible;
        }

        private DelegateCommand openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
            openFolderCommand ?? (openFolderCommand = new DelegateCommand(ExecuteOpenFolderCommand));

        void ExecuteOpenFolderCommand()
        {
            Process.Start("explorer.exe", recordingService.FolderPath);
        }

        private DelegateCommand openSettingsCommand;
        public DelegateCommand OpenSettingsCommand =>
            openSettingsCommand ?? (openSettingsCommand = new DelegateCommand(ExecuteOpenSettingsCommand));

        void ExecuteOpenSettingsCommand()
        {
            Process.Start("explorer.exe", "/select, \"" + SettingsService.FilePath + "\"");
        }
        #endregion commands




        public void DisplayValues()
        {
            var zoomMode = zoomExitService.ReadOnlyZoomHandlingService.ZoomState;
            var exitService = zoomExitService.JudgingWhetherToExitByRatioService;
            if (zoomMode == ZoomState.NotRunning)
            {
                NumberDisplayText = Tr("ZoomNotRunning");
            }

            else if (zoomMode == ZoomState.NotExpectedBehaviour)
            {
                NumberDisplayText = Tr("Bug");
            }
            else
            {
                NumberDisplayText = Tr("ParticipantCount", exitService.CurrentCount, exitService.MaximumCount) + "\r\n";
                if (exitService.IsOverThresholdToActivation)
                {
                    NumberDisplayText += Tr("NormalExitCondition", exitService.MaximumCountToExit);
                }
                else
                {
                    NumberDisplayText += Tr("UnderOrEqualsToThresholdExitCondition", exitService.ThresholdToActivation);
                }
                Title = $"{exitService.CurrentCount}/{exitService.MaximumCount}";
            }

        }
    }
}
