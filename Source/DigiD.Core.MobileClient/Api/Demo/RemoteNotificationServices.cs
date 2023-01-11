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
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class RemoteNotificationServices : IRemoteNotificationServices
    {
        /// <summary>
        /// /apps/notifications/register
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task<BaseResponse> RegisterNotificationToken(string token, bool enabled)
        {
            await Task.Delay(0);

            return DemoHelper.Log("/apps/notifications/register", new NotificationRegisterRequest
            {
                NotificationToken = token,
                Enabled = enabled
            }, new BaseResponse
            {
                ApiResult = ApiResult.Ok
            });
            
        }

        /// <summary>
        /// /apps/notifications/update
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<BaseResponse> UpdateNotificationToken(string token)
        {
            await Task.Delay(0);

            return DemoHelper.Log($"/apps/notifications/update", new NotificationUpdateRequest
            {
                NotificationToken = token,
                InstanceId = DependencyService.Get<IGeneralPreferences>().InstanceId,
                UserAppId = DependencyService.Get<IMobileSettings>().AppId
            }, new BaseResponse
            {
                ApiResult = ApiResult.Ok
            });
        }
    }
}
