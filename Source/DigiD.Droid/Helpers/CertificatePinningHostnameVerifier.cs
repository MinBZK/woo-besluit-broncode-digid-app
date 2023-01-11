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
﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using DigiD.Common.Services;
using Javax.Net.Ssl;
using Xamarin.Forms;

namespace DigiD.Droid.Helpers
{
    internal class CertificatePinningHostnameVerifier : Java.Lang.Object, IHostnameVerifier
    {
        private readonly bool _test;

        public CertificatePinningHostnameVerifier(bool test = false)
        {
            _test = test;
        }

        public bool Verify(string hostname, ISSLSession session)
        {
            if (_test)
                return false;

            var certificates = ExtractCertificates(session);
            return DependencyService.Get<ISslPinningService>().ValidateCertificate(hostname, certificates);
        }

        private static List<X509Certificate> ExtractCertificates(ISSLSession session)
        {
            var result = new List<X509Certificate>();
            var certificates = session.GetPeerCertificateChain();

            if (certificates == null)
                return result;

            foreach (var certificate in certificates)
            {
                result.Add(new X509Certificate(certificate.GetEncoded()));
            }

            return result;
        }
    }
}
