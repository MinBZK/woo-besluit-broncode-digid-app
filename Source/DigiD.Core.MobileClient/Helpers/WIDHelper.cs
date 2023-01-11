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
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.SessionModels;
using DigiD.ViewModels;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    internal static class WidHelper
    {
        internal static async Task AuthenticateAsync(SessionData sessionData)
        {
            if (!App.HasNfc)
            {
                await DependencyService.Get<IEIDServices>().AbortAuthentication(new AbortAuthenticationRequest(AbortConstants.NoNFC));
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoNFCSupport);

                return;
            }

            var code = string.Empty;

            if (!string.IsNullOrEmpty(sessionData.verification_code))
                code = sessionData.verification_code;

            if (string.IsNullOrEmpty(code) || sessionData.verification_code.Equals(HttpSession.VerificationCode, StringComparison.CurrentCultureIgnoreCase))
                await StartSession();
            else
            {
                await DependencyService.Get<IEIDServices>()
                    .AbortAuthentication(new AbortAuthenticationRequest(AbortConstants.VerificationCodeFailed));
                await DependencyService.Get<INavigationService>()
                    .ShowMessagePage(MessagePageType.VerificationCodeFailed);
            }
        }

        private static async Task StartSession()
        {
            var response = await DependencyService.Get<IEIDServices>().InitSessionEID(new WidSessionRequest());

            switch (response.ApiResult)
            {
                case ApiResult.Ok:
                    {
                        if (!string.IsNullOrEmpty(response.WebService))
                        {
                            await DependencyService.Get<INavigationService>().PushAsync(new WebserviceConfirmViewModel(null, response));
                        }
                        else if (!string.IsNullOrEmpty(response.Action))
                        {
                            var model = new ConfirmModel(response.Action)
                            {
                                WIDSessionResponse = response,
                            };
                            await DependencyService.Get<INavigationService>().PushAsync(new ConfirmViewModel(model, false));
                        }

                        break;
                    }
                case ApiResult.Nok:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ErrorOccoured);
                    break;
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    break;
                case ApiResult.HostNotReachable:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoInternetConnection);
                    break;
                case ApiResult.SSLPinningError:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SSLException);
                    break;
            }
        }
    }
}
