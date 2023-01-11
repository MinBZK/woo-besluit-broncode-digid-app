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
using DigiD.Common.EID.Interfaces;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;

namespace DigiD.Common.EID.Models
{
    public class ChipAuthenticationPublicKeyInfo : IAsn1Info
    {
        public byte[] Pk { get; }
        public DerObjectIdentifier OID { get; set; }
        public static X9ECParameters Algorithm => ECNamedCurveTable.GetByName("BrainpoolP320R1");

        public ChipAuthenticationPublicKeyInfo(Asn1Sequence data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            OID = DerObjectIdentifier.GetInstance(data[0]);
            var primitive = ((DerSequence) data[1])[1].ToAsn1Object();
            Pk = ((DerBitString) Asn1Object.FromByteArray(primitive.GetEncoded())).GetBytes();
        }
    }
}
