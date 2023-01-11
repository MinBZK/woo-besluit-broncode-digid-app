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
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.SessionModels;


namespace DigiD.Common.Api
{
    public class EIDServices : IEIDServices
    {

        /// <summary>
        /// De app stuurt een bericht hiernaar toe om aan te geven dat de app nooit de RDA server heeft kunnen bereiken.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<WidSessionResponse> InitSessionEID(WidSessionRequest requestData)
        {
            var response = await HttpHelper.PostAsync<WidSessionResponse>("/apps/wid/new", requestData);
            return response;
        }

        /// <summary>
        /// Bevestigen van authenticatie voor Hoog
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> Confirm()
        {
            return (await HttpHelper.PostAsync<BaseResponse>("/apps/wid/confirm", new BaseEIDRequest())).ApiResult;
        }

        /// <summary>
        /// Hoog cancelled endpoint
        /// </summary>
        public async Task<ApiResult> CancelAuthenticate()
        {
            if (string.IsNullOrEmpty(HttpSession.AppSessionId))
                return ApiResult.Ok;

            if (!string.IsNullOrEmpty(HttpSession.HostName))
                return (await HttpHelper.PostAsync<BaseResponse>("/apps/wid/cancel_authentication", new BaseEIDRequest())).ApiResult;
            
            return ApiResult.Aborted;
        }

        /// <summary>
        /// Hoog abort endpoint
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public async Task<ApiResult> AbortAuthentication(AbortAuthenticationRequest requestData)
        {
            if (string.IsNullOrEmpty(HttpSession.AppSessionId))
                return ApiResult.Ok;

            if (!string.IsNullOrEmpty(HttpSession.HostName))
                return (await HttpHelper.PostAsync<BaseResponse>("/apps/wid/abort_authentication", requestData)).ApiResult;

            return ApiResult.Aborted;
        }

        /// <summary>
        /// Polling endpoint voor de app om te checken of het inloggen met rijbewijs gelukt is
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponse> Poll()
        {
            var result = await HttpHelper.PostAsync<BaseResponse>("/apps/wid/poll", new BaseEIDRequest());

            while (result.ApiResult == ApiResult.Pending)
            {
                await Task.Delay(1000);
                result = await HttpHelper.PostAsync<BaseResponse>("/apps/wid/poll", new BaseEIDRequest());
            }

            return result;
        }
    }
}
