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
