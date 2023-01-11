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
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace DigiD.Common.Helpers
{
    internal static class CertificateExtension
    {
        internal static string GetSubjectName(this X509Certificate cert)
        {
            return cert.Subject.Split(',').Select(x => x.Split('=')).First(x => x[0] == "CN")[1];
        }

        internal static IEnumerable<string> GetSubjectAlternativeNames(this X509Certificate cert)
        {
            if (cert == null)
                throw new ArgumentNullException(nameof(cert));

            using (var certificate = new X509Certificate2(cert))
            {
                foreach (var extension in certificate.Extensions)
                {
                    // Create an AsnEncodedData object using the extensions information.
                    var asnEncodedData = new AsnEncodedData(extension.Oid, extension.RawData);
                    if (string.Equals(extension.Oid.FriendlyName, "Subject Alternative Name", StringComparison.InvariantCulture))
                    {
                        return new List<string>(asnEncodedData.Format(false).Split(new[] { ", ", "DNS Name=" }, StringSplitOptions.RemoveEmptyEntries));
                    }
                }

                return new List<string>();
            }
        }
    }
}
