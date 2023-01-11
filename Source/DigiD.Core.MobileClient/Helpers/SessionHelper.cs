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
using System.Threading.Tasks;
using DigiD.Common.BaseClasses;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.ViewModels;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    internal static class SessionHelper
    {
        internal static async Task StartSession(Func<Task> nextAction)
        {
            DependencyService.Get<IDialog>().ShowProgressDialog();

            var status = await DependencyService.Get<IAuthenticationService>().SessionStatus();

            switch (status.ApiResult)
            {
                case ApiResult.Ok:
                    await nextAction.Invoke();
                    break;
                case ApiResult.SessionNotFound:
                case ApiResult.Nok when status.ErrorMessage == "no_session":
                    {
                        AppSession.ManagementAction = nextAction;

                        var model = new PinCodeModel(PinEntryType.Authentication)
                        {
                            IsAppAuthentication = true
                        };

                        if (Application.Current.MainPage is CustomNavigationPage cnp &&
                            cnp.CurrentPage.BindingContext is PinCodeViewModel)
                        {
                            //user is already on pincode view, so view change is not needed
                        }
                        else
                            DependencyService.Get<INavigationService>().SetPage(new PinCodeViewModel(model));
                        break;
                    }
                default:
                    AppSession.AuthenticationSessionId = null;
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.UnknownError);
                    break;
            }

            DependencyService.Get<IDialog>().HideProgressDialog();
        }
    }
}
