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
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class NoLetterReceivedViewModel : BaseViewModel
    {
        public bool IsValid { get; set; }
        public bool ReRequestLetterAllowed { get; }

#if A11YTEST
        public NoLetterReceivedViewModel() : this(new AuthenticateChallengeResponse () { IV = "1234567890" } ) { }
#endif

        public NoLetterReceivedViewModel()
        {
            PageId = "AP048";

            ReRequestLetterAllowed = DependencyService.Get<IPreferences>().LetterReRequestAllowed;

            HeaderText = AppResources.NoLetterReceivedHeader;
            FooterText = ReRequestLetterAllowed ? AppResources.NoLetterReceivedMessage : AppResources.NoLetterReceivedNewAccountMessage;
            ButtonText = ReRequestLetterAllowed ? AppResources.NoLetterRequestCommand : AppResources.NoLetterRequestAccountCommand;

            var hours = (DependencyService.Get<IPreferences>().LetterRequestDate
                .AddDays(App.Configuration.LetterRequestDelay).ToLocalTime() - DateTime.UtcNow).TotalHours;

            hours = Math.Ceiling(hours);

            IsValid = hours <= 0;

            if (!IsValid)
            {
                var forbiddenMessage = ReRequestLetterAllowed ? AppResources.AP048_RequestNewLetterForbiddenMessage : AppResources.AP048_RequestNewLetterAccountForbiddenMessage;
                FooterText += string.Format(forbiddenMessage, hours);
            }

            ButtonCommand = new AsyncCommand(async () =>
            {
                if (!CanExecute)
                    return;

                CanExecute = false;

                if (ReRequestLetterAllowed)
                {
                    var response = await DependencyService.Get<IEnrollmentServices>().InitSessionActivationCode(new ActivationCodeSessionRequest
                    {
                        AppId = DependencyService.Get<IMobileSettings>().AppId,
                        RequestNewLetter = true
                    });

                    switch (response.ApiResult)
                    {
                        case ApiResult.Ok:
                            HttpSession.AppSessionId = response.SessionId;
                            await NavigationService.PushAsync(new ActivationLetterViewModel(true));
                            break;
                        default:
                            await NavigationService.ShowMessagePage(MessagePageType.ActivationFailed);
                            break;
                    }
                }
                else
                {
                    await DeactivationHelper.DeactivateApp();
                    await NavigationService.PushAsync(new AP079ViewModel());
                }

                CanExecute = true;
            }, () => IsValid && CanExecute);

            NavCloseCommand = new AsyncCommand(async () =>
            {
                await NavigationService.PopToRoot(true);
            });
        }
    }
}
