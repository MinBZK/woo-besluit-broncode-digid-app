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
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.SecurityInfoObjects;
using Org.BouncyCastle.Asn1;

namespace DigiD.Common.EID.Helpers
{
    internal static class InfoFactory
    {
        private const string Protocols = "2";
        private const string SmartCard = "2";
        private const string Pace = "4";
        private const string CA = "3";
        private const string TA = "2";
        private const string PK = "1";

        private static DerObjectIdentifier Base_OID = new DerObjectIdentifier("0.4.0.127.0.7");
        private static DerObjectIdentifier Pace_OID = Base_OID.Branch(Protocols).Branch(SmartCard).Branch(Pace);
        private static DerObjectIdentifier CA_OID = Base_OID.Branch(Protocols).Branch(SmartCard).Branch(CA);
        private static DerObjectIdentifier TA_OID = Base_OID.Branch(Protocols).Branch(SmartCard).Branch(TA);
        private static DerObjectIdentifier PK_OID = Base_OID.Branch(Protocols).Branch(SmartCard).Branch(PK);

        internal static IAsn1Info Parse(DerSequence sequence)
        {
            var obj = sequence[0];

            if (!(obj is DerObjectIdentifier))
                throw new ArgumentException("First object should be of type OID", nameof(sequence));

            var oid = (DerObjectIdentifier) obj;

            if (oid.Id.IndexOf(Pace_OID.Id, StringComparison.Ordinal) == 0)
                return new PaceInfo(sequence);
            if (oid.Id.IndexOf(CA_OID.Id, StringComparison.Ordinal) == 0)
            {
                if (oid.ToString().Length == 23)
                    return new CAInfo(sequence);

                return new CADomainParameterInfo(sequence);
            }
            if (oid.Id.IndexOf(TA_OID.Id, StringComparison.Ordinal) == 0)
                return new TAInfo(sequence);
            if (oid.Id.IndexOf(PK_OID.Id, StringComparison.Ordinal) == 0)
                return new ChipAuthenticationPublicKeyInfo(sequence);

            return null;
        }

        internal static IAsn1Info Parse<T>(Asn1Set set) where T : IAsn1Info
        {
            foreach (DerSequence sequence in set)
            {
                var result = Parse(sequence);
                if (result is T)
                    return result;
            }

            return null;
        }
    }
}
