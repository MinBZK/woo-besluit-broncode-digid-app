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
using DigiD.Common.Constants;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Interfaces;

namespace DigiD.Api.Demo
{
    public class ChangePinServiceServices : IChangePinService
    {
        /// <summary>
        /// /apps/change_pin/request_session
        /// </summary>
        /// <returns></returns>
        public async Task<StartSessionBaseResponse> InitSession()
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/change_pin/request_session", new BaseRequest(), new StartSessionBaseResponse
            {
                ApiResult = ApiResult.Ok,
                AppSessionId = DemoHelper.NewSession(AuthenticationActions.ChangeAppPIN),
                At = DateHelper.GetEpochSeconds()
            });
        }

        /// <summary>
        /// /apps/change_pin/request_pin_change
        /// </summary>
        /// <param name="encryptedPIN"></param>
        /// <returns></returns>
        public async Task<BaseResponse> ChangePIN(string encryptedPIN)
        {
            await Task.Delay(0);
            return DemoHelper.Log($"/apps/change_pin/request_pin_change", new ChangePinRequest(encryptedPIN), new BaseResponse
            {
                ApiResult = ApiResult.Ok
            });
        }
    }
}
