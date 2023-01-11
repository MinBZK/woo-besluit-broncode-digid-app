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
