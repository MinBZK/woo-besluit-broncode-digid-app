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
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace DigiD.Common.EID.CardSteps.PA
{
    public static class CmsVerifier
    {
        public static ContentInfo Verify(this SignedData signedData, ContentInfo signedMessage, DateTime date)
        {
            X509Certificate cert = certificate(signedData);
            var name = new X500DistinguishedName(cert.IssuerDN.GetEncoded());
            var cms = new CmsSignedData(signedMessage);
            var data = SignedData.GetInstance(signedMessage);
            return data.EncapContentInfo;
        }

        private static X509Certificate certificate(SignedData signedData)
        {
            var data = signedData.Certificates[0].ToAsn1Object();
            var x = new X509Certificate(X509CertificateStructure.GetInstance(data));
            return x;
        }
    }
}
