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
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;

namespace DigiD.Common.EID.Models.SecurityInfoObjects
{
    public class CADomainParameterInfo : Asn1Encodable, IAsn1Info
    {
        public DerObjectIdentifier OID { get; }
        public AlgorithmIdentifier DomainParameter { get; }
        public X9ECParameters Algorithm => new X9ECParameters(Asn1Sequence.GetInstance(DomainParameter.Parameters));

        public CADomainParameterInfo(Asn1Sequence sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            OID = (DerObjectIdentifier) sequence[0];
            DomainParameter = AlgorithmIdentifier.GetInstance(sequence[1]);
        }

        public override string ToString()
        {
            return $"CADomainParameters\r\nOID: {OID}\r\nDomainParameter: {DomainParameter.Algorithm}\r\n";
        }

        public override Asn1Object ToAsn1Object()
        {
            var v = new Asn1EncodableVector { OID, DomainParameter };
            return new DerSequence(v);
        }
    }
}
