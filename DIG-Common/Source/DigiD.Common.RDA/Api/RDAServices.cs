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
using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;

namespace DigiD.Common.RDA.Api
{
    public class RdaServices : IRdaServices
    {
        public async Task<InitSessionResponse> InitSessionRDA()
        {
            var requestData = new RdaInitRequest();
            return await HttpHelper.PostAsync<InitSessionResponse>("/apps/rda/init", requestData);
        }

        public async Task<BaseResponse> Documents()
        {
            var data = new
            {
                app_session_id = HttpSession.AppSessionId
            };

            return await HttpHelper.PostAsync<BaseResponse>("/apps/rda/documents", data);
        }

        public async Task<RdaSessionResponse> Poll()
        {
            var data = new
            {
                app_session_id = HttpSession.AppSessionId
            };

            return await HttpHelper.PostAsync<RdaSessionResponse>("/apps/rda/poll", data);
        }

        public async Task<ApiResult> Verified()
        {
            var data = new
            {
                app_session_id = HttpSession.AppSessionId
            };

            return (await HttpHelper.PostAsync<BaseResponse>("/apps/rda/verified", data)).ApiResult;
        }

        public async Task<BaseResponse> Cancel()
        {
            var data = new
            {
                app_session_id = HttpSession.AppSessionId
            };

            return await HttpHelper.PostAsync<BaseResponse>("/apps/rda/cancel", data);
        }

        public async Task<RdaStartResponse> Start(string type, EFCardAccess cardAccess)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                type,
                cardAccessData = cardAccess?.Base64Encoded
            };

            return await HttpHelper.PostAsync<RdaStartResponse>(new Uri($"{HttpSession.RDASessionData.Url}/v1/start"), data);
        }

        public async Task<SelectResponse> Challenge(string challenge)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                challenge
            };
            return await HttpHelper.PostAsync<SelectResponse>(new Uri($"{HttpSession.RDASessionData.Url}/v1/challenge"), data);
        }

        public async Task<CommandResponse> Authenticate(string authenticate, string challenge)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                authenticate,
                challenge
            };
            return await HttpHelper.PostAsync<CommandResponse>(new Uri($"{HttpSession.RDASessionData.Url}/v1/authenticate"), data);
        }

        public async Task<CommandResponse> SecureMessaging(List<string> responses)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                responses
            };
            return await HttpHelper.PostAsync<CommandResponse>(new Uri($"{HttpSession.RDASessionData.Url}/v1/secure_messaging"), data);
        }

        public async Task<RdaSessionResponse> InitForeignDocument(InitForeignDocumentRequestModel model)
        {
            return await HttpHelper.PostAsync<RdaSessionResponse>("/apps/rda/init_mrz_document", model);
        }

        public async Task<PrepareResponseModel> Prepare(byte[] encryptedNonce)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                encryptedNonce
            };

            return await HttpHelper.PostAsync<PrepareResponseModel>(new Uri($"{HttpSession.RDASessionData.Url}/v1/prepare"), data);
        }

        public async Task<MapResponseModel> Map(byte[] mappedNonce)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                mappedNonce
            };

            return await HttpHelper.PostAsync<MapResponseModel>(new Uri($"{HttpSession.RDASessionData.Url}/v1/map"), data);
        }

        public async Task<AgreeKeyResponseModel> KeyAgreement(byte[] keyAgree)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                keyAgree
            };

            return await HttpHelper.PostAsync<AgreeKeyResponseModel>(new Uri($"{HttpSession.RDASessionData.Url}/v1/key_agreement"), data);
        }

        public async Task<MutualAuthResponseModel> MutualAuthenticate(byte[] authenticate, byte[] encryptedNonce = null)
        {
            var data = new
            {
                sessionId = HttpSession.RDASessionData.SessionId,
                authenticate,
                encryptedNonce
            };

            return await HttpHelper.PostAsync<MutualAuthResponseModel>(new Uri($"{HttpSession.RDASessionData.Url}/v1/mutual_auth"), data);
        }
    }
}
