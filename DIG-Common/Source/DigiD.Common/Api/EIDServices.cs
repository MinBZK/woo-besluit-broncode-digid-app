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
