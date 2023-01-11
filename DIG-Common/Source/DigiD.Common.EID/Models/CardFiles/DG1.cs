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
using System.Text;
using BerTlv;
using DigiD.Common.EID.Helpers;

namespace DigiD.Common.EID.Models.CardFiles
{
    public class DG1 : File
    {
        public string MRZ { get; private set; }
        public DG1(byte[] data) : base(data, "DG1")
        {
            
        }

        public override bool Parse()
        {
            var tlv = Tlv.ParseTlv(Bytes);

            if (tlv.Count == 1)
            {
                var mrzData = tlv.First().FindTag("5F1F");
                var dlData = tlv.First().FindTag("5F02");

                if (mrzData != null)
                    MRZ = Encoding.ASCII.GetString(mrzData.Value);
                else
                    MRZ = Encoding.ASCII.GetString(dlData.Value);

                return true;
            }

            return false;
        }
    }
}
