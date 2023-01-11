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
﻿using DigiD.Common.EID.Helpers;
using Org.BouncyCastle.Asn1;

namespace DigiD.Common.EID.Models.CardFiles
{
    public class EFCardSecurity : File
    {
        public DerObjectIdentifier OID { get; private set; }
        public byte[] KeyReference { get; private set; }
        public ChipAuthenticationPublicKeyInfo ChipAuthenticationPublicKeyInfo  { get; set; }

        public EFCardSecurity(byte[] data) : base(data, "EF.CardSecurity")
        {
        }

        public override bool Parse()
        {
            using (var s = new Asn1InputStream(Bytes))
            {
                var tagged1 = (DerTaggedObject) Asn1Sequence.GetInstance(s.ReadObject())[1];
                var seq = (Asn1Sequence) Asn1Sequence.GetInstance(tagged1, false)[0];
                var tagged2 = (DerTaggedObject)((Asn1Sequence) seq[2])[1];

                var idSecurityObject = Asn1OctetString.GetInstance(tagged2, false).GetOctets();

                var caPkInfoSeq = (DerSequence)((DerSet) Asn1Object.FromByteArray(idSecurityObject))[1];
                
                ChipAuthenticationPublicKeyInfo = (ChipAuthenticationPublicKeyInfo) InfoFactory.Parse(caPkInfoSeq);

                var caDomInfoSeq = (DerSequence)((DerSet)Asn1Object.FromByteArray(idSecurityObject))[2];

                OID = (DerObjectIdentifier) caDomInfoSeq[0];
                KeyReference = ((DerInteger) caDomInfoSeq[1]).PositiveValue.ToByteArray();
            }

            return true;
        }
    }
}
