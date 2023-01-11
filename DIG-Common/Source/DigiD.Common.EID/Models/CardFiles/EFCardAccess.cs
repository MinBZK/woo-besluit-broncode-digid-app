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
