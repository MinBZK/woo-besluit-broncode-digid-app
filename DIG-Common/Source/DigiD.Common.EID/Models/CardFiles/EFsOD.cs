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
using System.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;

namespace DigiD.Common.EID.Models.CardFiles
{
    public class EFSOd : File
    {
        internal SignedData SignedData { get; private set; }
        internal Dictionary<int, byte[]> Hashes = new Dictionary<int, byte[]>();

        public EFSOd(byte[] data) : base(data, "EF.SOd")
        {
        }

        public override bool Parse()
        {
            using (var s = new Asn1InputStream(Bytes))
            {
                var app = (DerApplicationSpecific)s.ReadObject();
                var sequence = (DerSequence)app.GetObject();
                var objectIdentifier = (DerObjectIdentifier)sequence[0];

                if (objectIdentifier.Id != "1.2.840.113549.1.7.2")
                {
                    //Didn't found SignedData!
                    return false;
                }

                var s2 = (DerSequence)((DerTaggedObject)sequence[1]).GetObject();
                SignedData = SignedData.GetInstance(s2);

                var obj1 = (DerTaggedObject)sequence[1];
                var seq2 = (DerSequence)obj1.GetObject();
                var seq3 = (DerSequence)seq2[2];
                var obj2 = (DerTaggedObject)seq3[1];
                var dos = (DerOctetString)obj2.GetObject();

                var mRTDSignatureData = (DerSequence)Asn1Object.FromByteArray(dos.GetOctets());
                var hashes = (DerSequence)mRTDSignatureData[2];

                foreach (var hash in hashes.Cast<DerSequence>())
                {
                    var nr = ((DerInteger)hash[0]).PositiveValue.IntValue;
                    Hashes.Add(nr, ((Asn1OctetString)hash[1]).GetOctets());
                }
            }

            return true;
        }
    }
}
