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
using Gu.Localization;


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
            zoomExitService.ReadOnlyZoomHandlingService.OnEntered += (_, e) => Log("ParticipatedInMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnExit += (_, e) => Log("ExitMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnParticipantCountAvailable += (_, e) => Log("StartedCapturingTHeNumberOfParticipants");
            zoomExitService.ReadOnlyZoomHandlingService.OnThisForcedExit += (_, e) => Log("ThisSoftwareForcedToExitMeeting");
            zoomExitService.ReadOnlyZoomHandlingService.OnNotThisForcedExit += (_, e) => Log("UserForcedToExitMeeting");
            BindingOperations.EnableCollectionSynchronization(LogListBoxItemsSource, new object());
        }

        static string Tr(string key)
        {
            var result = Translation.GetOrCreate(ZoomCloser.Properties.Resources.ResourceManager, key).Translated;
            if (result == null)
            {
                throw new Exception("key not registered");
            }
            return result;
        }
        static string Tr(string key, params object[] args)
        {
            Validate.Format(Tr(key), args);
            return string.Format(Tr(key), args);
        }

        #region ListBox_Functions
        private string NowLongTimeString => System.DateTime.Now.ToLongTimeString();
        private void Log(string key)
        {
            LogListBoxItemsSource.Add(NowLongTimeString + " " + Tr(key));
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
                NumberDisplayText = Tr("Bug") + "\r\n";
            }
            else
            {
                NumberDisplayText = Tr("ParticipantCount", exitService.CurrentCount, exitService.MaximumCount);
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
