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
