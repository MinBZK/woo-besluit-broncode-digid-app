// Deze broncode is openbaar gemaakt vanwege een Woo-verzoek zodat deze 
// gericht is op transparantie en niet op hergebruik. Hergebruik van 
// de broncode is toegestaan onder de EUPL licentie, met uitzondering 
// van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit Woo-besluit kunt u mailen met open@logius.nl
//
// This code has been disclosed in response to a request under the Dutch
// Open Government Act ("Wet open Overheid"). This implies that publication 
// is primarily driven by the need for transparence, not re-use.
// Re-use is permitted under the EUPL-license, with the exception 
// of source files that contain a different license.
//
// The archive that this file originates from can be found at:
//   https://github.com/MinBZK/woo-besluit-broncode-digid-app
//
// Security vulnerabilities may be responsibly disclosed via the Dutch NCSC:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// using the reference "Logius, publicly disclosed source code DigiD-App" 
//
// Other questions regarding this Open Goverment Act decision may be
// directed via email to open@logius.nl
//
﻿using System;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class WebserviceConfirmViewModel : BaseViewModel
    {
        private readonly WebSessionInfoResponse _sessionInfo;
        private readonly WidSessionResponse _widSessionResponse;
        private ConfirmResponse _confirmResponse;
        private bool _timerSet;
        private bool _isConfirmed;
        private bool _isCancelled;
        public bool IsBlurVisible { get; set; }

        public string AppIcon { get; set; }
        public bool AppIconVisible => !string.IsNullOrEmpty(AppIcon);

#if A11YTEST
        public WebserviceConfirmViewModel() : this(new AuthenticateChallengeResponse { WebService = "A11YTest" })
        {
            App2AppSession.AppIconUrl = "";
            App2AppSession.App = new Common.Models.ResponseModels.App { Name = "Mijn DigiD" };
        }
#endif

        internal WebserviceConfirmViewModel(WebSessionInfoResponse sessionInfo, WidSessionResponse widSessionResponse = null)
        {
            _sessionInfo = sessionInfo;
            _widSessionResponse = widSessionResponse;
            PageId = "AP015";

            AppIcon = sessionInfo?.IconUri ?? App2AppSession.AppIconUrl;
            HeaderText = AppResources.LoginHeader;
            NavCloseCommand = CancelCommand;
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            //Only set timer when authentication is app authentication and not WID authentication
            if (_widSessionResponse == null)
                await SetTimer();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            _isCancelled = true;
        }

        private async Task SetTimer()
        {
            if (_timerSet)
                return;

            _timerSet = true;

            var delay = IsDemoMode ? 10 : 30;

            await Task.Delay(TimeSpan.FromSeconds(delay));

            IsTimerVisible = true;

            Device.StartTimer(TimeSpan.FromSeconds(IsDemoMode ? 0.5 : 1), () =>
            {
                SecondsUsed++;
                SecondsLeft--;

                if (SecondsLeft == 0)
                    ShowTimerAlert();

                return SecondsLeft > 0 && !_isConfirmed && !_isCancelled;
            });
        }

        private async void ShowTimerAlert()
        {
            await DependencyService.Get<IGeneralServices>().Cancel(false, true);
            await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.M19_Title, string.Format(AppResources.M19_Message, CustomerName), AppResources.OK);
            await NavigationService.PopToRoot(true);
        }

        public string CustomerName
        {
            get
            {

                if (_sessionInfo != null && _sessionInfo.IsOidcSession)
                    return _sessionInfo.WebService;

                if (!string.IsNullOrEmpty(App2AppSession.AppIconUrl))
                    return App2AppSession.App.Name;

                return _widSessionResponse != null ? _widSessionResponse.WebService : _sessionInfo.WebService;
            }
        }

        public int SecondsUsed { get; set; }
        public int SecondsLeft { get; set; } = 30;
        public bool LoginButtonEnabled => SecondsLeft > 0;
        public bool IsTimerVisible { get; set; }

        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;

                    _isCancelled = true;

                    if (_widSessionResponse != null)
                    {
                        await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                        await NavigationService.ShowMessagePage(MessagePageType.WIDCancelled);
                    }
                    else
                        await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.LoginCancelled));

                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        public AsyncCommand ConfirmCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;

                    DialogService.ShowProgressDialog();

                    _isConfirmed = true;

                    var confirmModel = new ConfirmRequest
                    {
                        AppId = DependencyService.Get<IMobileSettings>().AppId,
                    };

                    if (!string.IsNullOrEmpty(_sessionInfo?.HashedPIP))
                    {
                        var signingService = SigningHelper.GetService();
                        confirmModel.SignatureOfPIP = signingService.Sign(_sessionInfo.HashedPIP).ToBase16();
                    }

                    ApiResult result;

                    if (_widSessionResponse != null)
                        result = await DependencyService.Get<IEIDServices>().Confirm();
                    else
                    {
                        _confirmResponse = await DependencyService.Get<IAuthenticationService>().Confirm(confirmModel);
                        result = _confirmResponse.ApiResult;
                    }

                    DialogService.HideProgressDialog();

                    switch (result)
                    {
                        case ApiResult.Ok when _widSessionResponse != null:
                            await NavigationService.PushAsync(new WidScannerViewModel(new NfcScannerModel
                            {
                                Action = PinEntryType.Authentication,
                                EIDSessionResponse = _widSessionResponse
                            }));
                            break;
                        case ApiResult.Ok when _sessionInfo != null:
                            {
                                HttpSession.WebService = _sessionInfo.WebService;

                                if (HttpSession.IsApp2AppSession)
                                {
                                    if (DependencyService.Get<IMobileSettings>().NotificationPermissionSet == null)
                                    {
                                        await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RequestPushNotificationPermission), false, async res =>
                                        {
                                            await ContinueAuthenticationAsync();
                                        }));
                                    }
                                    else
                                        await ContinueAuthenticationAsync();

                                    return;
                                }

                                //inform user that webservice will upgrade in the future, and give option to upgrade to sub
                                if (_sessionInfo.NewAuthenticationLevelStartDate.HasValue && DependencyService.Get<IMobileSettings>().LoginLevel < _sessionInfo.NewAuthenticationLevel.ToLoginLevel())
                                {
                                    var action = App.HasNfc
                                        ? AuthenticationActions.ConfirmWebserviceUpgrade
                                        : AuthenticationActions.ConfirmWebserviceUpgradeNoNfc;

                                    await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(action)
                                    {
                                        SessionInfo = _sessionInfo,
                                    }, false, async _ =>
                                    {
                                        await ContinueAuthenticationAsync();
                                    }));

                                    return;
                                }

                                await ContinueAuthenticationAsync();
                                break;
                            }
                        case ApiResult.Nok:
                            await NavigationService.ShowMessagePage(MessagePageType.LoginCancelled);
                            App.ClearSession();
                            break;
                        default:
                            await NavigationService.ShowMessagePage(MessagePageType.LoginUnknown);
                            App.ClearSession();
                            break;
                    }

                    DialogService.HideProgressDialog();

                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        private async Task RegisterEmailTask(bool reRegister)
        {
            if (reRegister)
            {
                await Register();
            }
            else
                await NavigationService.PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RegisterEmailAddress), false, async isConfirmed =>
                {
                    if (isConfirmed)
                        await Register();
                    else
                        await ContinueAuthenticationAsync(MessagePageType.LoginSuccess);
                }));

            async Task Register()
            {
                await DependencyService.Get<INavigationService>().PushAsync(new EmailRegisterViewModel(new RegisterEmailModel
                {
                    AbortAction = async () => await ContinueAuthenticationAsync(MessagePageType.LoginSuccess),
                    NextAction = async () => await ContinueAuthenticationAsync(MessagePageType.EmailRegistrationSuccess)
                }));
            }
        }

        public AsyncCommand NextCommand { get; set; }

        private async Task ContinueAuthenticationAsync()
        {
            if (HttpSession.IsApp2AppSession)
            {
                await DependencyService.Get<IA11YService>().Speak(AppResources.LoginSuccessMessage);

                IsBlurVisible = true;
                NextCommand = new AsyncCommand(async () =>
                {
                    IsNextCommandExecuted = true;

                    PiwikHelper.Track("App2App", "Inloggen succes", NavigationService.CurrentPageId);
                    App.ShowSplashScreen(true, true);
                    await App2AppHelper.ReturnToClientApp();
                    App.ClearSession();
                }, () => !IsNextCommandExecuted);
                return;
            }

            var checkResult = await EmailHelper.CheckEmailStatus();

            if (checkResult != null)
            {
                switch (checkResult.Action)
                {
                    case ConfirmActions.ConfirmEmailAddress when checkResult.Confirmed:
                        await ContinueAuthenticationAsync(MessagePageType.LoginSuccess);
                        break;
                    case ConfirmActions.ConfirmEmailAddress:
                        await RegisterEmailTask(true);
                        break;
                    case ConfirmActions.ConfirmExistingEmailAddress when checkResult.Confirmed:
                        await DependencyService.Get<INavigationService>().PushAsync(new EmailConfirmViewModel(new RegisterEmailModel(true)
                        {
                            NextAction = async () => await ContinueAuthenticationAsync(MessagePageType.LoginSuccess),
                        }, checkResult.EmailAddress, true));
                        break;
                    case ConfirmActions.ConfirmExistingEmailAddress:
                        await RegisterEmailTask(false);
                        break;
                    case ConfirmActions.RegisterEmailAddress when checkResult.Confirmed:
                        await DependencyService.Get<INavigationService>().PushAsync(new EmailRegisterViewModel(new RegisterEmailModel
                        {
                            AbortAction = async () => await ContinueAuthenticationAsync(MessagePageType.LoginSuccess),
                            NextAction = async () => await ContinueAuthenticationAsync(MessagePageType.EmailRegistrationSuccess)
                        }));
                        break;
                    case ConfirmActions.RegisterEmailAddress:
                        await ContinueAuthenticationAsync(MessagePageType.LoginSuccess);
                        break;
                }

                return;
            }

            await ContinueAuthenticationAsync(MessagePageType.LoginSuccess);
        }

        private async Task ContinueAuthenticationAsync(MessagePageType messagePageType)
        {
            if (HttpSession.IsWeb2AppSession)
            {
                IsBlurVisible = true;
                await DependencyService.Get<IA11YService>().Speak(AppResources.LoginSuccessMessage);
                NextCommand = new AsyncCommand(async () =>
                {
                    IsNextCommandExecuted = true;
                    App.ShowSplashScreen(true, true);
                    await HandleWeb2AppSuccess(_sessionInfo, _widSessionResponse, _confirmResponse.NeedDeactivation);
                }, () => !IsNextCommandExecuted);
            }
            else
            {
                await NavigationService.ShowMessagePage(messagePageType);
                App.ClearSession();
            }
        }

        public static async Task HandleWeb2AppSuccess(WebSessionInfoResponse sessionInfo, WidSessionResponse widSessionResponse, bool resetApp)
        {
            PiwikHelper.Track("Web2App", "Inloggen succes", DependencyService.Get<INavigationService>().CurrentPageId);
            var url = widSessionResponse != null
                ? widSessionResponse.ReturnURL
                : sessionInfo.ReturnURL;

            if (Device.RuntimePlatform == Device.iOS && sessionInfo.IsConfirmAction)
            {
                url += $"?app_action={sessionInfo.Action}";
            }

            if (sessionInfo.IsOidcSession)
                await Launcher.OpenAsync(url);
            else
                await AuthenticationHelper.ProcessWebAuthentication(url);

            switch (sessionInfo.Action)
            {
                case AuthenticationActions.CANCEL_ACCOUNT:
                    await ResetApp(sessionInfo, true);
                    break;
                case AuthenticationActions.DeactivateApp:
                    await ResetApp(sessionInfo, resetApp);
                    break;
            }

            App.ClearSession();
        }

        private static async Task ResetApp(WebSessionInfoResponse sessionInfo, bool needReset)
        {
            if (needReset)
                App.Reset();

            switch (sessionInfo.Action)
            {
                case AuthenticationActions.CANCEL_ACCOUNT:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.AccountCancelledSuccess);
                    break;
                case AuthenticationActions.DeactivateApp:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.DeactivateAppSuccess);
                    break;
            }
        }
    }
}
