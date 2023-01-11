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
using BerTlv;
using DigiD.Common.EID.Helpers;

namespace DigiD.Common.EID.Models.CardFiles
{
    public class PhotoFile : File
    {
        public byte[] Photo { get; private set; }

        public PhotoFile(byte[] data, string name) : base(data, name)
        {
        }

        public override bool Parse()
        {
            var tlvs = Tlv.ParseTlv(Bytes);

            if (tlvs.Count == 1)
            {
                var photo = tlvs.First().FindTag("7F2E", "5F2E");

                if (photo != null)
                {
                    Photo = photo.Data.Skip(51).ToArray();
                }

                return true;
            }

            return false;
        }
    }

    public class DG2 : PhotoFile
    {
        public DG2(byte[] data) : base(data, "DG2")
        {

        }
    }

    public class DG6 : PhotoFile
    {
        public DG6(byte[] data) : base(data, "DG6")
        {

        }
    }
}
