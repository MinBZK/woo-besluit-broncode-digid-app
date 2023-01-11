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
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DigiD.Common.Helpers;
using DigiD.Common.Models.Piwik;
using DigiD.Common.Settings;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DigiD.Common.Services.Piwik
{
    /// <summary>
    /// Actual HTTP dispatcher, that sends the event to Piwik.
    /// </summary>
    public class HttpDispatcher : IPiwikDispatcher
    {
        public HttpDispatcher(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));
            if (!baseUrl.EndsWith("/piwik.php", StringComparison.InvariantCulture)) throw new ArgumentException("Url should be a full url ending with /piwik.php", nameof(baseUrl));

            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }
        public string ScreenSize { get; set; }
        public string UserAgent { get; set; }

        /// <summary>
        /// Send a single event to Piwik
        /// </summary>
        /// <returns>The send.</returns>
        /// <param name="singlePiwikEvent">Single event.</param>
        public Task<bool> Send(PiwikEvent singlePiwikEvent)
        {
            return Send(new[] { singlePiwikEvent });
        }

        /// <summary>
        /// Send a list of events to Piwik, and return true if this succeeded.
        /// </summary>
        /// <returns>True if sent.</returns>
        /// <param name="events">Events. A list of <see cref="PiwikEvent">events</see> </param>
        public async Task<bool> Send(IEnumerable<PiwikEvent> events)
        {
            // we moeten een HTTP Post request aanmaken met een JSON body.
            var url = BaseUrl + "?rand=" + new Random().Next().ToString(CultureInfo.InvariantCulture);
            using (var message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                message.Headers.Add("User-Agent", UserAgent);
                message.Content = new StringContent(CreateBody(events), Encoding.UTF8, "application/json");
                var result = await PerformHttpSend(message);

                return result.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// Perform the actual sending. This method is virtual so you can assign your own transport (native or managed)
        /// or even intercept the calls for testing purposes.
        /// </summary>
        /// <returns>The http send.</returns>
        /// <param name="message">Message.</param>
        public virtual async Task<HttpResponseMessage> PerformHttpSend(HttpRequestMessage message)
        {
            using (var client = HttpHelper.GetClient())
            {
                return await client.SendAsync(message);
            }
        }

        /// <summary>
        /// Create the JSon body for the events
        /// </summary>
        /// <returns>The body.</returns>
        /// <param name="events">Events.</param>
        public string CreateBody(IEnumerable<PiwikEvent> events)
        {
            var jsondata = new JSonRequest() { Requests = events.Select(ConvertEvent) };
            return JsonConvert.SerializeObject(jsondata);
        }

        /// <summary>
        /// Convert a single event to a string like it would be appended to the URL.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="e">E.</param>
        public string ConvertEvent(PiwikEvent e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var dict = new Dictionary<string, string>();
            dict.Add("idsite", e.SiteID);
            dict.Add("rec", "1"); // record on or off
            dict.Add("url", e.Url ?? "http://unused");
            dict.Add("action_name", e.ActionName != null ? string.Join(" ", e.ActionName) : string.Empty);
            dict.Add("apiv", "1"); // API version (always 1)
            dict.Add("_idvc", e.Session.SessionsCount.ToString(CultureInfo.InvariantCulture));
            dict.Add("_viewts", ConvertToUnixTime(e.Session.LastVisit));
            dict.Add("_idts", ConvertToUnixTime(e.Session.FirstVisit));

            // device info
            dict.Add("res", ScreenSize);
            dict.Add("h", e.Date.Hour.ToString(CultureInfo.InvariantCulture));
            dict.Add("m", e.Date.Minute.ToString(CultureInfo.InvariantCulture));
            dict.Add("s", e.Date.Second.ToString(CultureInfo.InvariantCulture));
            dict.Add("lang", e.Language);

            dict.Add("uid", DependencyService.Get<IGeneralPreferences>().InstanceId);
            dict.Add("new_visit", e.IsNewSession ? "1" : "0");

            if (!string.IsNullOrEmpty(e.EventCategory))
            {
                dict.Add("e_c", e.EventCategory);
                dict.Add("e_a", e.EventAction);
                dict.Add("e_n", e.EventName);
                dict.Add("e_v", e.EventValue);
            }

            if (e.PageCustomVar.Any())
                dict.Add("_cvar", JsonConvert.SerializeObject(e.PageCustomVar));
            
            var data = "?" + string.Join("&", dict.Where(kv => kv.Value != null).Select((kv) => kv.Key + "=" + UrlEncode(kv.Value)));
            return data;
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static string ConvertToUnixTime(DateTimeOffset date)
        {
            return (date - UnixEpoch).TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }

        private static string UrlEncode(string input)
        {
#if __IOS__ || __ANDROID__
            return HttpUtility.UrlEncode(input);
#else
            return WebUtility.UrlEncode(input);
#endif
        }

        /// <summary>
        /// Internal class om JSonConvert te kunnen gebruiken.
        /// </summary>
        class JSonRequest
        {
            [JsonProperty("requests")]
            public IEnumerable<string> Requests { get; set; }
        }
    }
}
