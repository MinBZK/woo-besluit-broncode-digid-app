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
using System.Diagnostics;
using System.Linq;
using System.Text;
using DigiD.Common.EID.Helpers;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Models;
using DigiD.Common.NFC.Helpers;
using Xamarin.Forms;

namespace DigiD.Common.Mobile.Helpers
{
    public static class DemoHelper
    {
        public static T Log<T>(string url, object requestData, T responseData) where T:new()
        {
            var sb = new StringBuilder();

            sb.Append("\r\n");
            sb.Append("***************************************************\r\n");
            sb.Append($"Url: {url}\r\n");
            sb.Append($"RequestData: {requestData.AsJson()}\r\n");
            sb.Append($"ResponseData: {responseData.AsJson()}\r\n");
            sb.Append("***************************************************\r\n");
            Debug.WriteLine(sb.ToString());

            return responseData;
        }

        public static string NewSession(string action = null, string sessionId = null, LoginLevel? loginLevel = null)
        {
            var key = sessionId ?? Guid.NewGuid().ToString();

            if (Http.Helpers.DemoHelper.Sessions.ContainsKey(key))
            {
                Http.Helpers.DemoHelper.Sessions[key].Action = action;
                return key;
            }

            var session = new DemoSession
            {
                Action = action,
                Challenge = CryptoHelper.GenerateRandom(32).ToBase16(),
                IV = CryptoHelper.GenerateRandom(16).ToBase16(),
                DateTime = DateTime.Now,
                LoginLevel = loginLevel?.ToInt() ?? 20
            };
            
            Http.Helpers.DemoHelper.Sessions.Add(key, session);
            return key;
        }

        public static void ExtendSession(string sessionId)
        {
            if (Http.Helpers.DemoHelper.Sessions.ContainsKey(sessionId))
                Http.Helpers.DemoHelper.Sessions[sessionId].DateTime = DateTime.Now;
        }

        public static DemoUser CurrentUser =>
            Constants.DemoConstants.DemoUsers.FirstOrDefault(x => x.UserId == DependencyService.Get<IDemoSettings>()?.UserId);
    }
}
