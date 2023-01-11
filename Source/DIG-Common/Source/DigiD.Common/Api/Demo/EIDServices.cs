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
