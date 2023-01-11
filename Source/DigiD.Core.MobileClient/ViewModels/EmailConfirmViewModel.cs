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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Interfaces;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class EmailConfirmViewModel : BaseActivationViewModel
    {
        public string ActivationCode { get; set; }
        private readonly DelegateWeakEventManager _hideKeyboardEventManager = new DelegateWeakEventManager();
        public event EventHandler HideKeyboard
        {
            add => _hideKeyboardEventManager.AddEventHandler(value);
            remove => _hideKeyboardEventManager.RemoveEventHandler(value);
        }

        private readonly RegisterEmailModel _model;
        private readonly string _emailAddress;
        private readonly IAsyncCommand _validateCommand;
        private const int InputLength = 9;
        private readonly string _baseMessageText;

        private const string CHARACTERS = "BCDFGHJKLMNPQRSTVWXZ23456789";
        private bool _fromResult;
        
        public IAsyncCommand ButtonSkipCommand => new AsyncCommand(async () =>
        {
            var response = await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.M13_Title,
                AppResources.M13_Message, AppResources.Yes, AppResources.No);

            if (response)
            {
                if (_model.AbortAction != null)
                    await _model.AbortAction.Invoke();
                else
                    await _model.NextAction.Invoke();
            }

        });

#if A11YTEST
        public EmailConfirmViewModel() : this(new RegisterEmailModel(ActivationMethod.SMS, true), "mail@server.com") { }
#endif

        public EmailConfirmViewModel(RegisterEmailModel model, string emailAddress, bool canGoBack)
        {
            _model = model;
            _emailAddress = emailAddress;
            HasBackButton = canGoBack;
            PageId = model.ExistingEmailAddress ? "AP108" : "AP081";
            HeaderText = model.ExistingEmailAddress ? AppResources.AP108_Header : AppResources.AP081_Header;
            ButtonText = model.ExistingEmailAddress ? AppResources.AP108_Button2 : AppResources.AP081_Button2;
            _baseMessageText =string.Format(CultureInfo.InvariantCulture, model.ExistingEmailAddress ? AppResources.AP108_Message : AppResources.AP081_Message, emailAddress);
            FooterText = _baseMessageText;

            ButtonCommand = new AsyncCommand(async () =>
            {
                CanExecute = false;
                var result = await DependencyService.Get<IEmailService>().Register(emailAddress);

                if (result.ApiResult == ApiResult.Ok)
                {
                    PiwikHelper.Track("email_bevestigd", "ja", "AP081");
                    await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.M10_Title, AppResources.M10_Message, AppResources.OK);
                    ActivationCode = string.Empty;
                    IsError = false;
                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                        await Task.Delay(1000);
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.AP081_Message, emailAddress).ChangeForA11y();
                }
                else if (result.ApiResult == ApiResult.Nok)
                {
                    switch (result.ErrorMessage)
                    {
                        case "too_many_emails":
                            await NavigationService.ShowMessagePage(MessagePageType.EmailRegistrationTooManyMails, _model);
                            break;
                        case "email_address_maximum_reached":
                            await NavigationService.ShowMessagePage(MessagePageType.EmailRegistrationMaximumReached, _model);
                            break;
                        case "email_already_verified":
                            await NavigationService.ShowMessagePage(MessagePageType.EmailRegistrationAlreadyVerified, _model);
                            break;
                    }
                }

                CanExecute = true;
            },() => CanExecute);

            _validateCommand = new AsyncCommand(async () =>
            {
                CanExecute = false;
                _hideKeyboardEventManager.RaiseEvent(this, EventArgs.Empty, nameof(HideKeyboard));
                await ValidateCode();
                CanExecute = true;
            }, () => CanExecute);
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        private async Task ValidateCode()
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 17 to the 15 allowed.
        {
            DialogService.ShowProgressDialog();
            var result = await DependencyService.Get<IEmailService>().Verify(ActivationCode.ToUpper());
            DialogService.HideProgressDialog();

            if (result.ApiResult == ApiResult.Ok)
            {
                NextCommand = new AsyncCommand(async () =>
                {
                    IsNextCommandExecuted = true;
                    await _model.NextAction.Invoke();
                }, () => !IsNextCommandExecuted);

                SetNavbar(false);
                IsBlurVisible = true;
            }
            else
            {
                switch (result.ErrorMessage)
                {
                    case "code_incorrect":
                        if (result.RemainingAttempts == 0)
                        {
                            await NavigationService.ShowMessagePage(MessagePageType.EmailRegistrationCodeBlocked, _model);
                            return;
                        }
                        
                        _fromResult = true;
                        ActivationCode = string.Empty;
                        IsError = true;
                        if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                            await Task.Delay(1000);
                        FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.AP081a_Message, result.RemainingAttempts).ChangeForA11y();
                        _fromResult = false;
                        break;
                    case "code_blocked":
                        await NavigationService.ShowMessagePage(MessagePageType.EmailRegistrationCodeBlocked, _model);
                        break;
                    case "code_invalid":
                        _fromResult = true;
                        ActivationCode = string.Empty;
                        IsError = true;
                        if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                            await Task.Delay(1000);
                        FooterText = AppResources.AP081b_Message.ChangeForA11y();
                        _fromResult = false;
                        break;
                }
            }
        }

#pragma warning disable S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 16 to the 15 allowed.
        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
#pragma warning restore S3776 //Code Smell: Refactor this method to reduce its Cognitive Complexity from 16 to the 15 allowed.
        {
            base.OnPropertyChanged(propertyName);

            if (_fromResult)
                return;

            if (propertyName == nameof(ActivationCode))
            {
                if (!ActivationCode.ToUpper().StartsWith("E"))
                {
                    ActivationCode = string.Empty;
                    if (DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                    {
                        await Task.Delay(1000);
                        FooterText = "";
                    }
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.SingleDeviceActivationcodeWrongFormat, "E");
                    IsError = true;
                    return;
                }

                if (string.IsNullOrEmpty(ActivationCode))
                    return;

                FooterText = _baseMessageText;
                IsError = false;
                
                var lastChar = ActivationCode.ToCharArray().Last();

                if (!CHARACTERS.Contains(lastChar.ToString().ToUpper()) && ActivationCode.Length > 1)
                {
                    ActivationCode = ActivationCode.Substring(0, ActivationCode.Length - 1);
                    return;
                }

                if (ActivationCode.Length == InputLength && _validateCommand.CanExecute(null))
                {
                    await _validateCommand.ExecuteAsync();
                }
                else
                {
                    FooterText = string.Format(CultureInfo.InvariantCulture,AppResources.AP081_Message, _emailAddress);
                    IsError = false;
                }
            }
        }
    }
}
