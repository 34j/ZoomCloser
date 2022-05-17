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
        public bool IsActivated { get; set; } = true;
        public bool IsMuted { get; set; } = false;
        public bool IsRecording { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public ReadOnlyObservableTranslationCollection LogListBoxItemsSource { get; } = new ReadOnlyObservableTranslationCollection();
        #endregion Fody_Bindings

        public IZoomExitByRatioService zoomExitService;
        private readonly IAudioService audioService;
        private readonly IRecordingService recordingService;
        private IJudgingWhetherToExitByRatioService JudgeService => zoomExitService.JudgingWhetherToExitByRatioService;

        public MainWindowViewModel(IZoomExitByRatioService zoomExitService, IAudioService audioService, IRecordingService recordingService)
        {
            this.audioService = audioService;
            IsMuted = audioService.GetMute();
            this.recordingService = recordingService;
            this.zoomExitService = zoomExitService;
            this.zoomExitService.OnRefreshed += (_, e) => DisplayValues();

            IReadOnlyZoomHandlingService2 zs = this.zoomExitService.ReadOnlyZoomHandlingService;
            zs.OnEntered += (_, e) => Log("ParticipatedInMeeting");
            zs.OnExit += (_, e) => { Log("ExitMeeting"); Log("ParticipantCount", JudgeService.CurrentCount, JudgeService.MaximumCount); };
            zs.OnExit += (_, e) => recordingService.StopRecording();
            zs.OnParticipantCountAvailable += (_, e) => Log("StartedCapturingTHeNumberOfParticipants");
            zs.OnThisForcedExit += (_, e) => Log("ThisSoftwareForcedToExitMeeting");
            zs.OnNotThisForcedExit += (_, e) => Log("UserForcedToExitMeeting");

            IsActivated = zoomExitService.IsActivated;

            //below is for the logging list.
            BindingOperations.EnableCollectionSynchronization(LogListBoxItemsSource, new object());
        }
        private static ITranslation GetITranslation(string key)
        {
            return Translation.GetOrCreate(Properties.Resources.ResourceManager, key);
        }

        private static string GetTranslationStr(string key)
        {
            string result = GetITranslation(key).Translated;
            if (result == null)
            {
                throw new Exception("key not registered");
            }
            return result;
        }
        private static string GetTranslationStr(string key, params object[] args)
        {
            Validate.Format(GetTranslationStr(key), args);
            return string.Format(GetTranslationStr(key), args);
        }

        #region ListBox_Functions
        private static string NowLongTimeString => DateTime.Now.ToLongTimeString();
        private void Log(string key, params object[] args)
        {
            string now = NowLongTimeString;
            LogListBoxItemsSource.Add(GetITranslation(key), s => now + " " + s, args);
        }

        #endregion ListBox_Functions



        #region commands
        private DelegateCommand activateCommand;
        public DelegateCommand ActivateCommand =>
activateCommand ??= new DelegateCommand(ExecuteActivateCommand);

        void ExecuteActivateCommand()
        {
            zoomExitService.IsActivated = IsActivated;
        }
        private DelegateCommand exitMeetingCommand;
        public DelegateCommand ExitMeetingCommand =>
exitMeetingCommand ??= new DelegateCommand(ExecuteExitMeetingCommand);

        private async void ExecuteExitMeetingCommand()
        {
            await zoomExitService.ExitManually().ConfigureAwait(false);
        }

        private DelegateCommand<Window> applicationExitCommand;
        public DelegateCommand<Window> ApplicationExitCommand =>
applicationExitCommand ??= new DelegateCommand<Window>(ExecuteApplicationExitCommand);

        private void ExecuteApplicationExitCommand(Window window)
        {
            _ = Task.Run(() =>
              {
                  window?.Close();
                  Environment.Exit(0);
              });
        }

        private DelegateCommand muteCommand;
        public DelegateCommand MuteCommand =>
muteCommand ??= new DelegateCommand(ExecuteMuteCommand);

        private void ExecuteMuteCommand()
        {
            audioService.SetMute(IsMuted);
        }

        private DelegateCommand recordCommand;

        public DelegateCommand RecordCommand =>
recordCommand ??= new DelegateCommand(ExecuteRecordCommand);

        void ExecuteRecordCommand()
        {
            if (IsRecording)
            {
                recordingService.StartRecording();
            }
            else
            {
                recordingService.StopRecording();
            }
            IsRecording = recordingService.IsRecording;
        }

        private DelegateCommand changeVisiblityCommand;
        public DelegateCommand ChangeVisiblityCommand =>
changeVisiblityCommand ??= new DelegateCommand(ExecuteChangeVisiblityCommand);

        void ExecuteChangeVisiblityCommand()
        {
            IsVisible = !IsVisible;
        }

        private DelegateCommand openFolderCommand;
        public DelegateCommand OpenFolderCommand =>
openFolderCommand ??= new DelegateCommand(ExecuteOpenFolderCommand);

        void ExecuteOpenFolderCommand()
        {
            Process.Start("explorer.exe", recordingService.FolderPath);
        }

        private DelegateCommand openSettingsCommand;
        public DelegateCommand OpenSettingsCommand =>
openSettingsCommand ??= new DelegateCommand(ExecuteOpenSettingsCommand);

        private void ExecuteOpenSettingsCommand()
        {
            _ = Process.Start("explorer.exe", "/select, \"" + BasicSettings.FilePath + "\"");
        }
        #endregion commands




        public void DisplayValues()
        {
            ZoomErrorState zoomMode = zoomExitService.ReadOnlyZoomHandlingService.ZoomState;
            IJudgingWhetherToExitByRatioService exitService = zoomExitService.JudgingWhetherToExitByRatioService;
            if (zoomMode == ZoomErrorState.NotRunning)
            {
                NumberDisplayText = GetTranslationStr("ZoomNotRunning");
            }
            else if (zoomMode == ZoomErrorState.NotExpectedBehaviour)
            {
                NumberDisplayText = GetTranslationStr("Bug");
            }
            else if (zoomMode == ZoomErrorState.NoError)
            {
                NumberDisplayText = GetTranslationStr("ParticipantCount", exitService.CurrentCount, exitService.MaximumCount) + "\r\n";
                if (exitService.IsOverThresholdToActivation)
                {
                    NumberDisplayText += GetTranslationStr("NormalExitCondition", exitService.MaximumCountToExit);
                }
                else
                {
                    NumberDisplayText += GetTranslationStr("UnderOrEqualsToThresholdExitCondition", exitService.ThresholdToActivation);
                }
                Title = $"{exitService.CurrentCount}/{exitService.MaximumCount}";
            }
            else if (zoomMode == ZoomErrorState.Minimized)
            {
                NumberDisplayText = GetTranslationStr("Minimized");
            }
            else
            {
                NumberDisplayText = zoomMode.ToString();
            }
        }
    }
}
