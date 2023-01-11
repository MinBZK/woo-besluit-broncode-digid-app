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
﻿using DigiD.Common.EID.Helpers;
using DigiD.Common.EID.Models.SecurityInfoObjects;
using Org.BouncyCastle.Asn1;

namespace DigiD.Common.EID.Models.CardFiles
{
    public class EFCardAccess : File
    {
        public PaceInfo PaceInfo { get; private set; }
        public CAInfo CAInfo { get; private set; }
        public CADomainParameterInfo CADomainParameters { get; private set; }
        public TAInfo TAInfo { get; private set; }

        public EFCardAccess(byte[] data) : base(data, "EF.EF_CardAccess")
        {
        }

        public override bool Parse()
        {
            if (Bytes == null)
                return false;

            foreach (DerSequence securityInfo in (Asn1Set)Asn1Object.FromByteArray(Bytes))
            {
                var info = InfoFactory.Parse(securityInfo);

                switch (info)
                {
                    case TAInfo ta:
                        TAInfo = ta;
                        break;
                    case CAInfo ca:
                        CAInfo = ca;
                        break;
                    case PaceInfo pi:
                        PaceInfo = pi;
                        break;
                    case CADomainParameterInfo d:
                        CADomainParameters = d;
                        break;
                }
            }

            return true;
        }
    }
}
