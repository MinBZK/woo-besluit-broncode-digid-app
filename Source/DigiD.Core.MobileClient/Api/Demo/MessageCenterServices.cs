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
using System.Threading.Tasks;
using DigiD.Common.Http.Enums;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.RequestModels;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Settings;
using DigiD.Interfaces;
using Xamarin.Forms;

namespace DigiD.Api.Demo
{
    public class MessageCenterServices : IMessageCenterServices
    {
        /// <summary>
        /// /apps/notifications/get_notifications
        /// </summary>
        /// <returns></returns>
        public async Task<MessageCenterResponse> GetMessages()
        {
            await Task.Delay(0);

            var requestData = new MessageCenterRequest
            {
                Language = DependencyService.Get<IGeneralPreferences>().Language
            };
            
            var result = new MessageCenterResponse { ApiResult = ApiResult.Ok };

            if (DemoHelper.CurrentUser.HasMessages)
            {
                result.Messages.Add(new Message { DateTime = new DateTime(2020, 3, 3, 8, 0, 0), Text = "Hallo, dit is een langere bericht. Dit textveld heeft een maximale breedte. Breder dan dit wordt het niet. Dit bericht heeft ook loremipsum.nl" });
                result.Messages.Add(new Message { DateTime = new DateTime(2020, 3, 3, 8, 23, 0), Text = "Lorem ipsum dolor sit amet illusi." });
                result.Messages.Add(new Message { DateTime = new DateTime(2020, 4, 17, 16, 30, 0), Text = "Dit is een korte bericht van DigiD." });
                result.Messages.Add(new Message { DateTime = new DateTime(2020, 4, 17, 16, 45, 0), Text = "Lorem ipsum." });
                result.Messages.Add(new Message { DateTime = new DateTime(2020, 7, 2, 13, 49, 0), Text = "Hallo, dit is een langere bericht. Dit textveld heeft een maximale breedte. Breder dan dit wordt het niet. Dit bericht heeft ook een link.nl" });
                result.Messages.Add(new Message { DateTime = DateTime.Now.AddDays(-1).AddMinutes(20), Text = "Meerdere berichten op dezelfde dag staan zo onder elkaar." });
                result.Messages.Add(new Message { DateTime = DateTime.Now.AddDays(-1).AddMinutes(40), Text = "Dit is een extra lange bericht. Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum." });
                result.Messages.Add(new Message { DateTime = DateTime.Now.AddDays(-1).AddMinutes(35), Text = "Lorem ipsum." });
                result.Messages.Add(new Message { DateTime = DateTime.Now, Text = "Dit is een korte bericht van DigiD.", IsRead = false });
            }

            return DemoHelper.Log("/apps/notifications/get_notifications", requestData, result);
        }
    }
}
