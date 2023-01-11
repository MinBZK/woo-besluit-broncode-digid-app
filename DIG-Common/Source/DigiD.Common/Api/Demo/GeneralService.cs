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
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.SessionModels;

namespace DigiD.Common.Api.Demo
{
    public class GeneralService : IGeneralServices
    {
        public async Task<BaseResponse> Cancel(bool cancelledByUser = false, bool timeout = false)
        {
            if (Http.Helpers.DemoHelper.Sessions.ContainsKey(HttpSession.AppSessionId))
                Http.Helpers.DemoHelper.Sessions.Remove(HttpSession.AppSessionId);

            await Task.Delay(100);

            return new BaseResponse
            {
                ApiResult = ApiResult.Ok
            };
        }

        public async Task<ConfigResponse> GetConfig()
        {
            await Task.Delay(100);
            return new ConfigResponse
            {
                ApiResult = ApiResult.Ok,
                MaxPinChangePerDay = 3,
                RDAEnabled = true,
                RequestStationEnabled = true,
                LetterRequestDelay = 3,
            };
        }

        public Task<bool> SslPinningCheck()
        {
            return Task.FromResult(true);
        }

        public async Task<AppServiceResponse> GetServices()
        {
            Debugger.WriteLine("/apps/services");
            await Task.Delay(0);
            return new AppServiceResponse
            {
                Services = new List<App>
                {
                    new App {Name = Constants.DemoConstants.MijnDigidAppName, Url = "https://mijn.digid.nl/authn_app"}
                },
                ApiResult = ApiResult.Ok,
            };
        }
    }
}
