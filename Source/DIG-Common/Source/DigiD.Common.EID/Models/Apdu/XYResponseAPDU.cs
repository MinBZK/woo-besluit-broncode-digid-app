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
using System.Linq;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Models.Apdu
{
    public class XYResponseAPDU
    {
        public bool UncompressedPoint { get; set; }

        public byte[] EncodedPoint { get; set; }
        public byte[] PointX { get; set; }
        public byte[] PointY { get; set; }
        public XYResponseAPDU(ResponseApdu apdu)
        {
            if (apdu == null)
                throw new ArgumentNullException(nameof(apdu));

            var data = apdu.Data;
            UncompressedPoint = data[4] == 0x04;

            EncodedPoint = data.Skip(4).ToArray();

            var points = data.Skip(5).ToArray();

            PointX = points.Take(points.Length / 2).ToArray();
            PointY = points.Skip(points.Length / 2).ToArray();
        }
    }
}
