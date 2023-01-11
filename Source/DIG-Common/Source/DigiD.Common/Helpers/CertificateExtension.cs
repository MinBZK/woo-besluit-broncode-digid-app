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
