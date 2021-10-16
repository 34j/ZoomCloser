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

namespace ZoomCloser.ViewModels
{
    //[AddINotifyPropertyChangedInterface]
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fody_Bindings
        public string Title { get; set; } = "";
        public string NumberDisplayText { get; set; } = "0";

        public bool IsMuted { get; private set; } = false;
        public ReadOnlyObservableTranslationCollection LogListBoxItemsSource { get; } = new ReadOnlyObservableTranslationCollection();
        #endregion Fody_Bindings

        public IZoomExitByRatioService zoomExitService;
        private readonly IAudioService audioService;

        public MainWindowViewModel(IZoomExitByRatioService zoomClosingService, IAudioService audioService)
        {
            this.audioService = audioService;
            zoomExitService = zoomClosingService;
            zoomExitService.OnRefreshed += (_, e) => DisplayValues();
            zoomExitService.ReadOnlyZoomHandlingService.OnEntered += (_, e) => Log("ParticipatedInMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnExit += (_, e) => Log("ExitMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnParticipantCountAvailable += (_, e) => Log("StartedCapturingTHeNumberOfParticipants");
            zoomExitService.ReadOnlyZoomHandlingService.OnThisForcedExit += (_, e) => Log("ThisSoftwareForcedToExitMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnNotThisForcedExit += (_, e) => Log("UserForcedToExitMeeting");
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
        private void Log(string key)
        {
            LogListBoxItemsSource.Translations.Add((Trr(key),s => NowLongTimeString + " " + s));
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

        private DelegateCommand muteCommand;
        public DelegateCommand MuteCommand =>
            muteCommand ?? (muteCommand = new DelegateCommand(Mute));

        private void Mute()
        {
            IsMuted = !IsMuted;
            audioService.SetMute(IsMuted);
        }

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
                NumberDisplayText = Tr("ZoomNotRunning");
            }
            else if (zoomMode == ZoomHandlingServiceState.E_NoMemberList)
            {
                NumberDisplayText = Tr("OpenParticipantsWindow");
            }
            else if (zoomMode == ZoomHandlingServiceState.E_DifferentShortCuts)
            {
                NumberDisplayText = Tr("DifferentShortcuts");
            }
            else if (zoomMode == ZoomHandlingServiceState.E_UnableToParse)
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

            if (zoomMode != ZoomHandlingServiceState.OK)
            {
                Title = "";
            }
        }
    }
}
