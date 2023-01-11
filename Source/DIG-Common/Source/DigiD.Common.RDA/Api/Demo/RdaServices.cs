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
using DigiD.Common.Constants;
using DigiD.Common.EID.Models.CardFiles;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;

namespace DigiD.Common.RDA.Api.Demo
{
    public class RdaServices : IRdaServices
    {
        private int _secureMessagingCount;
        private int _pollCount;

        public async Task<InitSessionResponse> InitSessionRDA()
        {
            await Task.Delay(0);

            return new InitSessionResponse
            {
                ApiResult = ApiResult.Ok,
                SessionId = DemoHelper.NewSession(AuthenticationActions.RDAUpgrade)
            };
        }

        public async Task<BaseResponse> Documents()
        {
            await Task.Delay(0);
            _pollCount = 0;
            return new BaseResponse { ApiResult = ApiResult.Ok };
        }

        public async Task<RdaSessionResponse> Poll()
        {
            await Task.Delay(1);
            _pollCount++;

            if (_pollCount != 5)
                return new RdaSessionResponse
                {
                    ApiResult = ApiResult.Pending
                };

            if (DemoHelper.CurrentUser?.HasNoDocuments == true)
                return new RdaSessionResponse
                {
                    ApiResult = ApiResult.NoDocumentsFound
                };

            return new RdaSessionResponse
            {
                ApiResult = ApiResult.Ok,
                SessionId = DemoHelper.NewSession(),
                Url = "https://rda-demo.digid.local"
            };
        }

        public Task<ApiResult> Verified()
        {
            return Task.FromResult(DemoHelper.CurrentUser?.RdaFailed == true ? ApiResult.Nok : ApiResult.Ok);
        }

        public Task<BaseResponse> Cancel()
        {
            return Task.FromResult(new BaseResponse { ApiResult = ApiResult.Ok });
        }

        public async Task<RdaStartResponse> Start(string type, EFCardAccess cardAccess)
        {
            await Task.Delay(0);

            var result = new RdaStartResponse
            {
                Select = type == "DRIVING_LICENCE" ? Convert.FromBase64String("SSSSSSSSSSSSSSSSSSSSSSSS") : Convert.FromBase64String("SSSSSSSSSSSSSSSS"),
                Status = "CHALLENGE",
                ApiResult = ApiResult.Ok,
                Challenge = "SSSSSSSS"
            };
            
            return result;
        }

        public async Task<SelectResponse> Challenge(string challenge)
        {
            await Task.Delay(0);

            return new SelectResponse
            {
                ApiResult = ApiResult.Ok,
                Status = "AUTHENTICATE",
                Authenticate = "SSSSSSSS"
            };
        }


        public async Task<CommandResponse> Authenticate(string authenticate, string challenge)
        {
            _secureMessagingCount = 0;

            await Task.Delay(0);
            return new CommandResponse
            {
                ApiResult = ApiResult.Ok,
                Status = "SECURE_MESSAGING",
                Commands = new List<string> { "SSSSSSSS" }
            };
        }

        public async Task<CommandResponse> SecureMessaging(List<string> responses)
        {
            _secureMessagingCount++;

            await Task.Delay(200);
            return new CommandResponse
            {
                ApiResult = ApiResult.Ok,
                Status = _secureMessagingCount == 9 ? "VERIFIED" : "SECURE_MESSAGING",
                Commands = new List<string> { "SSSSSSSS" }
            };
        }

        public async Task<RdaSessionResponse> InitForeignDocument(InitForeignDocumentRequestModel model)
        {
            await Task.Delay(0);

            if (DemoHelper.CurrentUser.DocumentNumber == model.DocumentNumber)
                return new RdaSessionResponse
                {
                    ApiResult = ApiResult.Ok,
                    SessionId = DemoHelper.NewSession(),
                    Url = "https://rda-demo.digid.local"
                };

            return new RdaSessionResponse
            {
                ApiResult = ApiResult.Nok
            };
        }

        public Task<PrepareResponseModel> Prepare(byte[] encryptedNonce)
        {
            throw new System.NotImplementedException();
        }

        public Task<MapResponseModel> Map(byte[] mappedNonce)
        {
            throw new System.NotImplementedException();
        }

        public Task<AgreeKeyResponseModel> KeyAgreement(byte[] keyAgree)
        {
            throw new System.NotImplementedException();
        }

        public Task<MutualAuthResponseModel> MutualAuthenticate(byte[] authenticate, byte[] encryptedNonce = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
