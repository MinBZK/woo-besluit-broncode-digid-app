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
﻿using DigiD.Common.Constants;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DigiD.Common.Http.Helpers;
using DigiD.Common.Http.Models;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Common.Helpers
{
    public static class HttpHelper
    {
        public static HttpClient GetClient()
        {
            var client = new HttpClient(DependencyService.Get<IHttpMessageHandlerService>(DependencyFetchTarget.NewInstance).GetHttpMessageHandler())
            {
                Timeout = AppConfigConstants.DefaultHttpTimeout
            };

            var version = Xamarin.Essentials.AppInfo.Version;

            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.TryAddWithoutValidation("API-Version", "3");
            client.DefaultRequestHeaders.TryAddWithoutValidation("App-Version", $"{version.Major}.{version.Minor}.{version.Build}");
            client.DefaultRequestHeaders.TryAddWithoutValidation("OS-Type", DependencyService.Get<IDevice>().RuntimePlatform);
            client.DefaultRequestHeaders.TryAddWithoutValidation("OS-Version", Xamarin.Essentials.DeviceInfo.VersionString);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Release-Type", AppConfigConstants.ReleaseType.ToString());
            
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", DependencyService.Get<IGeneralPreferences>().InstanceId.ToUpperInvariant());
            client.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache, no-store");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static async Task<T> GetAsync<T>(string path, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            return await GetAsync<T>(path.ToUri(), retry, timeout);
        }

        public static async Task<T> GetAsync<T>(Uri uri, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            using var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
            };
            return await SendAsync<T>(requestMessage, retry, timeout);
        }

        public static async Task<T> PostAsync<T>(string path, object data, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            return await PostAsync<T>(path.ToUri(), data, retry, timeout);
        }

        public static async Task<T> PostAsync<T>(Uri uri, object data, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            using var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
            };
            var postData = JsonConvert.SerializeObject(data);
            System.Diagnostics.Debug.WriteLine($"[DigiD] RequestData: \"{postData}\"\r\n");
            requestMessage.Content = new StringContent(postData, Encoding.UTF8, "application/json");

            return await SendAsync<T>(requestMessage, retry, timeout);
        }

        public static async Task<T> SendAsync<T>(HttpRequestMessage message, bool retry = false, int timeout = 60000,
            int tries = 0) where T : BaseResponse
        {
            using var client = GetClient();
            return await client.SendAsync<T>(message, retry, timeout, tries, ExceptionAction);
        }

        private static void ExceptionAction(ExceptionModel obj)
        {
            if (HttpSession.IsApp2AppSession)
                AppCenterHelper.TrackEvent("App2App-Error", obj.Properties);

            AppCenterHelper.TrackError(obj.Exception, obj.Properties);
        }
    }
}
