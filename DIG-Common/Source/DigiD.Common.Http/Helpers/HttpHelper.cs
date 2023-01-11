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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DigiD.Common.Http.Enums;
using DigiD.Common.Http.Models;
using Newtonsoft.Json;

namespace DigiD.Common.Http.Helpers
{
    public static class HttpHelper
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, Uri uri, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            using (var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri,
            })
            {
                return await client.SendAsync<T>(requestMessage, retry, timeout);
            }
        }

        public static async Task<T> PostAsync<T>(this HttpClient client, Uri uri, object data, bool retry = true, int timeout = 60000) where T : BaseResponse
        {
            using (var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
            })
            {
                var postData = JsonConvert.SerializeObject(data);
                System.Diagnostics.Debug.WriteLine($"[DigiD] RequestData: \"{postData}\"");
                requestMessage.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                return await client.SendAsync<T>(requestMessage, retry, timeout);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "<Pending>")]
        public static async Task<T> SendAsync<T>(this HttpClient client, HttpRequestMessage message, bool retry = false, int timeout = 60000, int tries = 0, Action<ExceptionModel> exceptionAction = null) where T : BaseResponse
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var result = (T)Activator.CreateInstance(typeof(T));

            var props = new Dictionary<string, string>
            {
                {"uri", message.RequestUri.ToString()},
                {"method", message.Method.ToString()},
                {"tries", tries.ToString()}
            };

            HttpResponseMessage response = null;

            try
            {

                var cts = new CancellationTokenSource();
                cts.CancelAfter(timeout);
                response = await client.SendAsync(message, cts.Token);
                cts.Dispose();
                props.Add("status", ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture));

                System.Diagnostics.Debug.WriteLine("[DigiD] **** SendAsync *******************************************\n" +
                                                   "[DigiD] \tHttpRequestMessage\n" +
                                                   $"[DigiD] \t\tUri:        \"{message.RequestUri}\" \n" +
                                                   $"[DigiD] \t\tMethod:     \"{message.Method}\" \n" +
                                                   "[DigiD] **********************************************************\n");
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[DigiD] ResponseData: \"{responseData}\"");

                result = JsonConvert.DeserializeObject<T>(responseData);

                if (result == null)
                    throw new InvalidDataException("ResponseData is invalid");

                if (string.IsNullOrEmpty(result.Status))
                    result.Status = "";

#pragma warning disable S1479 // "switch" statements should not have too many "case" clauses
                switch (result.Status.ToLowerInvariant())
#pragma warning restore S1479 // "switch" statements should not have too many "case" clauses
                {
                    case "no_documents":
                        result.ApiResult = ApiResult.NoDocumentsFound;
                        break;
                    case "kill_app":
                        result.ApiResult = ApiResult.Kill;
                        break;
                    case "unknown":
                        result.ApiResult = ApiResult.Unknown;
                        break;
                    case "nok":
                        result.ApiResult = ApiResult.Nok;
                        break;
                    case "ok":
                        result.ApiResult = ApiResult.Ok;
                        break;
                    case "pending":
                        result.ApiResult = ApiResult.Pending;
                        break;
                    case "cancelled":
                        result.ApiResult = ApiResult.Cancelled;
                        break;
                    case "disabled":
                        result.ApiResult = ApiResult.Disabled;
                        break;
                    case "success":
                        result.ApiResult = ApiResult.Ok;
                        break;
                    case "failed":
                        result.ApiResult = ApiResult.Failed;
                        break;
                    case "error":
                        result.ApiResult = ApiResult.Error;
                        break;
                    case "blocked":
                        result.ApiResult = ApiResult.Blocked;
                        break;
                    case "pending_confirmed":
                        result.ApiResult = ApiResult.PendingConfirmed;
                        break;
                    case "active":
                        result.ApiResult = ApiResult.Active;
                        break;
                    case "inactive":
                        result.ApiResult = ApiResult.InActive;
                        break;
                    case "aborted":
                        result.ApiResult = ApiResult.Aborted;
                        break;
                    case "verified":
                        result.ApiResult = ApiResult.Verified;
                        break;
                    case "confirmed":
                        result.ApiResult = ApiResult.Confirmed;
                        break;
                    case "bsns_not_identical":
                        result.ApiResult = ApiResult.BsnNotIdentical;
                        break;
                    default:
                        result.ApiResult = ApiResult.Ok;
                        break;
                }

                return result;
            }
            catch (OperationCanceledException oce)
            {
                exceptionAction?.Invoke(new ExceptionModel(oce, props));

                System.Diagnostics.Debug.WriteLine($"[DigiD] {oce} \r\b");

                if (tries <= 2 && retry)
                {
                    using (var clone = await message.Clone())
                    {
                        return await client.SendAsync<T>(clone, true, timeout, tries + 1);
                    }
                }

                if (result != null)
                    result.ApiResult = ApiResult.Timeout;

                return result;
            }
            catch (Exception ex)
            {
                //400/404/500 errors en TaskCancelled vervuilen AppCenter enorm
                if ((response != null && response.StatusCode != HttpStatusCode.InternalServerError && response.StatusCode != HttpStatusCode.BadRequest && response.StatusCode == HttpStatusCode.NotFound) && !(ex is TaskCanceledException))
                    exceptionAction?.Invoke(new ExceptionModel(ex, props));

                return HandleException(ex, result, response);
            }
        }

        private static T HandleException<T>(Exception ex, T result, HttpResponseMessage response) where T : BaseResponse
        {
            System.Diagnostics.Debug.WriteLine($"[DigiD] {ex.Message}");

            if (ex.InnerException?.Message?.Contains("TrustFailure") == true || ex.GetType()?.Name == "SSLPeerUnverifiedException" || ex is WebException we && we.Status == WebExceptionStatus.TrustFailure)
            {
                result.ApiResult = ApiResult.SSLPinningError;
                return result;
            }

            if (response != null && response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.ApiResult = ApiResult.Forbidden;
                return result;
            }

            if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
            {
                result.ApiResult = ApiResult.BadRequest;
                return result;
            }

            if (response != null && response.StatusCode == HttpStatusCode.NotFound)
            {
                result.ApiResult = ApiResult.SessionNotFound;
                return result;
            }

            result.ApiResult = ApiResult.Unknown;
            return result;
        }
    }
}
