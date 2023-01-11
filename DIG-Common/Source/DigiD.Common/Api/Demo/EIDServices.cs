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
﻿using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Helpers;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;

namespace DigiD.Common.Api.Demo
{
    public class EIDServices : IEIDServices
    {
        public async Task<WidSessionResponse> InitSessionEID(WidSessionRequest requestData)
        {
            await Task.Delay(20);

            var session = DemoHelper.GetSession(requestData.AppSessionId);

            var response = new WidSessionResponse
            {
                ApiResult = ApiResult.Ok,
            };

            if (!string.IsNullOrEmpty(session.Action))
            {
                response.Action = session.Action;

                switch (session.Action)
                {
                    case AuthenticationActions.ACTIVATE_IDENTITY_CARD:
                        HttpSession.IsWeb2AppSession = false;
                        break;
                    case "webservice":
                        response.WebService = "DigiD Demo EID";
                        break;
                }
            }

            return response;
        }

        public async Task<ApiResult> Confirm()
        {
            await Task.Delay(100);
            return ApiResult.Ok;
        }

        public async Task<ApiResult> CancelAuthenticate()
        {
            await Task.Delay(100);
            return ApiResult.Ok;
        }

        public async Task<ApiResult> AbortAuthentication(AbortAuthenticationRequest requestData)
        {
            await Task.Delay(100);
            return ApiResult.Ok;
        }

        public async Task<BaseResponse> Poll()
        {
            await Task.Delay(500);

            return new BaseResponse
            {
                ApiResult = ApiResult.Verified
            };
        }
    }
}
