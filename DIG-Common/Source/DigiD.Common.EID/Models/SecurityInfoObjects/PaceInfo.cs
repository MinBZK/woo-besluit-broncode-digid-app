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
using DigiD.Common.EID.Interfaces;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;

namespace DigiD.Common.EID.Models.SecurityInfoObjects
{
    public class PaceInfo : Asn1Encodable, IAsn1Info
    {
        public DerInteger Version { get; }
        public DerInteger ParameterId { get; }

        public PaceInfo(Asn1Sequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            OID = (DerObjectIdentifier)sequence[0];
            Version = (DerInteger)sequence[1];

            if (sequence.Count > 2)
                ParameterId = (DerInteger)sequence[2];
        }

        public override string ToString()
        {
            return $"PaceInfo\r\nOID: {OID}\r\nVersion: {Version}\r\nParameterId:{ParameterId}\n";
        }

        public override Asn1Object ToAsn1Object()
        {
            var v = new Asn1EncodableVector { OID, Version };
            if (ParameterId != null)
                v.Add(ParameterId);

            return new DerSequence(v);
        }

        public X9ECParameters Algorithm => ECNamedCurveTable.GetByName(AlgorithmName);

        public string AlgorithmName
        {
            get
            {
                switch (ParameterId.Value.IntValue)
                {
                    case 8:
                        return "secp192r1";
                    case 9:
                        return "brainpoolP192r1";
                    case 10:
                        return "secp224r1";
                    case 11:
                        return "brainpoolP224r1";
                    case 12:
                        return "prime256v1";
                    case 13:
                        return "brainpoolP256r1";
                    case 14:
                        return "brainpoolP320r1";
                    case 15:
                        return "secp384r1";
                    case 16:
                        return "brainpoolP384r1";
                    case 17:
                        return "brainpoolP512r1";
                }

                return string.Empty;
            }
        }

        public DerObjectIdentifier OID { get; set; }
    }
}
