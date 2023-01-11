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
using DigiD.Common.Interfaces;
using DigiD.Common.NFC.Helpers;
using DigiD.Droid.Helpers;
using DigiD.Droid.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(HttpMessageHandlerService))]
namespace DigiD.Droid.Services
{
    public class HttpMessageHandlerService : IHttpMessageHandlerService
    {
        private AndroidClientHandler _handler;

        public HttpMessageHandler GetHttpMessageHandler()
        {
            _handler = new AndroidClientHandler
            {
                AllowAutoRedirect = false,
#if DEBUG || DEV || TEST || ACC
                ConnectTimeout = TimeSpan.FromMilliseconds(200),
#endif
            };

            return _handler;
        }

        public int RemoveNativeCookies(string url = null, string name = null)
        {
            if (url == null)
            {
                if (_handler != null)
                    _handler.CookieContainer = new CookieContainer();
                return 0;
            }

            var cookieContainer = _handler.CookieContainer;
            var cookies = cookieContainer.GetCookies(new Uri(url));

            if (name != null)
            {
                try
                {
                    var cookie = cookies[name];
                    if (cookie != null)
                    {
                        cookie.Expired = true;

                        Debugger.WriteLine($"Cookie with name '{name}' was deleted.");
                        return 1;
                    }
                }
                catch
                {
                    Debugger.WriteLine($"Cookie with name '{name}' was not found.");
                    return 0;
                }
            }

            var deletedCookies = 0;

            for (int i = 0; i < cookies.Count; i++)
            {
                try
                {
                    var cookie = cookies[i];
                    cookie.Expired = true;

                    Debugger.WriteLine($"Cookie with name '{cookie.Name}' was deleted.");
                    deletedCookies++;
                }
                catch (Exception ex)
                {
                    Debugger.WriteLine($"Exception removing the cookie at index {i}, with message: {ex.Message}.");
                }
            }

            return deletedCookies;
        }
    }
}
