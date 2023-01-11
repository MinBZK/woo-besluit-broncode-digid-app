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
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using DigiD.Services;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    internal class ActivationLetterViewModel : BaseActivationViewModel
    {
        public string ActivationCode { get; set; }
        public string MessageText { get; set; }
        public ButtonType ButtonType { get; set; }

        private readonly DelegateWeakEventManager _hideKeyboardEventManager = new DelegateWeakEventManager();

        public event EventHandler HideKeyboard
        {
            add => _hideKeyboardEventManager.AddEventHandler(value);
            remove => _hideKeyboardEventManager.RemoveEventHandler(value);
        }

        public bool ActivationCodeEntryVisible => !_reRequest;

        private readonly string _baseMessageText;
        private readonly int _inputLength;
        private readonly bool _reRequest;
        private readonly string _iv;
        private bool _fromResult;
        private readonly ICommand _validateCommand;

#if A11YTEST
        public ActivationLetterViewModel() : this(true) { }
#endif

        public ActivationLetterViewModel(bool reRequest, string iv = null)
        {
            _reRequest = reRequest;
            _iv = iv;

            HeaderText = AppResources.ActivationHeader;

            if (_reRequest)
            {
                PageId = "AP050";
                _baseMessageText = AppResources.SingleDeviceLetterReRequestMessage;
                MessageText = _baseMessageText;
                ButtonText = AppResources.Next;

                ButtonCommand = new AsyncCommand(async () =>
                {
                    DialogService.ShowProgressDialog();
                    var result = await ActivationHelper.RequestLetter(null);

                    if (result.ApiResult == ApiResult.Ok)
                        DependencyService.Get<IPreferences>().LetterReRequestAllowed = false;

                    await NavigationService.ShowMessagePage(result.ApiResult == ApiResult.Ok
                        ? MessagePageType.ReRequestLetterSuccess
                        : result.MessagePageType);

                    DialogService.HideProgressDialog();
                });
            }
            else
            {
                PageId = "AP045";
                _baseMessageText = AppResources.SingleDeviceLetterConfirmMessage;

                MessageText = _baseMessageText;

                _validateCommand = new AsyncCommand(async () =>
                {
                    await VerifyLetterActivationCode();
                }, () => !IsError);
                _inputLength = 9;
            }
            NavCloseCommand = CancelCommand;
        }
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (_fromResult)
                return;

            IsError = false;

            if (propertyName == nameof(ActivationCode))
            {
                if (string.IsNullOrEmpty(ActivationCode))
                {
                    MessageText = _baseMessageText;
                    IsError = false;
                }
                else if (!ActivationCode.StartsWith("A", StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageText = string.Format(CultureInfo.InvariantCulture, AppResources.SingleDeviceActivationcodeWrongFormat, "A");
                    IsError = true;
                }
                else if (ActivationCode.Length == _inputLength && _validateCommand.CanExecute(null))
                {
                    _validateCommand.Execute(null);
                }
            }
        }

        private async Task VerifyLetterActivationCode()
        {
            DialogService.ShowProgressDialog();

            var request = new LetterActivationRequest
            {
                ActivationCode = ActivationCode.ToUpper()
            };

            var response = HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.AccountAndApp
                    ? await DependencyService.Get<IEnrollmentServices>().ActivateAccountByApp(request)
                    : await DependencyService.Get<IEnrollmentServices>().CompleteLetterActivation(request);

            DialogService.HideProgressDialog();

            switch (response.ApiResult)
            {
                case ApiResult.Ok:
                    await HandleOk();
                    break;
                case ApiResult.Nok:
                    await HandleNok(response);
                    break;
                case ApiResult.Blocked:
                    DependencyService.Get<IMobileSettings>().Reset();
                    await NavigationService.ShowMessagePage(MessagePageType.ActivationLetterBlocked); // 3x foutieve activeringscode
                    break;
                case ApiResult.SessionNotFound:
                    await NavigationService.ShowMessagePage(MessagePageType.SessionTimeout);
                    break;
                case ApiResult.Disabled:
                    await NavigationService.ShowMessagePage(MessagePageType.ActivationDisabled);
                    break;
                case ApiResult.HostNotReachable:
                    {
                        IsError = true;
                        MessageText = AppResources.NoInternetConnectionMessage;
                        break;
                    }
                default:
                    await NavigationService.ShowMessagePage(MessagePageType.LoginUnknown);
                    break;
            }
        }

        private async Task HandleNok(ActivationLetterResponse response)
        {
            var errorMessage = response.ErrorMessage;
            switch (errorMessage)
            {
                case "invalid":
                case "activation_code_not_correct":
                    _fromResult = true;
                    IsError = true;
                    ActivationCode = string.Empty;
                    _fromResult = false;

                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                        await Task.Delay(2000);  // Kleine pauze zodat de foutmelding volledig wordt voorgelezen

                    var message = response.RemainingAttempts == 1
                        ? AppResources.SingleDeviceLetterActivationIncorrectLast
                        : AppResources.SingleDeviceLetterActivationIncorrect;

                    MessageText = string.Format(message, response.DateLetterSent.ToString("dd-MM"), response.RemainingAttempts);
                    break;
                case "expired":
                case "activation_code_invalid":
                    DependencyService.Get<IMobileSettings>().Reset();
                    if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.AccountAndApp)
                        await NavigationService.ShowMessagePage(MessagePageType.InAppActivationCodeExpired);
                    else
                        await NavigationService.ShowMessagePage(MessagePageType.ActivationLetterExpired);
                    break;
                case "activation_code_blocked":
                    DependencyService.Get<IMobileSettings>().Reset();
                    await NavigationService.ShowMessagePage(MessagePageType.InAppActivationCodeBlocked);
                    break;
            }
        }

        private async Task HandleOk()
        {
            _hideKeyboardEventManager.RaiseEvent(this, EventArgs.Empty, nameof(HideKeyboard));
            IsError = false;

            var settings = DependencyService.Get<IMobileSettings>();
            if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.AccountAndApp)
            {
                await NavigationService.PushAsync(new PinCodeViewModel(new Common.Models.PinCodeModel(PinEntryType.Enrollment)
                {
                    IV = _iv
                }));
            }
            else
            {
                settings.ActivationStatus = ActivationStatus.Activated;
                settings.Save();

                LetterNotificationHelper.ResetNotifications();
                Xamarin.Essentials.Preferences.Remove(nameof(Preferences.LetterRequestDate));

                await ActivationHelper.AskPushNotificationPermissionAsync(ActivationMethod.App, App.HasNfc && settings.LoginLevel == LoginLevel.Midden);
            }
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
                    var stop = await DependencyService.Get<IAlertService>().DisplayAlert(
                        AppResources.ActivationAnnulerenAlertHeader
                        , AppResources.ActivationAnnulerenAlertMessage
                        , AppResources.Yes
                        , AppResources.No);

                    if (stop)
                    {
                        if (HttpSession.TempSessionData != null && HttpSession.IsApp2AppSession)
                            await App2AppHelper.AbortSession("cancelled", false);

                        await App.CancelSession(true, async () => await NavigationService.ShowMessagePage(MessagePageType.ActivationCancelled));
                    }

                    CanExecute = true;
                }, () => CanExecute);
            }
        }
    }
}
