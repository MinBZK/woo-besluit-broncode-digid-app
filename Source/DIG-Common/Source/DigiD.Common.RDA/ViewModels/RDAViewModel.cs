// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
//
﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Interfaces;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Interfaces;
using DigiD.Common.RDA.Enums;
using DigiD.Common.RDA.Services;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.RDA.ViewModels
{
    public sealed class RdaViewModel : BaseViewModel
    {
        private readonly Func<bool, Task> _completeAction;
        private RdaService _serviceRda;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _inSettings;
        private readonly bool _fromIdChecker;
        private readonly Func<Task> _retryAction;
        private Helpers.Timer _timer;
        private bool _fromBackground;

        public double ProgressValue { get; set; }
        public bool ProgressVisible { get; set; }
        public string AnimationSource { get; set; }
        public bool AnimationVisible { get; set; }
        public string ImageSource { get; set; }
        public bool NFCDisabled { get; set; }
        public bool RetryButtonVisible { get; set; }
        public bool HelpButtonVisible { get; set; }

#if A11YTEST
        public RdaViewModel() : this(new RdaSessionResponse(), async b => { await Task.FromResult(false);}, "a11yTst-ID-check", true, async () => await Task.FromResult(false)) { }
#endif

        /// <summary>
        /// This viewmodel will handle the RDA process
        /// </summary>
        /// <param name="completeAction">This action will be fired when the process is completed (success and failed)</param>
        /// <param name="pageId">PageID</param>
        /// <param name="fromIdChecker"></param>
        /// <param name="retryAction">This action is called for the new forced RDA check while activating the app. It will return true when the user is given the option to retry, else it will activate the app via the alternative method (sms/letter)</param>
        /// <param name="session">RDA Session, if started</param>
        public RdaViewModel(RdaSessionResponse session, Func<bool, Task> completeAction, string pageId, bool fromIdChecker = false, Func<Task> retryAction = null)
        {
            if (retryAction == null && HttpSession.ActivationSessionData != null)
                throw new ArgumentException("RetryAction is required", nameof(retryAction));

            if (session == null)
                throw new ArgumentException("RDA Session is required", nameof(session));

            HttpSession.RDASessionData = new RdaSession(session);
            _cancellationTokenSource = new CancellationTokenSource();

            PageId = pageId;

            _completeAction = completeAction ?? throw new ArgumentException("CompleteAction is required", nameof(completeAction));
            _fromIdChecker = fromIdChecker;
            _retryAction = retryAction;

            AnimationSource = AppResources.NFC_RDA_Animation_RemoveFromCover;
            NavCloseCommand = CancelCommand;

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                RetryButtonVisible = true;

            HelpButtonVisible = retryAction != null && Device.RuntimePlatform == Device.Android;
        }

        public AsyncCommand OpenHelpCommand => new AsyncCommand(async () =>
        {
            await NavigationService.PushModalAsync(new VideoPlayerViewModel(VideoFile.Rda), true, false);
        });

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            if (Device.RuntimePlatform == Device.Android && _serviceRda != null && _serviceRda.IsReading)
            {
                _fromBackground = true;
                DependencyService.Get<INfcService>().StopScanningAsync();
            }
        }

        public override async void OnAppearing()
        {
            if (Device.RuntimePlatform == Device.Android && _fromBackground)
            {
                _fromBackground = false;
                await _serviceRda.StartScanning(true);
                return;
            }

            if (_inSettings)
                return;

            NFCDisabled = !DependencyService.Get<INfcService>().IsNFCEnabled;

            var pageId = PageId;

            if (NFCDisabled)
            {
                DialogService.HideProgressDialog();
                _inSettings = true;
                HeaderText = AppResources.AP037_Header;
                FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.AP037_Message, DocumentType.Generic.Translate().ToLowerInvariant());
                ButtonText = AppResources.NFCDisabledButton;
                ImageSource = AppSession.Process == Process.IDChecker ? "resource://IDChecker.Resources.idchecker_afbeelding_idchecker_aanzetten_nfc.svg?assembly=IDChecker" : "resource://DigiD.Common.Resources.afbeelding_aanzetten_nfc.svg?assembly=DigiD.Common";
                AnimationVisible = false;

                PageId = AppSession.Process == Process.IDChecker ? "ID06" : "AP037";
                TrackView();

                await Task.Run(async () =>
                {
                    while (!DependencyService.Get<INfcService>().IsNFCEnabled)
                    {
                        await Task.Delay(100);
                    }

                    PageId = pageId;
                    NFCDisabled = false;
                    _inSettings = false;
                });

                return;
            }
#if !A11YTEST
            await StartSession();
#endif 

            base.OnAppearing();
        }

        private async Task HandleAction(MessagePageType messagePageType)
        {
            if (_retryAction != null)
                await _retryAction.Invoke();
            else
                await ShowMessagePage(messagePageType);
        }

        private async Task StartSession()
        {
            if (HttpSession.RDASessionData == null)
            {
                DialogService.ShowProgressDialog();
                var session = await GetRdaSessionUrl();
                DialogService.HideProgressDialog();

                switch (session.ApiResult)
                {
                    case ApiResult.Ok:
                        HttpSession.RDASessionData = new RdaSession(session);
                        break;
                    case ApiResult.Disabled:
                        await HandleAction(MessagePageType.RDADisabled);
                        return;
                    case ApiResult.Nok:
                        await HandleAction(MessagePageType.UpgradeAccountWithNFCFailed);
                        return;
                    default:
                        await HandleAction(MessagePageType.UpgradeAccountWithNFCError);
                        return;
                }
            }

            DialogService.HideProgressDialog();

            HeaderText = string.Format(CultureInfo.InvariantCulture, AppResources.RDAScannerTitle, DocumentType.Generic.Translate().ToLowerInvariant());

            if (AppSession.Process == Process.UpgradeAndAuthenticate)
                HeaderText = AppResources.LoginAndUpgradeHeader;

            if (HttpSession.RDASessionData != null)
                Initialize();
            else
                await ShowMessagePage(MessagePageType.UpgradeAccountWithNFCError);
        }

        private async Task ShowMessagePage(MessagePageType type)
        {
            if (AppSession.Process == Process.UpgradeAndAuthenticate)
                await _completeAction.Invoke(false);
            else
                await NavigationService.ShowMessagePage(type);
        }

        public new ICommand ButtonCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;
                    DependencyService.Get<INfcService>().OpenNFCSettings();
                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        public AsyncCommand RetryScanCommand => new AsyncCommand(async () =>
        {
            StartTimer();

            await _serviceRda.StartScanning(false);
            if (DeviceInfo.Platform != DevicePlatform.iOS)
                RetryButtonVisible = false;
        });


        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    var result = await DependencyService.Get<IAlertService>().DisplayAlert(
                        AppResources.UpgradeRDACancelHeader
                        , AppResources.UpgradeRDACancelMessage
                        , AppResources.Yes
                        , AppResources.No);

                    if (result)
                    {
                        if (!_cancellationTokenSource.IsCancellationRequested)
                            _cancellationTokenSource.Cancel(false);

                        await DependencyService.Get<INfcService>().StopScanningAsync();

                        if (AppSession.Process == Process.UpgradeAndAuthenticate)
                        {
                            await _completeAction.Invoke(false);
                            return;
                        }

                        _timer?.Stop();
                        await DependencyService.Get<IRdaServices>().Cancel();
                        await ShowMessagePage(MessagePageType.UpgradeAccountWithNFCCancelled);
                    }
                });
            }
        }

        public static async Task<RdaSessionResponse> GetRdaSessionUrl()
        {
            var status = ApiResult.Pending;

            var response = await DependencyService.Get<IRdaServices>().Documents();

            if (response.ApiResult != ApiResult.Ok)
                return new RdaSessionResponse { ApiResult = response.ApiResult };

            while (status == ApiResult.Pending)
            {
                var result = await DependencyService.Get<IRdaServices>().Poll();

                status = result.ApiResult;

                //if success, save the url
                if (result.ApiResult == ApiResult.Ok)
                {
                    return result;
                }

                //if pending, wait 1000ms, and retry
                if (status == ApiResult.Pending)
                    await Task.Delay(1000);
            }

            return new RdaSessionResponse { ApiResult = status };
        }

        internal void Initialize(bool retry = false)
        {
            ProgressVisible = false;
            ProgressValue = 0;

            AnimationVisible = true;

            if (Device.RuntimePlatform == Device.iOS)
                FooterText = string.Format(!retry ? AppResources.RDAStartScanning_iOS : AppResources.RDAReStartScanning_iOS, AppResources.eID.ToLowerInvariant(), DependencyService.Get<IDevice>().DeviceTypeName);
            else
                FooterText = string.Format(!retry ? AppResources.RDAStartScanning : AppResources.RDAReStartScanning, AppResources.eID.ToLowerInvariant(), DependencyService.Get<IDevice>().DeviceTypeName);

            if (_serviceRda == null)
                _serviceRda = new RdaService(StatusChanged, p =>
                {
                    ProgressValue = p;
                    DependencyService.Get<INfcService>().UpdateStatus(p);
                });

            if (!retry && HttpSession.ActivationSessionData != null && Device.RuntimePlatform == Device.Android)
            {
                StartTimer();
            }
            else if (retry && HttpSession.ActivationSessionData != null && Device.RuntimePlatform == Device.iOS)
            {
                DependencyService.Get<INfcService>().StopScanningAsync();
            }

            if (Device.RuntimePlatform != Device.iOS)
                Task.Run(async () => { await _serviceRda.StartScanning(retry); });
        }

        private bool fromTimer;

        private void StartTimer()
        {
            fromTimer = false;

            var seconds = DependencyService.Get<IDemoSettings>().IsDemo && !DependencyService.Get<IA11YService>().IsInVoiceOverMode() ? 10 : 30;
            _timer = new Helpers.Timer(TimeSpan.FromSeconds(seconds), async () =>
            {
                fromTimer = true;
                await DependencyService.Get<INfcService>().StopScanningAsync();
                _retryAction?.Invoke();
            }, false);
            _timer.Start();
        }

        public AsyncCommand CancelScanCommand => new AsyncCommand(async () =>
        {
            if (fromTimer)
                return;

            await DependencyService.Get<INfcService>().StopScanningAsync();
            _retryAction?.Invoke();
            _timer?.Stop();
        });

        private async Task StatusChanged(RdaStatus status)
        {
            _timer?.Stop();

            switch (status)
            {
                case RdaStatus.ReadingStarted:
                    {
                        ProgressVisible = true;
                        await DependencyService.Get<IA11YService>().Speak(AppResources.AlternateTextProgressBar);

                        FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.ReadDocument, AppResources.eID.ToLowerInvariant());
                        _cancellationTokenSource.Cancel(false);
                        AnimationVisible = false;
                        ImageSource = AppSession.Process == Process.IDChecker ? "resource://IDChecker.Resources.idchecker_afbeelding_idchecker_read_document.svg?assembly=IDChecker" : "resource://DigiD.Common.Resources.afbeelding_read_document.svg?assembly=DigiD.Common";

                        break;
                    }
                case RdaStatus.ReadingCompleted:
                    {
                        ProgressVisible = false;
                        DialogService.ShowProgressDialog();
                        await Device.InvokeOnMainThreadAsync(async () => await GetStatus());
                        DialogService.HideProgressDialog();
                        break;
                    }
                case RdaStatus.ReadingCancelled:
                    {
                        await CancelScanCommand.ExecuteAsync();
                        break;
                    }
                case RdaStatus.ReadingFailed:
                case RdaStatus.AuthenticationFailed:
                case RdaStatus.UnknownCard:
                    {
                        if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                            await _retryAction.Invoke();
                        else
                            await HandleFailed(status);
                        break;
                    }
                case RdaStatus.CardLost:
                    {
                        if (Device.RuntimePlatform == Device.iOS)
                            await DependencyService.Get<INfcService>().StopScanningAsync(AppResources.ConnectionLostMessage);

                        Initialize(true);
                        break;
                    }
                default:
                    if (AppSession.Process == Process.UpgradeAndAuthenticate)
                        await HandleFailed(RdaStatus.AuthenticationFailed);
                    else
                        await ShowMessagePage(MessagePageType.UpgradeAccountWithNFCFailed);
                    break;
            }
        }

        private async Task HandleFailed(RdaStatus status)
        {
            await DependencyService.Get<INfcService>().StopScanningAsync();

            if (status == RdaStatus.AuthenticationFailed)
            {
                if (AppSession.Process == Process.AppActivationViaMrz || _fromIdChecker)
                {
                    await _completeAction.Invoke(false);
                    return;
                }

                await NavigationService.PushAsync(new AP103ViewModel(_completeAction, _retryAction));
                return;
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var prompt = await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.ReadingFailedPromptTitle, AppResources.ReadingFailedPromptMessage, AppResources.Yes, AppResources.No);

                if (prompt)
                    await StartSession();
                else
                {
                    HttpSession.RDASessionData = null;
                    if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.Password && _retryAction != null)
                    {
                        await _retryAction.Invoke();
                        return;
                    }
                    await _completeAction.Invoke(false);
                }
            });
        }

        private async Task GetStatus()
        {
            var status = await DependencyService.Get<IRdaServices>().Verified();

            while (status == ApiResult.Pending)
            {
                await Task.Delay(100);
                status = await DependencyService.Get<IRdaServices>().Verified();
            }

            if (status == ApiResult.Ok && !_fromIdChecker)
            {
                DependencyService.Get<IMobileSettings>().LoginLevel = LoginLevel.Substantieel;

                if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                    DependencyService.Get<IMobileSettings>().ActivationStatus = ActivationStatus.Pending;
                else
                    DependencyService.Get<IMobileSettings>().ActivationStatus = ActivationStatus.Activated;

                AppSession.IsAppActivated = DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated;

                DependencyService.Get<IMobileSettings>().Save();
                DependencyService.Get<ILocalNotifications>()?.Cancel((int)NotificationType.IDcheck);
            }

            await _completeAction.Invoke(status == ApiResult.Ok);
        }

        public Command EnableNFCCommand
        {
            get
            {
                return new Command(() =>
                {
                    CanExecute = false;
                    DependencyService.Get<INfcService>().OpenNFCSettings();
                    CanExecute = true;
                }, () => CanExecute);
            }
        }
    }
}
