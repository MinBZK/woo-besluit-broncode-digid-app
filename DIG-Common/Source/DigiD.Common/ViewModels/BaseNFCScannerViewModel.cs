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
﻿using DigiD.Common.BaseClasses;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common.EID.BaseClasses;
using DigiD.Common.EID.CardOperations;
using DigiD.Common.EID.Demo;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Exceptions;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Exceptions;
using DigiD.Common.NFC.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.Common.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class BaseNfcScannerViewModel : CommonBaseViewModel
    {
        private bool _isTagLost;
        private readonly IEIDOperationService _eidOperationService = DependencyService.Get<IEIDOperationService>();

        public NfcScannerModel Model { get; }
        public bool IsScanning { get; set; }
        public float ProgressValue { get; set; }
        public string AnimationSource { get; set; }
        public bool ProgressVisible { get; set; }
        public string ImageSource { get; set; }
        public bool AnimationVisible { get; set; }

        protected static string DocumentName => EIDSession.Card != null ? EIDSession.Card.DocumentType.Translate().ToLowerInvariant() : AppResources.eID.ToLowerInvariant();
        protected INfcService NfcService { get; } = DependencyService.Get<INfcService>();

        public AsyncCommand CancelCommand { get; protected set; }

        protected BaseNfcScannerViewModel(NfcScannerModel model)
        {
            HeaderText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDReadingDocument, DocumentName);
            Model = model;

            if (Device.Idiom == TargetIdiom.Desktop && model != null)
            {
                PageId = !model.IsStatusChecked ? "DA008" : "DA011";
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            DialogService.HideProgressDialog();
        }

        public async Task StopScanning(string message = null)
        {
            if (IsScanning)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await NfcService.StopScanningAsync(message);
                });
            }

            IsScanning = false;
        }

        public async Task Cancel()
        {
            NfcService.IsCancelled = true;
            await StopScanning(AppResources.Cancel);
        }

        public async Task Start()
        {
            ProgressVisible = true;
            IsScanning = true;
            _isTagLost = false;
            NfcService.IsCancelled = false;

            switch (Model.Action)
            {
                case PinEntryType.Authentication:
                    if (Model.IsStatusChecked)
                        await Randomize();
                    else
                        await StatusCheck(PasswordType.PIN);
                    break;
                case PinEntryType.ChangePIN_PIN:
                    if (Model.IsStatusChecked)
                        await ChangePin(PasswordType.PIN);
                    else
                        await StatusCheck(PasswordType.PIN);
                    break;
                case PinEntryType.ResumePIN_CAN:
                    if (Model.IsStatusChecked)
                        await ResumePin();
                    else
                        await StatusCheck(PasswordType.PIN);
                    break;
                case PinEntryType.ChangeTransportPIN:
                    await ChangePin(PasswordType.PIN);
                    break;
#if READ_PHOTO
                case PinEntryType.ReadPhoto:
                    await PerformReadPhoto();
                    break;
#endif
            }
        }

        public virtual Task RandomizationComplete()
        {
            return Task.CompletedTask;
        }

        public virtual Task ReadPhotoCompleted(byte[] photo)
        {
            return Task.CompletedTask;
        }

        private async Task HandleStatusOperation(ISecureCardOperation operation)
        {
            PinCodeModel model;

            if (operation.GAP.IsBlocked)
            {
                //wanneer PIN is geblokkeerd, controleer de PUK status
                if (operation.GAP.PasswordType == PasswordType.PIN)
                {
                    await StatusCheck(PasswordType.PUK);
                    return;
                }

                //wanneer PUK is geblokkeerd, toon melding aan gebruiker
                if (operation.GAP.PasswordType != PasswordType.PUK)
                    return;

                await DependencyService.Get<IEIDServices>().AbortAuthentication(new Models.RequestModels.AbortAuthenticationRequest(AbortConstants.WIDPUKBlocked));
                await NavigationService.ShowMessagePage(MessagePageType.WIDPUKBlocked);
                return;
            }

            if (operation.GAP.IsSuspended) //wanneer PIN/PUK tijdelijks is geblokkeerd, un-suspend met CAN
            {
                model = new PinCodeModel(PinEntryType.ResumePIN_CAN, operation.GAP.Card, Model.EIDSessionResponse);
            }
            else
            {
                switch (Model.Action)
                {
                    case PinEntryType.ChangePIN_PIN when operation.GAP.PasswordType == PasswordType.PUK:
                        await NavigationService.ShowMessagePage(MessagePageType.WIDPINBlocked);
                        return;
                    case PinEntryType.ChangePIN_PIN:
                        model = new PinCodeModel(Model.Action, operation.GAP.Card, triesLeft: operation.GAP.PinTriesLeft);
                        break;
                    default:
                        {
                            if (Model.EIDSessionResponse != null && operation.GAP.PasswordType == PasswordType.PUK)
                            {
                                await DependencyService.Get<IEIDServices>().AbortAuthentication(new Models.RequestModels.AbortAuthenticationRequest(AbortConstants.WIDPINBlocked));
                                await NavigationService.ShowMessagePage(MessagePageType.WIDPINBlocked);
                                return;
                            }

                            //in het geval het een authenticatie betreft, en de PIN status, redirect naar PIN invoer, met juiste aantal pogingen
                            model = new PinCodeModel(PinEntryType.Authentication, operation.GAP.Card, Model.EIDSessionResponse, operation.GAP.PinTriesLeft);
                            break;
                        }
                }
            }

            model.IsStatusChecked = true;
            model.IsWIDActivation = Model.IsActivation;

            if (model.EntryType == PinEntryType.ResumePIN_CAN)
                await NavigationService.PushAsync(new WidSuspendedViewModel(model));
            else
                await NavigationService.GoToPincodePage(model);
        }

        private async Task ExecuteOperation(BaseSecureCardOperation operation)
        {
            var message = string.Empty;

            try
            {
                var success = await operation.Execute();

                if (NfcService.IsCancelled)
                {
                    await DependencyService.Get<IEIDServices>().CancelAuthenticate();

                    if (Model.EIDSessionResponse != null)
                        await NavigationService.ShowMessagePage(MessagePageType.LoginAborted);
                    else
                        await NavigationService.PopToRoot();

                    return;
                }

                if (operation is StatusOperation || operation is DemoCardOperation && !Model.IsStatusChecked)
                {
                    await HandleStatusOperation(operation);
                }
                else if (success)
                {
                    switch (operation)
                    {
                        case ReadPhotoOperation rpo:
                            await ReadPhotoCompleted(rpo.Photo);
                            break;
                        case RandomizeOperation _:
                        case DemoCardOperation { OperationType: CardOperationType.Authentication }:
                            await HandleRandomizationCompletion();
                            break;
                        case ChangePinOperation _:
                        case DemoCardOperation { OperationType: CardOperationType.ChangePin }:
                            await HandleChangePinCompletion();
                            break;
                        case ResumeOperation _:
                        case DemoCardOperation { OperationType: CardOperationType.ResumePin }:
                            await HandlePinResumeCompletion();
                            break;
                    }
                }
                else
                {
                    if (_isTagLost)
                        return;

                    if (operation.GAP.Card.EF_Dir == null)
                    {
                        await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                        await NavigationService.ShowMessagePage(MessagePageType.WIDNotSupported);
                    }
                    else if (operation.GAP.IsBlocked)
                    {
                        await DependencyService.Get<IEIDServices>().AbortAuthentication(new Models.RequestModels.AbortAuthenticationRequest(AbortConstants.WIDPINBlocked));
                        await NavigationService.ShowMessagePage(MessagePageType.WIDPINBlocked);
                    }
                    else if (operation.GAP.IsSuspended && Model.EIDSessionResponse == null)
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.WIDSuspended);
                    }
                    else if (Model.IsActivation && operation.GAP.ChangePinRequired)
                    {
                        var model = new PinCodeModel(PinEntryType.ChangeTransportPIN, Model.Card, Model.EIDSessionResponse)
                        {
                            IsWIDActivation = true,
                            PIN = Model.PIN,
                            IsStatusChecked = Model.IsStatusChecked,
                            NeedPinChange = true
                        };

                        await NavigationService.GoToPincodePage(model);
                    }
                    else if (operation.GAP.ChangePinRequired)
                    {
                        await DependencyService.Get<IEIDServices>().AbortAuthentication(new Models.RequestModels.AbortAuthenticationRequest(AbortConstants.WIDChangePINFirstUse));
                        await NavigationService.ShowMessagePage(MessagePageType.WIDActivationNeeded);
                    }
                    else if (operation.GAP.AuthenticationResult == AuthenticationResult.Failed)
                    {
                        var model = new PinCodeModel(Model.Action, operation.GAP.Card, Model.EIDSessionResponse, operation.GAP.PinTriesLeft - 1)
                        {
                            WrongCAN = operation.GAP.PinTriesLeft == null && Model.Action == PinEntryType.ResumePIN_CAN 
                        };

                        if (operation.GAP.PinTriesLeft == 2)
                            model.EntryType = PinEntryType.ResumePIN_CAN;

                        model.IsStatusChecked = true;
                        model.IsWIDActivation = Model.IsActivation;

                        if (model.EntryType == PinEntryType.ResumePIN_CAN)
                            await NavigationService.PushAsync(new WidSuspendedViewModel(model));
                        else
                            await NavigationService.GoToPincodePage(model);
                    }
                    else if (Model.IsActivation && !operation.GAP.ChangePinRequired)
                    {
                        await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                        await NavigationService.ShowMessagePage(MessagePageType.WIDFailed);
                    }
                    else
                    {
                        switch (operation.GAP.ApiResult)
                        {
                            case ApiResult.Unknown:
                                await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                                await NavigationService.ShowMessagePage(MessagePageType.WIDFailed);
                                break;
                            case ApiResult.SessionNotFound:
                                await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                                break;
                            case ApiResult.BadRequest:
                                message = AppResources.WIDRandomizeFailedMessage;
                                await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                                await NavigationService.ShowMessagePage(MessagePageType.WIDRandomizeFailed);
                                break;
                            case ApiResult.Cancelled:
                                await NavigationService.ShowMessagePage(MessagePageType.WIDCancelled);
                                break;
                            default:
                                await NavigationService.ShowMessagePage(MessagePageType.WIDFailed);
                                break;
                        }
                    }
                }
            }
            catch (CardLostException)
            {
                _isTagLost = true;
            }
            catch (CardNotSupportedException)
            {
                await DependencyService.Get<IEIDServices>().CancelAuthenticate();
                await NavigationService.ShowMessagePage(MessagePageType.WIDNotSupported);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                if (!_isTagLost || Device.RuntimePlatform != Device.iOS)
                    await StopScanning(message);
            }

            if (_isTagLost)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    AnimationVisible = true;
                    ProgressVisible = false;
                    ProgressValue = 0;
                    DependencyService.Get<INfcService>().UpdateStatus(0);
                    await StartScan(true);
                });
            }
        }

        private async Task HandlePinResumeCompletion()
        {
            if (Model.EIDSessionResponse != null)
            {
                Model.PIN = Model.NewPIN;
                await Randomize();
            }
            else
                await NavigationService.ShowMessagePage(Model.Action == PinEntryType.ResumePIN_CAN ? MessagePageType.WIDResumePINSuccess : MessagePageType.WIDResumePUKSuccess);
        }

        private async Task HandleChangePinCompletion()
        {
            if (Model.EIDSessionResponse != null)
            {
                Model.PIN = Model.NewPIN;
                Model.NeedPinChange = false;
                Model.Action = PinEntryType.Authentication;

                await NavigationService.PushAsync(new WidChangeTransportPinConfirmViewModel(Model));
            }
            else
            {
                await NavigationService.ShowMessagePage(MessagePageType.WIDChangePINSuccess);
            }
        }

        private async Task HandleRandomizationCompletion()
        {
            await StopScanning();
            DependencyService.Get<IDialog>().ShowProgressDialog();

            //Poll naar kern om de status van de sessie te controleren
            var pollingResult = await DependencyService.Get<IEIDServices>().Poll();

            var tries = 0;

            // we expect completed as ApiResult when it's a session derived from a remote session (qr-code or desktop)
            while (pollingResult.ApiResult == ApiResult.Confirmed)
            {
                await Task.Delay(1000);

                pollingResult = await DependencyService.Get<IEIDServices>().Poll();

                tries++;

                if (tries > 10)
                    pollingResult.ApiResult = ApiResult.Timeout;
            }

            DependencyService.Get<IDialog>().HideProgressDialog();

            if (HttpSession.IsWeb2AppSession)
            {
                if (string.IsNullOrEmpty(pollingResult.ErrorMessage))
                    await RandomizationComplete();
                else
                    await NavigationService.ShowMessagePage(MessagePageType.WIDActivationNeeded);

                return;
            }

            if (Model.IsActivation)
            {
                await NavigationService.ShowMessagePage(pollingResult.ApiResult == ApiResult.Verified ? MessagePageType.WIDActivationSuccess : MessagePageType.WIDActivationFailed);
            }
            else if (Model.EIDSessionResponse.Action == AuthenticationActions.REACTIVATE_DRIVING_LICENSE || Model.EIDSessionResponse.Action == AuthenticationActions.REACTIVATE_IDENTITY_CARD)
            {
                await NavigationService.ShowMessagePage(pollingResult.ApiResult == ApiResult.Verified ? MessagePageType.WIDReActivationSuccess : MessagePageType.WIDReActivationFailed);
            }
            else
            {
                switch (pollingResult.ApiResult)
                {
                    case ApiResult.Verified:
                        {
                            switch (Model.EIDSessionResponse.Action)
                            {
                                case AuthenticationActions.DeactivateApp:
                                    await NavigationService.ShowMessagePage(MessagePageType.DeactivateAppSuccess);
                                    break;
                                case AuthenticationActions.ChangePassword:
                                    await NavigationService.ShowMessagePage(MessagePageType.PasswordChangeSuccess);
                                    break;
                                case AuthenticationActions.ChangePhoneNumber:
                                    await NavigationService.ShowMessagePage(MessagePageType.PhoneNumberChangeSuccess);
                                    break;
                                case AuthenticationActions.EMAIL_CHANGE:
                                    await NavigationService.ShowMessagePage(MessagePageType.EmailChange);
                                    break;
                                case AuthenticationActions.EMAIL_ADD:
                                    await NavigationService.ShowMessagePage(MessagePageType.EmailAdd);
                                    break;
                                case AuthenticationActions.EMAIL_REMOVE:
                                    await NavigationService.ShowMessagePage(MessagePageType.EmailRemove);
                                    break;
                                case AuthenticationActions.CANCEL_ACCOUNT:
                                    await NavigationService.ShowMessagePage(MessagePageType.AccountCancelledSuccess);
                                    break;
                                default:
                                    await NavigationService.ShowMessagePage(MessagePageType.LoginSuccess);
                                    break;
                            }
                            break;
                        }
                    case ApiResult.Aborted when pollingResult.ErrorMessage == "msc_issued":
                        await NavigationService.ShowMessagePage(MessagePageType.WIDActivationNeeded);
                        break;
                    case ApiResult.Cancelled:
                    case ApiResult.Aborted:
                        await NavigationService.ShowMessagePage(MessagePageType.WIDCancelled);
                        break;
                    case ApiResult.Timeout:
                        await NavigationService.ShowMessagePage(MessagePageType.WIDRandomizeFailed);
                        break;
                    default:
                        await NavigationService.ShowMessagePage(MessagePageType.WIDLoginFailed);
                        break;
                }
            }
        }

        private void ProgressChanged(float f)
        {
            Debug.WriteLine(f);
            ProgressValue = f;
            DependencyService.Get<INfcService>().UpdateStatus(f);
        }

        private async Task Randomize()
        {
            var credentials = new CardCredentials(Model.PIN, null, null);
            var operation = _eidOperationService.StartRandomize(UserConsent.PIP, Model.EIDSessionResponse.SessionId, Model.EIDSessionResponse.Host, credentials, ProgressChanged);
            FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDReadingDocument, DocumentName);
            await ExecuteOperation(operation);
        }

        private async Task ChangePin(PasswordType type)
        {
            var credentials = new CardCredentials(Model.PIN, null, Model.PIN);
            var operation = _eidOperationService.StartChangePin(Model.NewPIN, credentials, type, ProgressChanged);
            FooterText = AppResources.WIDDoingChangePIN;
            await ExecuteOperation(operation);
        }

        private async Task ResumePin()
        {
            var credentials = new CardCredentials(Model.NewPIN, Model.PIN, Model.NewPIN);
            var operation = _eidOperationService.StartResumePin(Model.Action == PinEntryType.ResumePIN_CAN ? GAPType.PINResumeCAN : GAPType.PUKResumeCAN, credentials, ProgressChanged);
            await ExecuteOperation(operation);
        }

#if READ_PHOTO
        private async Task PerformReadPhoto()
        {
            _cardCredentials = new CardCredentials(null, null, null, Model.PIN);
            var operation = new ReadPhotoOperation(_cardCredentials, GAPType.Bac, PasswordType.MRZ, ProgressChanged);
            await ExecuteOperation(operation);
        }
#endif

        private async Task StatusCheck(PasswordType type)
        {
            var operation = _eidOperationService.StartStatus(type, ProgressChanged);
            FooterText = AppResources.WIDCheckStatus;
            await ExecuteOperation(operation);
        }

        public async Task StartScan(bool retry)
        {
            IsScanning = true;
            _isTagLost = false;
            NfcService.IsCancelled = false;

            var success = false;

            var idiom = Device.Idiom == TargetIdiom.Tablet ? "tablet" : DependencyService.Get<IDevice>().DeviceTypeName;

            HeaderText = string.Format(CultureInfo.InvariantCulture, !Model.IsStatusChecked ? AppResources.WIDReadingDocument : AppResources.WIDReadingDocumentAgain, DocumentName);
            var txt = Device.RuntimePlatform != Device.iOS ? AppResources.RDAStartScanning : AppResources.RDAStartScanning_iOS;

            if (!retry)
                FooterText = Device.Idiom == TargetIdiom.Desktop ? AppResources.WIDPutCardOnReader
                    : string.Format(CultureInfo.InvariantCulture, txt, DocumentName, idiom);
            else
                FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.RDAReStartScanning, DocumentName, idiom);

            if (await NfcService.IsTagConnected() && !retry && !_isTagLost)
            {
                await StartReading();
                return;
            }

            while (!success && IsScanning)
            {
                success = await NfcService.StartScanningAsync(retry, StartReading, HandleError);
                await Task.Delay(1000);
            }
        }

        private static Task HandleError(NfcError e)
        {
            return Task.CompletedTask;
        }

        private async Task StartReading()
        {
            AnimationVisible = false;
            FooterText = string.Format(CultureInfo.InvariantCulture, AppResources.WIDReadingDocument, DocumentName);
            await Start();
        }
    }
}
