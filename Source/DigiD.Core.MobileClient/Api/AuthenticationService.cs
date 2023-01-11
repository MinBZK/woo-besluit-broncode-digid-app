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
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;

namespace DigiD.Api
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<BaseResponse> SessionStatus()
        {
            if (string.IsNullOrEmpty(AppSession.AuthenticationSessionId))
                return new BaseResponse {ApiResult = ApiResult.SessionNotFound};

            return await HttpHelper.PostAsync<StartSessionBaseResponse>("/apps/session_status", new BaseRequest());
        }

        public async Task<StartSessionBaseResponse> InitSessionAuthentication()
        {
            return await HttpHelper.GetAsync<StartSessionBaseResponse>("/apps/request_session");
        }

        public async Task<ChallengeResponse> Challenge(ChallengeRequest requestData)
        {
            return await HttpHelper.PostAsync<ChallengeResponse>("/apps/challenge_auth", requestData);
        }

        public async Task<ValidatePinResponse> ValidatePin(ValidatePinRequest requestData)
        {
            return await HttpHelper.PostAsync<ValidatePinResponse>("/apps/check_pincode", requestData);
        }

        public async Task<ConfirmResponse> Confirm(ConfirmRequest requestData)
        {
            return await HttpHelper.PostAsync<ConfirmResponse>("/apps/confirm", requestData);
        }

        public async Task<WebSessionInfoResponse> SessionInfo()
        {
            var model = new WebSessionInfoRequest();
            return await HttpHelper.PostAsync<WebSessionInfoResponse>("/apps/web_session_information", model);
        }

        public async Task<ApiResult> AbortAuthentication(string abortCode)
        {
            return (await HttpHelper.PostAsync<BaseResponse>("/apps/abort_authentication", new AbortAuthenticationRequest(abortCode))).ApiResult;
        }
    }
}
