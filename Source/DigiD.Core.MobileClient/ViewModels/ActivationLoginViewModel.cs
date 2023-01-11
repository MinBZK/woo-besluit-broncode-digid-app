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
﻿using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class ActivationLoginViewModel : BaseActivationViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsValid => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !IsError;
        public string MessageText { get; set; }
        public AsyncCommand ButtonSecondaryCommand { get; set; }
        public string CancelText { get; set; }

        public ActivationLoginViewModel()
        {
            HeaderText = AppResources.ActivationHeader;
            FooterText = AppResources.ActivationStep1Message;

            if (AppSession.Process == Process.AppActivationViaRequestStation)
            {
                PageId = "AP059";
                MessageText = AppResources.AP092_MessageText;
                CancelText = AppResources.AP092_CancelButtonText;
                ButtonSecondaryCommand = new AsyncCommand(async () => { await StartRequestStationSession(false); });
                ButtonCommand = new AsyncCommand(async () => { await StartRequestStationSession(true); });
            }
            else
            {
                PageId = "AP043";
                HasBackButton = true;
                CancelText = AppResources.NoDigiD;
                ButtonCommand = LoginCommand;
                ButtonSecondaryCommand = new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;
                    await NavigationService.PushAsync(new NoDigiDViewModel());
                    CanExecute = true;
                });
            }
            NavCloseCommand = CancelCommand;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
#if !PROD && !PREPROD
//SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
//SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
#endif
            if (propertyName == nameof(Username) || propertyName == nameof(Password))
            {
                IsError = false;
                FooterText = AppResources.ActivationStep1Message;
            }
        }

        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (HttpSession.TempSessionData != null && HttpSession.IsApp2AppSession)
                        await App2AppHelper.AbortSession("cancelled");
                    else
                        await App.CancelSession(true, async () => await NavigationService.PopToRoot(true));
                });
            }
        }

        public AsyncCommand LoginCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    CanExecute = false;
                    await Login(false);
                    CanExecute = true;
                }, () => IsValid && CanExecute);
            }
        }

        private async Task Login(bool removeOldApp)
        {
            var isDemoUser = Common.Mobile.Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.UserName == Username) != null;
            App.RegisterServices(isDemoUser ? AppMode.Demo : AppMode.Normal);

            if (!await DependencyService.Get<IGeneralServices>().SslPinningCheck())
            {
                await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                return;
            }

            var response = await LoginAsync(Username, Password, removeOldApp, !removeOldApp);

            await HandleResponse(response);
        }

        private static async Task<BasicAuthenticationResponse> LoginAsync(string userName, string password, bool removeOldApp, bool ignoreError = false)
        {
            DependencyService.Get<IDialog>().ShowProgressDialog();

            var requestData = new BasicAuthenticateRequest
            {
                Username = userName,
                Password = password,
                DeviceName = Xamarin.Essentials.DeviceInfo.Name.ReplaceEmoticons(),
                InstanceId = DependencyService.Get<IGeneralPreferences>().InstanceId,
                RemoveOldApp = removeOldApp
            };

            var result = await DependencyService.Get<IEnrollmentServices>().BasicAuthenticate(requestData);

            if (result.ApiResult == ApiResult.Ok)
                HttpSession.AppSessionId = result.SessionId;
            else if (!ignoreError)
            {
                switch (result.ErrorMessage)
                {
                    case "account_blocked":
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.AccountBlocked, result.Payload.minutes);
                        break;
                    default:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationFailed);
                        break;
                }
            }

            if (HttpSession.ActivationSessionData == null && App.HasNfc)
            {
                HttpSession.ActivationSessionData = new ActivationSessionData
                {
                    ActivationMethod = result.ActivationMethod,
                    RemoveOldApp = removeOldApp
                };
            }
            else if (HttpSession.ActivationSessionData is { ActivationMethod: ActivationMethod.Unsupported })
            {
                HttpSession.ActivationSessionData.ActivationMethod = result.ActivationMethod;
            }

            if (HttpSession.ActivationSessionData != null)
            {
                HttpSession.ActivationSessionData.Password = password;
                HttpSession.ActivationSessionData.UserName = userName;
            }

            DependencyService.Get<IDialog>().HideProgressDialog();

            return result;
        }

        public static async Task StartActivationAsync(BasicAuthenticationResponse result)
        {
            var first = true;

            if (result == null)
            {
                result = await LoginAsync(HttpSession.ActivationSessionData.UserName, HttpSession.ActivationSessionData.Password, HttpSession.ActivationSessionData.RemoveOldApp);
                first = false;
            }

            HttpSession.AppSessionId = result.SessionId;

            switch (result.ActivationMethod)
            {
                case ActivationMethod.SMS:
                    if (first && App.HasNfc)
                        await ActivationHelper.StartActivationWithRDA();
                    else
                    {
                        AppSession.Process = Process.NotSet;

                        if (HttpSession.ActivationSessionData == null)
                            HttpSession.ActivationSessionData = new ActivationSessionData();

                        HttpSession.ActivationSessionData.ActivationMethod = ActivationMethod.SMS;
                        await DependencyService.Get<INavigationService>().PushAsync(new ActivationSmsViewModel());
                    }
                    break;
                case ActivationMethod.Password:
                    if (first && App.HasNfc)
                        await ActivationHelper.StartActivationWithRDA();
                    else
                    {
                        var activationResult = await ActivationHelper.StartActivationByLetterOrPassword();

                        if (activationResult.ApiResult == ApiResult.Ok)
                        {
                            if (HttpSession.ActivationSessionData != null)
                            {
                                AppSession.Process = Process.NotSet;
                                await ActivationHelper.ActivateAsync(HttpSession.ActivationSessionData.Pin, activationResult.IV, true);
                            }
                            else
                            {
                                HttpSession.ActivationSessionData = new ActivationSessionData { ActivationMethod = result.ActivationMethod };
                                var model = new PinCodeModel(PinEntryType.Enrollment)
                                {
                                    IV = activationResult.IV
                                };

                                await DependencyService.Get<INavigationService>().PushAsync(new PinCodeViewModel(model));
                            }
                        }
                        else
                            await DependencyService.Get<INavigationService>().ShowMessagePage(activationResult.MessagePageType);
                        
                    }
                    break;
                case ActivationMethod.AccountAndApp:
                    HttpSession.ActivationSessionData = new ActivationSessionData { ActivationMethod = ActivationMethod.AccountAndApp };

                    if (result.IsSmsCheckRequested)
                        await DependencyService.Get<INavigationService>().PushAsync(new ActivationSmsViewModel());
                    else
                    {
                        var activationResult = await ActivationHelper.StartActivationWithoutSms();
                        switch (activationResult.ApiResult)
                        {
                            case ApiResult.Ok:
                                await DependencyService.Get<INavigationService>().PushAsync(new ActivationLetterViewModel(false, activationResult.IV));
                                break;
                            case ApiResult.HostNotReachable:
                                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoInternetConnection);
                                break;
                            default:
                                await DependencyService.Get<INavigationService>().ShowMessagePage(activationResult.MessagePageType);
                                break;
                        }
                    }
                    break;
            }
        }

        private async Task HandleResponse(BaseResponse result)
        {
            var basicAuthenticationResponse = result as BasicAuthenticationResponse;
            var requestStationResponse = result as RequestStationSessionResponse;

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        if (basicAuthenticationResponse is { PendingActivation: true } && !App.HasNfc)
                        {
                            NextCommand = new AsyncCommand(async () =>
                            {
                                IsNextCommandExecuted = true;
                                await NavigationService.PushAsync(new ActivationConfirmViewModel(async () => await StartActivationAsync(basicAuthenticationResponse)));
                            }, () => !IsNextCommandExecuted);
                        }
                        else
                        {
                            NextCommand = new AsyncCommand(async () =>
                            {
                                IsNextCommandExecuted = true;
                                if (AppSession.Process == Process.AppActivationViaRequestStation)
                                    await NavigationService.PushAsync(new ActivationRequestStationViewModel(requestStationResponse));
                                else
                                    await StartActivationAsync(basicAuthenticationResponse);

                            }, () => !IsNextCommandExecuted);
                        }

                        SetNavbar(false);
                        IsBlurVisible = true;

                        break;
                    }
                case ApiResult.Nok:
                    {
                        await HandleNok(result, basicAuthenticationResponse);
                        break;
                    }
                case ApiResult.Timeout:
                    await NavigationService.ShowMessagePage(MessagePageType.NetworkTimeout);
                    break;
                case ApiResult.Disabled:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                        break;
                    }
                case ApiResult.HostNotReachable:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.NoInternetConnection);
                        break;
                    }
                case ApiResult.SessionNotFound:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                        break;
                    }
                case ApiResult.SSLPinningError:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                        break;
                    }
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    break;
                default:
                    await NavigationService.ShowMessagePage(MessagePageType.ActivationFailed);
                    break;
            }
        }

        private async Task HandleNok(BaseResponse result, ActivationResponse basicAuthenticationResponse)
        {
            var footerText = string.Empty;
            switch (result.ErrorMessage)
            {
                case "invalid":
                    PageId = "AP232";
                    Username = string.Empty;
                    Password = string.Empty;
                    footerText = AppResources.SingleDeviceLoginInvalid;
                    break;
                case "request_for_account_basic":
                    if (basicAuthenticationResponse != null)
                        await DependencyService.Get<IGeneralServices>().Cancel();
                    await NavigationService.ShowMessagePage(MessagePageType.ActivationPendingNoSMSCheck);
                    return;
                case "account_inactive":
                    PageId = "AP233";
                    footerText = AppResources.SingleDeviceAccountInactive;
                    break;
                case "account_blocked":
                    PageId = "AP234";
                    footerText = string.Format(CultureInfo.InvariantCulture, AppResources.SingleDeviceAccountBlocked, result.Payload?.minutes ?? 15);
                    break;
                case "no_bsn_on_account":
                    PageId = "AP235";
                    footerText = AppResources.SingleDeviceNoBSNOnAccount;
                    break;
                case "too_many_active":
                    if (basicAuthenticationResponse != null)
                    {
                        await NavigationService.PushModalAsync(new ToManyActiveAppsViewModel(basicAuthenticationResponse, async () =>
                        {
                            await Login(true);
                        }));
                        return;
                    }
                    break;
                case "classified_deceased":
                    await NavigationService.ShowMessagePage(MessagePageType.ClassifiedDeceased);
                    return;
            }

            IsError = true;

            MessagingCenter.Send(this, MessagingConstants.FindDefaultElement);
            TrackView();

            if (!string.IsNullOrEmpty(footerText))
            {
                if (DependencyService.Get<IA11YService>().IsInVoiceOverMode() && Device.RuntimePlatform == Device.Android)
                    await Task.Delay(2000);

                FooterText = footerText;
            }
        }

        private async Task StartRequestStationSession(bool needAuthentication)
        {
            var isDemoUser = Common.Mobile.Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.UserName == Username) != null;
            App.RegisterServices(isDemoUser ? AppMode.Demo : AppMode.Normal);

            var result = await ActivationHelper.InitSessionRequestStation(needAuthentication, Username, Password);

            await HandleResponse(result);
        }

        public override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
