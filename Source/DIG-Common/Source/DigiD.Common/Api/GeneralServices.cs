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
