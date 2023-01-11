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
using System.Globalization;
using System.Security;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Constants;
using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using DigiD.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class PinCodeViewModel : BasePinCodeViewModel
    {
        private string _iv;

        public bool LogoVisible => PreventLock;
        public bool IsHeaderVisible { get; set; }
        public AsyncCommand PincodeForgottenCommand { get; }
        public string AccessibilityInputHint => string.Format(CultureInfo.InvariantCulture, AppResources.AccessibilityPinInput, NumberOfTiles, Model.EntryType.ToString().ToLowerInvariant().Contains("puk") ? "PUK" : "PIN");

#if A11YTEST
        public PinCodeViewModel() : this(new PinCodeModel(PinEntryType.Enrollment)) { }
#endif

        public PinCodeViewModel(PinCodeModel model) : base(model)
        {
            IsHeaderVisible = !model.IsAppAuthentication && !model.ChangeAppPin;
            PreventLock = model.IsAppAuthentication;
            HasBackButton = EIDSession.Card == null && HttpSession.ActivationSessionData == null;

            if (!model.IsAppAuthentication)
                NavCloseCommand = CancelCommand;

            if (Model.Card == null && Model.EntryType == PinEntryType.Authentication)
                PincodeForgottenCommand = new AsyncCommand(async () => await DependencyService.Get<INavigationService>().PushAsync(new AP099ViewModel()));
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            if (Model.IsAppAuthentication && WhatsNewHelper.MustShow)
                await WhatsNewHelper.Show();
        }

        public override async Task PINCompleteAction()
        {
            await ValidatePinCode();
        }

        public override async Task ChangePINAction()
        {
            DialogService.ShowProgressDialog();

            var response = await DependencyService.Get<IChangePinService>().InitSession();
            HttpSession.AppSessionId = response.AppSessionId;

            var mask = PinCodeHelper.GenerateMaskCode();
            var maskedPin = PinCodeHelper.MaskPin(Pin2, mask);
            var key = DependencyService.Get<IMobileSettings>().SymmetricKey.HexToByteArray();
            var encryptedPin = EncryptionHelper.Encrypt(maskedPin, _iv.HexToByteArray(), key);
            var changePinResult = await DependencyService.Get<IChangePinService>().ChangePIN(encryptedPin);

            switch (changePinResult.ApiResult)
            {
                case ApiResult.Ok:
                    DependencyService.Get<IMobileSettings>().MaskCode = mask;

                    if (DependencyService.Get<IDemoSettings>().IsDemo)
                    {
                        DependencyService.Get<IDemoSettings>().DemoPin = maskedPin;
                        DependencyService.Get<IDemoSettings>().Save();
                    }

                    await NavigationService.ShowMessagePage(MessagePageType.ChangePinSuccess);
                    break;
                case ApiResult.Nok:
                    {
                        switch (changePinResult.ErrorMessage)
                        {
                            case "no_app_found":
                            case "wrong_session":
                            case "failed_decoding_pin":
                                await NavigationService.ShowMessagePage(MessagePageType.ChangePinFailed);
                                break;
                            case "too_many_changes_today":
                                await NavigationService.ShowMessagePage(MessagePageType.ChangePinMaxReached);
                                break;
                            default:
                                await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
                                break;
                        }
                        break;
                    }
                default:
                    await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
                    break;
            }

            DialogService.HideProgressDialog();
        }

        public AsyncCommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;

                    if (Model.EIDSessionResponse == null && Model.EntryType == PinEntryType.ChangePIN_PIN)
                    {
                        await NavigationService.PopToRoot(true);
                        return;
                    }

                    bool result;

                    if (DependencyService.Get<IMobileSettings>().ActivationStatus != ActivationStatus.Activated)
                    {
                        result = await DependencyService.Get<IAlertService>().DisplayAlert(
                        AppResources.ActivationAnnulerenAlertHeader
                        , AppResources.ActivationAnnulerenAlertMessage
                        , AppResources.Yes
                        , AppResources.No);
                    }
                    else
                    {
                        result = await DependencyService.Get<IAlertService>().DisplayAlert(
                            AppResources.LoginCancelConfirmHeader
                            , AppResources.LoginCancelConfirmMessage
                        , AppResources.Yes
                        , AppResources.No);
                    }

                    if (result)
                    {
                        if (Model.EIDSessionResponse != null)
                        {
                            await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                            await NavigationService.ShowMessagePage(MessagePageType.WIDCancelled);
                        }
                        else
                            await App.CancelSession(true, async () =>
                            {
                                switch (Model.EntryType)
                                {
                                    case PinEntryType.Authentication
                                        when DependencyService.Get<IMobileSettings>().ActivationStatus ==
                                             ActivationStatus.Activated:
                                        await NavigationService.ShowMessagePage(MessagePageType.LoginCancelled);
                                        break;
                                    case PinEntryType.Authentication:
                                    case PinEntryType.Enrollment:
                                        await NavigationService.ShowMessagePage(MessagePageType.ActivationCancelled);
                                        break;
                                    default:
                                        await NavigationService.PopToRoot();
                                        break;
                                }
                            });
                    }

                    CanExecute = true;
                }, () => CanExecute);
            }
        }

        private async Task<bool?> AuthenticateAsync(SecureString pin)
        {
            DialogService.ShowProgressDialog();

            var response = await DependencyService.Get<IAuthenticationService>().InitSessionAuthentication();

            if (response.ApiResult != ApiResult.Ok)
            {
                DialogService.HideProgressDialog();
                return false;
            }

            var challengeResponse = await AuthenticationHelper.GetChallenge(response.AppSessionId);

            if (challengeResponse == null)
            {
                DialogService.HideProgressDialog();
                return false;
            }

            var result = await AuthenticationHelper.AuthenticateAsync(pin, challengeResponse, response.AppSessionId);

            if (Model.ChangeAppPin)
                _iv = challengeResponse.IV;

            DialogService.HideProgressDialog();

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    DependencyService.Get<IMobileSettings>().LoginLevel = result.AuthenticationLevel.ToLoginLevel();
                    DependencyService.Get<IMobileSettings>().Save();

                    //this check is only needed for old installations where notification permission had never been asked
                    if (DependencyService.Get<IMobileSettings>().NotificationPermissionSet == null && DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated)
                        await NavigationService.ConfirmAsync(ConfirmActions.RequestPushNotificationPermission);

                    AppSession.AuthenticationSessionId = response.AppSessionId;

                    if (Model.IsAppAuthentication)
                        AppSession.SetAccountStatus(await DependencyService.Get<IAccountInformationServices>().GetAccountStatus());

                    if (!string.IsNullOrEmpty(App.NewNotificationToken))
                        await RemoteNotificationHelper.UpdateToken(App.NewNotificationToken);

                    if (AppSession.ManagementAction != null)
                    {
                        await AppSession.ManagementAction.Invoke();
                        AppSession.ManagementAction = null;
                    }
                    else if (Model.ChangeAppPin)
                    {
                        Model.EntryType = PinEntryType.ChangePIN_PIN;
                        IsPIN1Active = false;
                        IsPIN2Active = true;
                        ActivePinTile = 0;

                        SetLabels(AppResources.AppChangePinHeader, AppResources.AppChangePinEnterMessage);
                    }
                    else
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.LoginUnknown);
                        App.ClearSession();
                    }

                    DialogService.HideProgressDialog();

                    return true;
                case ApiResult.Cancelled:
                    await NavigationService.ShowMessagePage(MessagePageType.LoginCancelled);
                    DialogService.HideProgressDialog();
                    break;
                case ApiResult.Disabled:
                    await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                    DialogService.HideProgressDialog();
                    break;
                case ApiResult.Aborted:
                    DialogService.HideProgressDialog();
                    await NavigationService.ShowMessagePage(MessagePageType.LoginAborted);
                    break;
                case ApiResult.Blocked:
                    DialogService.HideProgressDialog();
                    HandleBlocked();
                    await NavigationService.ShowMessagePage(MessagePageType.AppBlocked);
                    break;
                case ApiResult.Nok:
                    DialogService.HideProgressDialog();
                    if (result.ErrorMessage == "classified_deceased")
                        await NavigationService.ShowMessagePage(MessagePageType.ClassifiedDeceased);
                    else
                        HandlePinError(result);
                    break;
                case ApiResult.Unknown:
                    DialogService.HideProgressDialog();
                    await NavigationService.ShowMessagePage(MessagePageType.LoginUnknown);
                    break;
                case ApiResult.SessionNotFound:
                    DialogService.HideProgressDialog();
                    await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                    break;
                case ApiResult.HostNotReachable:
                    DialogService.HideProgressDialog();
                    await NavigationService.ShowMessagePage(MessagePageType.NoInternetConnection);
                    break;
                case ApiResult.SSLPinningError:
                    DialogService.HideProgressDialog();
                    await NavigationService.ShowMessagePage(MessagePageType.SSLException);
                    break;
                case ApiResult.Failed:
                    DialogService.HideProgressDialog();
                    DependencyService.Get<IMobileSettings>().Reset();
                    await NavigationService.ShowMessagePage(MessagePageType.ReactivationNeeded);
                    break;
            }

            return null;
        }

        private void HandleBlocked()
        {
            ActivePinTile = 0;
            OnError();

            App.Reset();
        }

        private void HandlePinError(ValidatePinResponse result)
        {
            ActivePinTile = 0;

            if (result.RemainingAttempts == 1)
            {
                SetLabels(AppResources.WrongPinHeader, AppResources.WrongPinMessageLastTry, true);
                PiwikHelper.TrackView("AP210", GetType().FullName);
            }
            else if (result.RemainingAttempts == -1)
                SetLabels(AppResources.UnknownErrorHeader, AppResources.UnknownErrorMessage, true);
            else
                SetLabels(AppResources.WrongPinHeader, string.Format(CultureInfo.InvariantCulture, AppResources.WrongPinMessageTriesLeft, result.RemainingAttempts), true);

            PiwikHelper.TrackView("AP209", GetType().FullName);

            OnError();
        }

        private async Task PinsAreEqual()
        {
            await Task.Delay(100); //ensures last "*" is shown before display alert is opened
            await ActivationHelper.ActivateAsync(Pin1, Model.IV);

            Pin1 = new SecureString();
            Pin2 = new SecureString();
        }

        private async Task ValidatePinCode()
        {
            switch (Model.EntryType)
            {
                //New pincode to validate
                case PinEntryType.Enrollment when !IsPIN2Active:
                    return;
                case PinEntryType.Enrollment when Pin1.ToPlain() == Pin2.ToPlain(): //The two codes are equal
                    await PinsAreEqual();
                    break;
                case PinEntryType.Enrollment: // The two codes do are not equal
                    OnError();

                    Pin1 = new SecureString();
                    Pin2 = new SecureString();

                    SetLabels(AppResources.ActivationHeader, AppResources.PincodeMatchErrorMessage, true);
                    ActivePinTile = 0;
                    IsPIN1Active = true;
                    IsPIN2Active = false;
                    break;
                case PinEntryType.Authentication:
                    {
                        await Task.Delay(100); //ensures last "*" is shown before display alert is opened

                        if (Model.EIDSessionResponse != null)
                        {
                            var model = new NfcScannerModel
                            {
                                Action = Model.EntryType,
                                PIN = Pin1,
                                EIDSessionResponse = Model.EIDSessionResponse,
                                NewPIN = null,
                                IsStatusChecked = Model.IsStatusChecked,
                                Card = Model.Card,
                                IsActivation = Model.IsWIDActivation
                            };

                            await NavigationService.PushAsync(new WidScannerViewModel(model));
                            return;
                        }

                        var isValid = await AuthenticateAsync(Pin1);
                        if (isValid.HasValue && !isValid.Value)
                            HandlePinError(new ValidatePinResponse { RemainingAttempts = -1 });

                        Pin1 = new SecureString();
                        break;
                    }
            }
        }
    }
}
