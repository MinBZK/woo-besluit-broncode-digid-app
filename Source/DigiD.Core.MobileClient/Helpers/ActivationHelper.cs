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
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.ViewModels;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    internal static class ActivationHelper
    {
        internal static async Task RegisterEmailTaskAsync(RegisterEmailModel model)
        {
            if ((HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession) && HttpSession.TempSessionData != null)
                await ContinueAuthentication();
            else
            {
                //If new account is request, no need to check if email address is already verified
                if (model.ActivationMethod != ActivationMethod.RequestNewDigidAccount)
                {
                    var checkResult = await EmailHelper.CheckEmailStatus();

                    if (checkResult != null)
                    {
                        switch (checkResult.Action)
                        {
                            case ConfirmActions.RegisterEmailAddress when !checkResult.Confirmed:
                            case ConfirmActions.ConfirmEmailAddress when checkResult.Confirmed:
                                await ContinueActivation(model);
                                break;
                            case ConfirmActions.ConfirmExistingEmailAddress when !checkResult.Confirmed:
                            case ConfirmActions.RegisterEmailAddress:
                            case ConfirmActions.ConfirmEmailAddress when !checkResult.Confirmed:
                                model.NextAction = async () => await ContinueActivation(model);
                                await DependencyService.Get<INavigationService>()
                                    .PushAsync(new EmailRegisterViewModel(model));
                                break;
                            case ConfirmActions.ConfirmExistingEmailAddress:
                                await DependencyService.Get<INavigationService>().PushAsync(new EmailConfirmViewModel(new RegisterEmailModel(true)
                                {
                                    NextAction = async () => await ContinueActivation(model),
                                }, checkResult.EmailAddress, true));
                                break;
                        }
                    }
                    else
                        await ContinueActivation(model);
                }
                else
                {
                    model.NextAction = async () => await ContinueActivation(model);
                    await DependencyService.Get<INavigationService>().PushAsync(new EmailRegisterViewModel(model));
                }
            }
        }

        internal static async Task HandlePendingCompletion()
        {
            var method = HttpSession.ActivationSessionData?.ActivationMethod ?? ActivationMethod.RequestNewDigidAccount;

            DependencyService.Get<IMobileSettings>().ActivationStatus = ActivationStatus.Pending;
            AppSession.IsAppActivated = false;
            DependencyService.Get<IMobileSettings>().Save();
            DependencyService.Get<IPreferences>().LetterRequestDate = DateTimeOffset.UtcNow;

            if (method != ActivationMethod.RequestNewDigidAccount)
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationPending);
            else
            {
                var model = new RegisterEmailModel(method, false);
                await RegisterEmailTaskAsync(model);
            }
        }

        internal static async Task AskPushNotificationPermissionAsync(ActivationMethod type, bool upgradeRda = false)
        {
            var model = new RegisterEmailModel(type, upgradeRda);

            if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                await HandlePendingCompletion();
            else
            {
                var response = await DependencyService.Get<IAccountInformationServices>().GetTwoFactorStatus();
                var twoFactorEnabled = response != null && (response.IsAppOnlyAccount || response.Enabled);
                var notificationsAvailable = DependencyService.Get<IPushNotificationService>().NotificationsAvailable;

                if (twoFactorEnabled && !notificationsAvailable)
                    await RegisterEmailTaskAsync(model);
                else
                    await DependencyService.Get<INavigationService>().PushAsync(new AP118ViewModel(twoFactorEnabled, model));
            }
        }

        internal static async Task ContinueActivation(RegisterEmailModel model)
        {
            App.IsNewActivated = true;

            if (model.UpgradeRda)
                await DependencyService.Get<INavigationService>().PushAsync(new ActivationRdaCompletedViewModel(model));
            else if (model.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(new ConfirmModel(AuthenticationActions.LetterNotification), false));
            else
                await DependencyService.Get<INavigationService>().PushAsync(new ActivationCompletedViewModel(model));
        }

        internal static async Task ContinueLetterActivation()
        {
            await SessionHelper.StartSession(async () =>
            {
                var response = await DependencyService.Get<IEnrollmentServices>().InitSessionActivationCode(new ActivationCodeSessionRequest
                {
                    AppId = DependencyService.Get<IMobileSettings>().AppId
                });

                if (response.ApiResult == ApiResult.Ok)
                    HttpSession.AppSessionId = response.SessionId;

                await AuthenticationHelper.ContinueAuthenticationAsync();
            });
        }

        internal static async Task<RequestStationSessionResponse> InitSessionRequestStation(bool needAuthentication, string userName, string password)
        {
            var requestData = new RequestStationSessionRequest
            {
                Authenticate = needAuthentication.ToString().ToLowerInvariant(),
                UserName = userName,
                Password = password,
                DeviceName = Xamarin.Essentials.DeviceInfo.Name.ReplaceEmoticons(),
                InstanceId = DependencyService.Get<IGeneralPreferences>().InstanceId,
            };

            return await DependencyService.Get<IRequestStationServices>().InitSessionRequestStation(requestData);
        }

        internal static async Task<SessionResponse> InitSession(bool removeOldApp = false, string smsCode = null)
        {
            var requestData = new BasicAuthenticationSessionRequest
            {
                InstanceId = DependencyService.Get<IGeneralPreferences>().InstanceId,
                SMSCode = smsCode,
                DeviceName = Xamarin.Essentials.DeviceInfo.Name.ReplaceEmoticons(),
                RemoveOldApp = removeOldApp
            };

            //Start session
            return await DependencyService.Get<IEnrollmentServices>().InitSession(requestData);
        }

        internal static async Task<ActivationResult> StartActivationByLetterOrPassword()
        {
            var result = await InitSession();

            var res = new ActivationResult();

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
                            res = await RequestNewAccount(result);
                        else
                            res = await RequestLetter(result);
                        break;
                    }
                case ApiResult.Nok:
                    {
                        res.MessagePageType = MessagePageType.ActivationFailed;
                        break;
                    }
                case ApiResult.SessionNotFound:
                    {
                        res.MessagePageType = MessagePageType.SessionTimeout;
                        break;
                    }
                case ApiResult.Disabled:
                    {
                        res.MessagePageType = MessagePageType.ActivationDisabled;
                        break;
                    }
            }

            return res;
        }

        internal static async Task<ActivationResult> StartActivationWithoutSms()
        {
            var activationResult = new ActivationResult();
            var result = await InitSession();

            activationResult.ApiResult = result.ApiResult;

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    activationResult = await StartChallenge(result.AppId);
                    break;
                case ApiResult.SessionNotFound:
                    activationResult.MessagePageType = MessagePageType.SessionTimeout;
                    break;
                case ApiResult.Disabled:
                    activationResult.MessagePageType = MessagePageType.ActivationDisabled;
                    break;
                case ApiResult.HostNotReachable:
                    // doe niets wordt afgehandeld in aanroep
                    break;
                default:
                    activationResult.MessagePageType = MessagePageType.UnknownError;
                    break;
            }

            return activationResult;
        }

        internal static async Task<ActivationResult> RequestNewAccount(SessionResponse result)
        {
            //Initialize request for new account
            var activationResult = await StartChallenge(result.AppId);

            switch (activationResult.ApiResult)
            {
                case ApiResult.Blocked:
                    activationResult.MessagePageType = MessagePageType.RequestAccountAccountBlocked;
                    break;
                case ApiResult.Aborted:
                    activationResult.MessagePageType = MessagePageType.AuthenticationAborted;
                    break;
                case ApiResult.Cancelled:
                    activationResult.MessagePageType = MessagePageType.ActivationCancelled;
                    break;
            }

            return activationResult;
        }

        internal static async Task<ActivationResult> RequestLetter(SessionResponse result)
        {
            var activationResult = new ActivationResult();

            var initResult = await DependencyService.Get<IEnrollmentServices>().InitSessionLetterActivation();

            switch (initResult.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        var pollResult = await DependencyService.Get<IEnrollmentServices>().LetterActivationPolling();

                        while (pollResult.ApiResult == ApiResult.Pending)
                        {
                            pollResult = await DependencyService.Get<IEnrollmentServices>().LetterActivationPolling();
                            await Task.Delay(1000);
                        }

                        switch (pollResult.ApiResult)
                        {
                            case ApiResult.Ok:
                                if (result != null)
                                    activationResult = await StartChallenge(result.AppId);
                                else
                                    return new ActivationResult { ApiResult = ApiResult.Ok };
                                break;
                            case ApiResult.Nok:
                                {
                                    switch (pollResult.ErrorMessage)
                                    {
                                        case "gba_deceased": //Persoon is overleden
                                        case "gba_invalid": //ongeldige GBA
                                            activationResult.MessagePageType = MessagePageType.GBAFailed;
                                            break;

                                        default:
                                            activationResult.MessagePageType = MessagePageType.ActivationFailed;
                                            break;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case ApiResult.Nok:
                    {
                        switch (initResult.ErrorMessage)
                        {
                            case "too_soon": //Minimale tijd (in dagen) tussen twee DigiD app aanvragen per brief (1 aanvraag per x dagen)
                                activationResult.MessagePageType = MessagePageType.ActivationFailedTooSoon;
                                break;
                            case "too_fast": //Blokkering DigiD app aanvragen via brief (aantal per maand) /Deprecated
                            case "too_often":
                                BaseMessageViewModel.Payload = initResult.Payload;
                                activationResult.MessagePageType = MessagePageType.ActivationFailedTooFast;
                                break;
                            case "no_bsn_on_account":
                                activationResult.MessagePageType = MessagePageType.NoBSN;
                                break;
                            case "too_many_letter_requests":
                                activationResult.MessagePageType = MessagePageType.UnknownError;
                                break;
                        }
                    }
                    break;
                case ApiResult.Disabled:
                    {
                        activationResult.MessagePageType = MessagePageType.ActivationDisabled;
                        break;
                    }
            }

            return activationResult;
        }

        internal static async Task<ActivationResult> StartChallenge(string appId)
        {
            var res = new ActivationResult();

            var signingService = SigningHelper.GetService();

            if (signingService.GenerateKeyPair())
            {
                var publicKey = signingService.ExportPublicKey();

                if (publicKey == null)
                {
                    res.ApiResult = ApiResult.Error;
                    res.MessagePageType = MessagePageType.ActivationFailed;
                    return res;
                }

                var enrollmentChallengeRequestData = new EnrollmentChallengeRequest
                {
                    AppId = appId,
                    PublicKey = publicKey.ToBase16()
                };

                var response = await DependencyService.Get<IEnrollmentServices>().EnrollmentChallenge(enrollmentChallengeRequestData);
                res.ApiResult = response.ApiResult;

                if (response.ApiResult == ApiResult.Ok)
                {
                    DependencyService.Get<IMobileSettings>().AppId = appId;
                    var signature = signingService.Sign(response.Challenge);
                    var challengeResult = await CompleteChallenge(signature, publicKey, signingService.HardwareSupport);

                    res.IV = challengeResult.IV;
                    res.ApiResult = challengeResult.ApiResult;
                }

                switch (res.ApiResult)
                {
                    case ApiResult.Nok:
                        res.MessagePageType = MessagePageType.ChallengeFailed;
                        break;
                    case ApiResult.HostNotReachable:
                        res.MessagePageType = MessagePageType.NoInternetConnection;
                        break;
                    case ApiResult.SSLPinningError:
                        res.MessagePageType = MessagePageType.SSLException;
                        break;
                    case ApiResult.Disabled:
                        res.MessagePageType = MessagePageType.ActivationDisabled;
                        break;
                }

                return res;
            }

            res.ApiResult = ApiResult.Error;
            res.MessagePageType = MessagePageType.ActivationFailed;

            return res;
        }

        internal static async Task<CompleteChallengeResponse> CompleteChallenge(byte[] signature, byte[] publicKey, bool hardwareSupport)
        {
            var requestData = new CompleteChallengeRequest
            {
                Signature = signature.ToBase16(),
                PublicKey = publicKey.ToBase16(),
                HardwareSupport = hardwareSupport,
            };

            try
            {
                requestData.NFCSupport = App.HasNfc;
            }
            catch (Exception)
            {
                requestData.NFCSupport = false;
            }

            var response = await DependencyService.Get<IEnrollmentServices>().CompleteChallenge(requestData);

            if (response.ApiResult == ApiResult.Ok)
            {
                DependencyService.Get<IMobileSettings>().SymmetricKey = response.SymmetricKey;
            }

            return response;
        }

        internal static async Task CompleteActivation()
        {
            AppSession.SetAccountStatus(await DependencyService.Get<IAccountInformationServices>().GetAccountStatus());

            if (HttpSession.TempSessionData != null && HttpSession.ActivationSessionData != null && (HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession))
                await ContinueAuthentication();
            else
                await ShowActivationCompleteMessage();
        }

        private static async Task ContinueAuthentication()
        {
            HttpSession.TempSessionData.SwitchSession();
            await AuthenticationHelper.ContinueAuthenticationAsync();
        }

        private static async Task ShowActivationCompleteMessage()
        {
            App.ClearSession();
            await DependencyService.Get<INavigationService>().PopToRoot(true);
        }

        internal static async Task StartActivationWithRDA()
        {
            DependencyService.Get<IDialog>().ShowProgressDialog();

            await App.GetActualConfiguration();

            if (!App.Configuration.RDAEnabled)
            {
                DependencyService.Get<IDialog>().HideProgressDialog();
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.RDADisabled);
                return;
            }

            var initResult = await DependencyService.Get<IEnrollmentServices>().InitSessionActivationRDA();

            if (initResult.ApiResult == ApiResult.Ok)
            {
                var result = await InitSession();

                switch (result.ApiResult)
                {
                    case ApiResult.Ok:
                        {
                            AppSession.Process = Process.AppActivateWithRDA;
                            var activationResult = await StartChallenge(result.AppId);

                            if (activationResult.ApiResult == ApiResult.Ok)
                                await DependencyService.Get<INavigationService>().PushAsync(new PinCodeViewModel(new PinCodeModel(PinEntryType.Enrollment)
                                {
                                    IV = activationResult.IV
                                }));
                            else
                                await DependencyService.Get<INavigationService>().ShowMessagePage(activationResult.MessagePageType);

                            break;
                        }
                    case ApiResult.Disabled:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.RDADisabled);
                        break;
                    default:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationFailed);
                        break;
                }
            }
            else
            {
                if (initResult.ErrorMessage == "no_bsn_on_account")
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoBSN);
                else
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.RDADisabled);
            }

            DependencyService.Get<IDialog>().HideProgressDialog();
        }

        internal static async Task StartActivationWithAlternative()
        {
            if (HttpSession.ActivationSessionData != null)
                await ActivationLoginViewModel.StartActivationAsync(null);
            else
                await DependencyService.Get<INavigationService>().PopToRoot();
        }

        public static async Task ActivateAsync(SecureString pin, string iv, bool alternative = false)
        {
#if !DEBUG
            if (DependencyService.Get<ISecurityService>().IsDebuggerAttached)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("debugger_attached", new System.Collections.Generic.Dictionary<string, string>
                {
                    {"flow","activation"},
                    {"user_app_id",DependencyService.Get<IMobileSettings>().AppId},
                    {"instance_id",DependencyService.Get<IGeneralPreferences>().InstanceId}
                });
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationFailed);
            }
#endif
            DependencyService.Get<IDialog>().ShowProgressDialog();

            var mask = PinCodeHelper.GenerateMaskCode();
            var maskedPin = PinCodeHelper.MaskPin(pin, mask);
            var key = DependencyService.Get<IMobileSettings>().SymmetricKey.HexToByteArray();
            var requestData = new PincodeRequest
            {
                AppId = DependencyService.Get<IMobileSettings>().AppId,
                MaskedPincode = EncryptionHelper.Encrypt(maskedPin, iv.HexToByteArray(), key),
            };

            //ActivateAsync device
            var result = await DependencyService.Get<IEnrollmentServices>().CompleteActivationAsync(requestData);

            if (AppSession.AppMode == AppMode.Demo && (result.ApiResult == ApiResult.Ok || result.ApiResult == ApiResult.Pending))
            {
                Common.Http.Helpers.DemoHelper.Sessions[requestData.AppSessionId].DateTime = DateTime.Now;
                Common.Http.Helpers.DemoHelper.Sessions[requestData.AppSessionId].IsAuthenticated = true;

                DependencyService.Get<IDemoSettings>().DemoPin = maskedPin;

                if (!string.IsNullOrEmpty(DemoHelper.CurrentUser.EmailAddress))
                    DependencyService.Get<IDemoSettings>().EmailAddress = DemoHelper.CurrentUser.EmailAddress;

                DependencyService.Get<IDemoSettings>().Save();
            }

            DependencyService.Get<IDialog>().HideProgressDialog();

            if (result.ApiResult == ApiResult.Ok || result.ApiResult == ApiResult.Pending)
            {
                AppSession.AuthenticationSessionId = HttpSession.AppSessionId;
                DependencyService.Get<IMobileSettings>().MaskCode = mask;
            }

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        await HandleActivationCompletion(result);
                        return;
                    }
                case ApiResult.Pending:
                    {
                        if (AppSession.Process == Process.AppActivateWithRDA && !alternative || HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount && App.HasNfc)
                            await RdaHelper.HandleRdaCompletion(pin);
                        else
                            await HandlePendingCompletion();

                        return;
                    }
                case ApiResult.Nok:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationFailed);
                    break;
                case ApiResult.Disabled:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationDisabled);
                    break;
                case ApiResult.HostNotReachable:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoInternetConnection);
                    break;
                case ApiResult.SessionNotFound:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SessionTimeout);
                    break;
                case ApiResult.Unknown:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.LoginUnknown);
                    break;
                case ApiResult.SSLPinningError:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SSLException);
                    break;
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    break;
                default:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationFailed);
                    break;
            }

            App.ClearSession();
        }

        private static async Task HandleActivationCompletion(CompleteActivationResponse result)
        {
            if (HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession)
                HttpSession.TempSessionData ??= new TempSessionData(HttpSession.AppSessionId);

            DependencyService.Get<IMobileSettings>().ActivationStatus = ActivationStatus.Activated;
            AppSession.IsAppActivated = true;
            DependencyService.Get<IMobileSettings>().LoginLevel = result.AuthenticationLevel.ToLoginLevel();
            DependencyService.Get<IMobileSettings>().Save();

            switch (AppSession.Process)
            {
                case Process.AppActivationViaRequestStation:
                    await AskPushNotificationPermissionAsync(ActivationMethod.Balie);
                    break;
                default:
                    {
                        await AskPushNotificationPermissionAsync(HttpSession.ActivationSessionData.ActivationMethod, App.HasNfc && (HttpSession.ActivationSessionData.ActivationMethod == ActivationMethod.AccountAndApp || HttpSession.ActivationSessionData.ActivationMethod == ActivationMethod.AccountAndApp && DependencyService.Get<IMobileSettings>().LoginLevel < LoginLevel.Substantieel));
                        break;
                    }
            }
        }
    }

    internal class ActivationResult
    {
        public MessagePageType MessagePageType { get; set; }
        public ApiResult ApiResult { get; set; }
        public string IV { get; set; }
    }
}
