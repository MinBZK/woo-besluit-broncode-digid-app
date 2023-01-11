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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class ActivationSmsViewModel : BaseActivationViewModel
    {
        public string SMSCode { get; set; }

        public string AccessibilityMessageText
        {
            get => _accessibilityMessageText;
            set => _accessibilityMessageText = string.Concat(value, IsError ? $", {AppResources.AccessibilityTryAgain} " : string.Empty);
        }

        public string MessageText { get; set; }
        public ButtonType ButtonType { get; set; }
        private readonly DelegateWeakEventManager _hideKeyboardEventManager = new DelegateWeakEventManager();

        public event EventHandler HideKeyboard
        {
            add => _hideKeyboardEventManager.AddEventHandler(value);
            remove => _hideKeyboardEventManager.RemoveEventHandler(value);
        }

        public bool ButtonEnabled => !string.IsNullOrEmpty(SMSCode) && SMSCode.Length == _inputLength;

        private readonly string _baseMessageText;
        private readonly int _inputLength;
        private bool _init;
        private bool _fromResult;
        private RequestSmsResponse _response;
        private string _accessibilityMessageText;

        public ActivationSmsViewModel()
        {
            HeaderText = AppResources.ActivationHeader;

            PageId = "AP044";
            _baseMessageText = AppResources.SingleDeviceSMSMessage;
            MessageText = _baseMessageText;
            AccessibilityMessageText = _baseMessageText;
            ButtonText = AppResources.SingleDeviceSMSButton;
            ButtonType = ButtonType.Secundairy;

            _validateCommand = new AsyncCommand(async () =>
            {
                await VerifySMSCode();
            }, () => !IsError);
            ButtonCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PushAsync(new NoSmsViewModel(_response.PhoneNumber));
            });
            _inputLength = 6;

            NavCloseCommand = CancelCommand;
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            CanExecute = true;

            if (_init)
                return;

            await DependencyService.Get<IA11YService>().Speak(_accessibilityMessageText);

            _init = true;
            await RequestSMS();
        }

        private async Task RequestSMS()
        {
            _response = await DependencyService.Get<IEnrollmentServices>().SendSMS();

            if (_response.ApiResult == ApiResult.Nok)
            {
                IsError = true;

                switch (_response.ErrorMessage)
                {
                    case "sms_too_fast":
                        var seconds = (int)_response.Payload.time_left;
                        while (seconds > 0)
                        {
                            MessageText = string.Format(CultureInfo.InvariantCulture, AppResources.SingleDeviceSMSTooFast, seconds);
                            await Task.Delay(1000);
                            seconds--;
                            if (seconds % 5 == 0)
                                AccessibilityMessageText = MessageText;
                        }

                        await RequestSMS();
                        break;
                }
            }
            else if (_response.ApiResult == ApiResult.Ok)
            {
                MessageText = _baseMessageText;
                IsError = false;
                CanExecute = true;
            }
        }

        private readonly ICommand _validateCommand;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (_fromResult)
                return;

            if (propertyName == nameof(SMSCode))
            {
                if (SMSCode.Length == _inputLength && _validateCommand.CanExecute(null))
                {
                    _validateCommand.Execute(null);
                }
                else
                {
                    MessageText = _baseMessageText;
                    IsError = false;
                }
            }
        }

        private async Task HandleOK(SessionResponse result)
        {
            _hideKeyboardEventManager.RaiseEvent(this, EventArgs.Empty, nameof(HideKeyboard));
            IsError = false;
            var activationResult = await ActivationHelper.StartChallenge(result.AppId);

            if (activationResult.ApiResult == ApiResult.Ok)
            {
                if (HttpSession.ActivationSessionData.ActivationMethod == ActivationMethod.SMS)
                {
                    NextCommand = new AsyncCommand(async () =>
                    {
                        IsNextCommandExecuted = true;
                        if (HttpSession.ActivationSessionData.Pin == null)
                            await NavigationService.PushAsync(new PinCodeViewModel(new Common.Models.PinCodeModel(PinEntryType.Enrollment) {IV = activationResult.IV }));
                        else
                            await ActivationHelper.ActivateAsync(HttpSession.ActivationSessionData.Pin, activationResult.IV, true);
                    }, () => !IsNextCommandExecuted);
                }
                else //AccountAndApp activation
                {
                    NextCommand = new AsyncCommand(async () =>
                    {
                        IsNextCommandExecuted = true;
                        await NavigationService.PushAsync(new ActivationLetterViewModel(false, activationResult.IV));

                    },() => !IsNextCommandExecuted);
                }

                SetNavbar(false);
                IsBlurVisible = true;
            }
            else
                await NavigationService.ShowMessagePage(activationResult.MessagePageType);
        }

        private async Task HandleNOK(SessionResponse result)
        {
            _fromResult = true;
            IsError = false;
            SMSCode = string.Empty;
            _fromResult = false;

            switch (result.ErrorMessage)
            {
                case "smscode_incorrect":
                    IsError = true;
                    MessageText = AppResources.SingleDeviceSMSIncorrect;
                    AccessibilityMessageText = MessageText;
                    await DependencyService.Get<IA11YService>().Speak(AccessibilityMessageText, 1000);
                    break;
                case "smscode_invalid":
                    IsError = true;
                    MessageText = AppResources.SingleDeviceSMSInvalid;
                    AccessibilityMessageText = MessageText;
                    await DependencyService.Get<IA11YService>().Speak(AccessibilityMessageText, 1000);
                    break;
                case "smscode_blocked":
                    await NavigationService.ShowMessagePage(MessagePageType.ActivateSMSAuthenticationBlocked);
                    break;
                case "smscode_not_send":
                    MessageText = AppResources.SingleDeviceSMSNotSend;
                    AccessibilityMessageText = MessageText;
                    break;
                case "smscode_expired":
                    MessageText = AppResources.SingleDeviceSMSExpired;
                    AccessibilityMessageText = MessageText;
                    break;
                default:
                    MessageText = $"Response '{result.ErrorMessage}' not specified";
                    break;
            }
        }

        private async Task VerifySMSCode()
        {
            DialogService.ShowProgressDialog();

            var result = await ActivationHelper.InitSession(false, SMSCode);

            switch (result.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        await HandleOK(result);
                        break;
                    }
                case ApiResult.Nok:
                    {
                        await HandleNOK(result);
                        break;
                    }
                case ApiResult.SessionNotFound:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                        break;
                    }
                case ApiResult.Disabled:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                        break;
                    }
                case ApiResult.Unknown:
                    {
                        await NavigationService.ShowMessagePage(MessagePageType.UnknownError);
                        break;
                    }
                case ApiResult.HostNotReachable:
                    {
                        IsError = true;
                        MessageText = AppResources.NoInternetConnectionMessage;
                        AccessibilityMessageText = MessageText;
                        break;
                    }
            }

            DialogService.HideProgressDialog();
        }

        public ICommand CancelCommand
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    if (!CanExecute)
                        return;

                    CanExecute = false;

                    var stop = await DependencyService.Get<IAlertService>().DisplayAlert(
                        AppResources.ActivationAnnulerenAlertHeader
                        , AppResources.ActivationAnnulerenAlertMessage
                        , AppResources.Yes
                        , AppResources.No);

                    if (stop)
                    {

                        if (HttpSession.TempSessionData != null && HttpSession.IsApp2AppSession)
                        {
                            await App2AppHelper.AbortSession("cancelled", false);

                        }
                        await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.ActivationCancelled));
                    }

                    CanExecute = true;
                }, () => CanExecute);
            }
        }
    }
}
