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
