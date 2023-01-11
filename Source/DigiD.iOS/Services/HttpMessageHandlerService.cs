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
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using DigiD.Common.Constants;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using DigiD.iOS.Services;
using Foundation;
using Security;
using Xamarin.Forms;

[assembly: Dependency(typeof(HttpMessageHandlerService))]
namespace DigiD.iOS.Services
{
    public class HttpMessageHandlerService : IHttpMessageHandlerService
    {
        public HttpMessageHandler GetHttpMessageHandler()
        {
            var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            config.TimeoutIntervalForRequest = AppConfigConstants.DefaultHttpTimeout.TotalSeconds;
            config.TimeoutIntervalForResource = AppConfigConstants.DefaultHttpTimeout.TotalSeconds;

            var nsUrlSessionHandler = new NSUrlSessionHandler(config)
            {
                TrustOverrideForUrl = (sender, url, trust) =>
                {
                    var certificates = ExtractCertificates(trust);
                    _ = Uri.TryCreate(url, UriKind.Absolute, out var uri);
                    return DependencyService.Get<ISslPinningService>().ValidateCertificate(uri?.Host, certificates);
                },
                AllowAutoRedirect = false,
                DisableCaching = true //https://books.nowsecure.com/secure-mobile-development/en/ios/avoid-caching-https-requests-responses.html
            };

            return nsUrlSessionHandler;
        }

        public int RemoveNativeCookies(string url = null, string name = null)
        {
            var cookieStorage = NSHttpCookieStorage.SharedStorage;
            var deletedCookies = 0;

            var cookies = url == null ? cookieStorage.Cookies.ToList() : cookieStorage.CookiesForUrl(new NSUrl(url)).ToList();
            cookies = name != null ? cookies.Where(x => x.Name == name).ToList() : cookies;

            foreach (var cookie in cookies)
            {
                cookieStorage.DeleteCookie(cookie);
                deletedCookies++;
            }

            NSUserDefaults.StandardUserDefaults.Synchronize();

            return deletedCookies;
        }

        private static List<X509Certificate> ExtractCertificates(SecTrust trust)
        {
            var result = new List<X509Certificate>();

            if (trust == null || trust.Count == 0)
                return result;

            try
            {
                for (var i = 0; i < trust.Count; i++)
                {
                    result.Add(trust[i].ToX509Certificate());
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Creating SSL thumbprint failed with exception: " + ex.Message);
            }

            return result;
        }
    }
}
