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
using System.Net.Http;
using System.Threading.Tasks;
using DigiD.Common.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Models;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Api
{
    public class App2AppService : IApp2AppService
    {
        public async Task<InitApp2AppResponse> InitSessionApp2App(App2AppRequest requestData)
        {
            if (requestData.Destination.Contains("v4"))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestData.Destination))
                {
                    Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("SAMLRequest", requestData.SAMLRequest),
                        new KeyValuePair<string, string>("RelayState", requestData.RelayState),
                        new KeyValuePair<string, string>("Type", "app_to_app"),
                    })
                };

                return await HttpHelper.SendAsync<InitApp2AppResponse>(request, true, 10000);
            }

            return await HttpHelper.PostAsync<InitApp2AppResponse>(new Uri(requestData.Destination), new StartApp2AppRequest
            {
                SAMLRequest = requestData.SAMLRequest,
                RelayState = requestData.RelayState,
                SigAlg = requestData.SigAlg,
                Signature = requestData.Signature,
                Type = "app_to_app"
            });
        }

        public async Task<SamlArtifactResponse> GetSamlArtifact(SamlArtifactRequest requestData)
        {
            var host = DependencyService.Get<IGeneralPreferences>().SelectedHost;
            var uri = new Uri($"https://{host}/saml/idp/app_to_app_artifact");
            return await HttpHelper.PostAsync<SamlArtifactResponse>(uri, requestData);
        }
    }
}
