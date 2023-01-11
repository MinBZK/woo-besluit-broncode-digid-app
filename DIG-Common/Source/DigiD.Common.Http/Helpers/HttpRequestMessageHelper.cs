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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DigiD.Common.Http.Helpers
{
    public static class HttpRequestMessageHelper
    {
        public static async Task<HttpRequestMessage> Clone(this HttpRequestMessage req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Content = await req.Content.CloneAsync().ConfigureAwait(false), 
                Version = req.Version
            };

            foreach (var prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (var header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        /// <summary>Get a copy of the request content.</summary>
        /// <param name="content">The content to copy.</param>
        /// <remarks>Note that cloning content isn't possible after it's dispatched, because the stream is automatically disposed after the request.</remarks>
        internal static async Task<HttpContent> CloneAsync(this HttpContent content)
        {
            if (content == null)
                return null;
 
            Stream stream = new MemoryStream();
            await content.CopyToAsync(stream).ConfigureAwait(false);
            stream.Position = 0;
 
            StreamContent clone = new StreamContent(stream);
            foreach (var header in content.Headers)
                clone.Headers.Add(header.Key, header.Value);
 
            return clone;
        }
    }
}
