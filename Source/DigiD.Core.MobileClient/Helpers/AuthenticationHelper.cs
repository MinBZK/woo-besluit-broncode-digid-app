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
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.RDA.ViewModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.ViewModels;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Browser = DigiD.Common.Enums.Browser;

namespace DigiD.Helpers
{
    internal static class AuthenticationHelper
    {
        public static async Task<ValidatePinResponse> AuthenticateAsync(SecureString pin, ChallengeResponse response, string sessionId)
        {
#if !DEBUG
            if (DependencyService.Get<ISecurityService>().IsDebuggerAttached)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("debugger_attached", new System.Collections.Generic.Dictionary<string, string>
                {
                    {"flow","authentication"},
                    {"user_app_id",DependencyService.Get<IMobileSettings>().AppId},
                    {"instance_id",DependencyService.Get<IGeneralPreferences>().InstanceId}
                });
                return new ValidatePinResponse {ApiResult = ApiResult.Unknown};
            }
#endif
            var signingService = SigningHelper.GetService();

            var maskCode = DependencyService.Get<IMobileSettings>().MaskCode;
            var maskedPin = PinCodeHelper.MaskPin(pin, maskCode);

            byte[] signature;
            byte[] publicKey;

            try
            {
                signature = signingService.Sign(response.Challenge);
                publicKey = signingService.ExportPublicKey();
            }
            catch (Exception e)
            {
                AppCenterHelper.TrackError(e);
                return new ValidatePinResponse
                {
                    ApiResult = ApiResult.Failed
                };
            }

            var key = DependencyService.Get<IMobileSettings>().SymmetricKey.HexToByteArray();

            var requestData = new ValidatePinRequest(sessionId)
            {
                PIN = EncryptionHelper.Encrypt(maskedPin, response.IV.HexToByteArray(), key),
                PublicKey = publicKey.ToBase16(),
                Signature = signature.ToBase16(),
                AppId = DependencyService.Get<IMobileSettings>().AppId,
            };

            return await DependencyService.Get<IAuthenticationService>().ValidatePin(requestData);
        }

        internal static async Task ProcessWebAuthentication(string returnUrl)
        {
            if (DependencyService.Get<IDemoSettings>().IsDemo)
                return;

            if (Device.RuntimePlatform == Device.iOS)
            {
                switch (HttpSession.Browser)
                {
                    case Browser.Chrome:
                        returnUrl = $"googlechromes://{returnUrl.Replace("https://", "")}";
                        break;
                    case Browser.Firefox:
                        returnUrl = $"firefox://open-url?url={WebUtility.UrlEncode(returnUrl)}";
                        break;
                    case Browser.Edge:
                        returnUrl = $"microsoft-edge-https://{returnUrl.Replace("https://", "")}";
                        break;
                }

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        await Launcher.OpenAsync(returnUrl);
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e, new Dictionary<string, string> { { "ReturnUrl", returnUrl } });
                    }
                });
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IDevice>().OpenBrowser(HttpSession.SourceApplication, returnUrl);
            }
        }

        public static async Task<ChallengeResponse> GetChallenge(string sessionId)
        {
            var response = await DependencyService.Get<IAuthenticationService>().Challenge(new ChallengeRequest(sessionId)
            {
                AppId = DependencyService.Get<IMobileSettings>().AppId,
                InstanceId = DependencyService.Get<IGeneralPreferences>().InstanceId
            });

            switch (response.ApiResult)
            {
                case ApiResult.Nok:
                    {
                        switch (response.ErrorMessage)
                        {
                            case "aborted_contact_helpdesk":
                                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ContactHelpDesk);
                                return null;
                        }

                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.LoginUnknown);
                        return null;
                    }
                case ApiResult.Kill:
                    App.Reset();
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.AppDeactivated);
                    return null;
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    return null;
                case ApiResult.SSLPinningError:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SSLException);
                    return null;
                case ApiResult.HostNotReachable:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoInternetConnection);
                    return null;
                case ApiResult.Timeout:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NetworkTimeout);
                    return null;
                case ApiResult.Unknown:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.LoginUnknown);
                    return null;
                case ApiResult.SessionNotFound:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SessionTimeout);
                    return null;
            }

            if (string.IsNullOrEmpty(response.IV))
            {
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UnknownError);
                return null;
            }

            return response;
        }

        private static async Task ProcessAction(WebSessionInfoResponse sessionInfo)
        {
            switch (sessionInfo.Action)
            {
                case AuthenticationActions.RequestSession:
                case AuthenticationActions.ChangeAppPIN:
                    {
                        var model = new PinCodeModel(PinEntryType.Authentication);
                        await DependencyService.Get<INavigationService>().PushAsync(new PinCodeViewModel(model));
                        break;
                    }
                default:
                    {
                        var model = new ConfirmModel(sessionInfo.Action)
                        {
                            SessionInfo = sessionInfo
                        };
                        await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(model, false));
                        break;
                    }
            }
        }

        internal static async Task ContinueAuthenticationAsync()
        {
            var sessionInfo = await DependencyService.Get<IAuthenticationService>().SessionInfo();

            if (sessionInfo.ApiResult != ApiResult.Ok)
            {
                await DependencyService.Get<INavigationService>()
                    .PushAsync(new MessageViewModel(MessagePageType.UnknownError));
                return;
            }

            if (sessionInfo.Action == AuthenticationActions.RequestSession || sessionInfo.Action == AuthenticationActions.ChangeAppPIN)
            {
                var model = new PinCodeModel(PinEntryType.Authentication)
                {
                    SessionInfo = sessionInfo
                };

                await DependencyService.Get<INavigationService>().PushAsync(new PinCodeViewModel(model), false);
            }
            else
            {
                if (sessionInfo.IsConfirmAction)
                {
                    await ProcessAction(sessionInfo);
                }
                else
                {
                    if (DependencyService.Get<IMobileSettings>().LoginLevel < sessionInfo.AuthenticationLevel.ToLoginLevel())
                    {
                        if (App.HasNfc)
                        {
                            AppSession.Process = Process.UpgradeAndAuthenticate;
                            await DependencyService.Get<INavigationService>().PushAsync(new ConfirmRdaViewModel(async res =>
                            {
                                if (!res)
                                    await App.CancelSession(true, async () => await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.LoginCancelled));
                                else
                                    await ContinueRdaUpgradeWhileAuthenticate(sessionInfo);
                            }));
                        }
                        else
                        {
                            await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.InsufficientLoginLevel);
                            await App2AppHelper.AbortSession(AbortConstants.NoNFC, false);
                        }
                    }
                    else
                        await DependencyService.Get<INavigationService>().PushAsync(new WebserviceConfirmViewModel(sessionInfo));
                }
            }
        }

        internal static async Task ContinueRdaUpgradeWhileAuthenticate(WebSessionInfoResponse sessionInfo, Func<bool, Task> resultAction = null)
        {
            DependencyService.Get<IDialog>().ShowProgressDialog();

            //There should not be activation session data while authentication
            HttpSession.ActivationSessionData = null;
            
            var response = await DependencyService.Get<IRdaServices>().InitSessionRDA();
            
            if (response.ApiResult != ApiResult.Ok)
            {
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UnknownError);
                DependencyService.Get<IDialog>().HideProgressDialog();
                return;
            }

            HttpSession.TempSessionData = new TempSessionData(HttpSession.AppSessionId);
            HttpSession.AppSessionId = response.SessionId;

            var session = await RdaViewModel.GetRdaSessionUrl();

            switch (session.ApiResult)
            {
                case ApiResult.Ok:
                    await DependencyService.Get<INavigationService>().PushAsync(new RdaViewModel(session, async s => await CompleteAction(s, sessionInfo, resultAction), "AP038"));
                    break;
                case ApiResult.NoDocumentsFound:
                    await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(ConfirmActions.RdaNoDocumentsFound), false, async success => await RdaResultAction(success, async s => await CompleteAction(s, sessionInfo, resultAction))));
                    DependencyService.Get<IDialog>().HideProgressDialog();
                    break;
                case ApiResult.Nok:
                    if (resultAction == null)
                    {
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UnknownError);
                        DependencyService.Get<IDialog>().HideProgressDialog();
                    }
                    break;
            }
        }

        private static async Task CompleteAction(bool status, WebSessionInfoResponse sessionInfo, Func<bool, Task> resultAction)
        {
            if (status)
            {
                HttpSession.TempSessionData?.SwitchSession();
                
                if (resultAction != null)
                    await resultAction.Invoke(true);
                else
                    await DependencyService.Get<INavigationService>().PushAsync(new WebserviceConfirmViewModel(sessionInfo));
            }
            else
            {
                if (sessionInfo.AuthenticationLevel.ToLoginLevel() > DependencyService.Get<IMobileSettings>().LoginLevel)
                {
                    await DependencyService.Get<IRdaServices>().Cancel();
                    await App.CancelSession(false, async () => await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UpgradeAndAuthenticateAborted));
                }
                else if (AppSession.Process == Process.UpgradeAndAuthenticate)
                {
                    await DependencyService.Get<IRdaServices>().Cancel();
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UpgradeAndAuthenticateFailed, sessionInfo.ReturnURL);
                }
                else
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UpgradeAccountWithNFCFailed);
            }
        }

        private static async Task RdaResultAction(bool success, Func<bool, Task> completeAction)
        {
            if (success)
                await DependencyService.Get<INavigationService>().PushAsync(new AP109ViewModel(completeAction, RdaRetryAction));
            else
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoValidDocuments);
        }

        private static async Task RdaRetryAction()
        {
            await DependencyService.Get<INavigationService>().PushAsync(new RdaScanFailedViewModel(HttpSession.ActivationSessionData.ActivationMethod));
        }
    }
}

