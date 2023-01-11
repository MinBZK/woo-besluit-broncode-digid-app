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
﻿using System.Linq;
using DigiD.Common.EID.Helpers;
using Org.BouncyCastle.Asn1;

namespace DigiD.Common.EID.Models
{
    public class Certificate
    {
        public Certificate(byte[] certBytes)
        {
            using (var stream = new Asn1InputStream(certBytes))
            {
                var data = (DerSequence)stream.ReadObject();
                Parse(data);
            }
        }

        public byte[] Body { get; private set; }
        public byte[] Signature { get; private set; }

        public DataObject ProfileIdentifier { get; set; }
        public DataObject AuthReference { get; set; }
        public DataObject PublicKeyOID { get; set; }
        public DataObject PublicKeyECPoint { get; set; }
        public DataObject HolderReference { get; set; }
        public DataObject ChatOID { get; set; }
        public DataObject ChatDiscretionaryData { get; set; }
        public DataObject EffectiveDate { get; set; }
        public DataObject ExpirationDate { get; set; }
        public DataObject CertRelativeAuth { get; set; }


        private void Parse(DerSequence o)
        {
            var certificateEncoded = Asn1Sequence.GetInstance(o);
            Body = certificateEncoded[0].ToAsn1Object().GetEncoded();

            Signature = certificateEncoded[1].ToAsn1Object().GetEncoded();

            var certBody = Asn1Sequence.GetInstance(((DerApplicationSpecific)certificateEncoded[0]).GetObject(16)); //Sequence

            var tmp = certBody[0].ToAsn1Object().GetEncoded();
            ProfileIdentifier = new DataObject(new[] { tmp[0], tmp[1] }, tmp.Skip(3).ToArray());

            tmp = certBody[1].ToAsn1Object().GetEncoded();
            AuthReference = new DataObject(new[] { tmp[0] }, tmp.Skip(2).ToArray());

            var sequence = Asn1Sequence.GetInstance(((DerApplicationSpecific)certBody[2]).GetObject(16)); //Sequence

            tmp = sequence[0].ToAsn1Object().GetEncoded();
            PublicKeyOID = new DataObject(new []{tmp[0]}, tmp.Skip(2).ToArray());

            tmp = sequence[1].ToAsn1Object().GetEncoded();
            PublicKeyECPoint = new DataObject(new[]{tmp[0]}, tmp.Skip(3).ToArray());

            tmp = certBody[3].ToAsn1Object().GetEncoded();
            HolderReference = new DataObject(new []{tmp[0], tmp[1]}, tmp.Skip(3).ToArray() );

            sequence = Asn1Sequence.GetInstance(((DerApplicationSpecific)certBody[4]).GetObject(16)); //Sequence
            tmp = sequence[0].ToAsn1Object().GetEncoded();
            ChatOID = new DataObject(new[] { tmp[0] }, tmp.Skip(2).ToArray());

            tmp = sequence[1].ToAsn1Object().GetEncoded();
            ChatDiscretionaryData = new DataObject(new[] { tmp[0] }, tmp.Skip(2).ToArray());

            tmp = certBody[5].ToAsn1Object().GetEncoded();
            EffectiveDate = new DataObject(new[] { tmp[0], tmp[1] }, tmp.Skip(3).ToArray());

            tmp = certBody[6].ToAsn1Object().GetEncoded();
            ExpirationDate = new DataObject(new[] { tmp[0], tmp[1] }, tmp.Skip(3).ToArray());

            tmp = certBody[7].ToAsn1Object().GetEncoded();
            CertRelativeAuth = DataObjectHelper.GetDataObject(tmp);
        }
    }
}
