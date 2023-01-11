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
﻿using System.Text.RegularExpressions;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class EmailRegisterViewModel : BaseActivationViewModel
    {
        private readonly RegisterEmailModel _model;

        public IAsyncCommand ButtonSkipCommand => new AsyncCommand(async () =>
        {
            var response = await DependencyService.Get<IAlertService>().DisplayAlert(AppResources.M12_Title,
                AppResources.M12_Message, AppResources.Yes, AppResources.No);

            if (response)
            {
                await DependencyService.Get<IEmailService>().Register(null);

                if (_model.AbortAction != null)
                    await _model.AbortAction.Invoke();
                else
                    await _model.NextAction.Invoke();
            }
        });

        public string EmailAddress { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(EmailAddress) && Regex.IsMatch(EmailAddress,
            "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])");

#if A11YTEST
        public EmailRegisterViewModel() : this(new RegisterEmailModel(ActivationMethod.SMS, true )) { }
#endif

        public EmailRegisterViewModel(RegisterEmailModel model)
        {
            _model = model;
            PageId = "AP080";
            FooterText = AppResources.AP080_Message;
            HeaderText = AppResources.AP080_Header;

            ButtonCommand = new AsyncCommand(async () =>
            {
                CanExecute = false;
                
                DialogService.ShowProgressDialog();
                var result = await DependencyService.Get<IEmailService>().Register(EmailAddress);

                switch (result.ApiResult)
                {
                    case ApiResult.Ok:
                        await NavigationService.PushAsync(new EmailConfirmViewModel(_model, result.EmailAddress, true));
                        break;
                    case ApiResult.Nok:
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

                        break;
                }
                
                DialogService.HideProgressDialog();

                CanExecute = true;
            },() => CanExecute);
        }
    }
}
