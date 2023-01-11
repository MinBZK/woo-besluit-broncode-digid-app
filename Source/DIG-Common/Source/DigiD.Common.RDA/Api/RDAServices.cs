// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
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
