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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Exceptions;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using Xamarin.Forms;

namespace DigiD.Common.Api
{
    public class GeneralServices : IGeneralServices
    {
        public async Task<BaseResponse> Cancel(bool cancelledByUser = false, bool timeout = false)
        {
            HttpSession.TempSessionData = null;

            switch (AppSession.Process)
            {
                case Process.UpgradeAndAuthenticate:
                case Process.AppActivateWithRDA:
                    return await HttpHelper.PostAsync<BaseResponse>("/apps/wid/cancel_authentication", new BaseMijnDigiDRequest());
            }

            if (AppSession.IsAppActivated)
            {
                return await HttpHelper.PostAsync<BaseResponse>($"/apps/cancel_authentication?usercancel={(cancelledByUser ? "1" : "0")}&timeout={(timeout ? "1" : "0")}", new BaseMijnDigiDRequest());
            }

            if (HttpSession.ActivationSessionData?.ActivationMethod == ActivationMethod.RequestNewDigidAccount)
            {
                return await HttpHelper.PostAsync<BaseResponse>("/apps/account_requests/cancel_application", new BaseMijnDigiDRequest());
            }

            return await HttpHelper.PostAsync<BaseResponse>("/apps/cancel_activation", new BaseMijnDigiDRequest());
        }

        public async Task<ConfigResponse> GetConfig()
        {
            return await HttpHelper.GetAsync<ConfigResponse>("/apps/config", false, 5000);
        }

        public async Task<bool> SslPinningCheck()
        {
            using (var client = new HttpClient(DependencyService.Get<ISecurityService>(DependencyFetchTarget.NewInstance).GetHttpMessageHandler()))
            {
                try
                {
                    var url = AppConfigConstants.PublicFileUrl.ToString();
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return false;
                }
                catch (Exception e)
                {
                    e = DependencyService.Get<IExceptionService>().Cast(e);

                    if (e is SslException || e is WebException we && we.Status == WebExceptionStatus.TrustFailure)
                        return true;

                    return false;
                }
            }
        }

        public async Task<AppServiceResponse> GetServices()
        {
            return await HttpHelper.GetAsync<AppServiceResponse>("/apps/services", false, 5000);
        }
    }
}
