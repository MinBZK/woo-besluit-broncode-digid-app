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
using Org.BouncyCastle.Math;

namespace DigiD.Common.EID.Models
{
    public class SMContext
    {
        internal bool IsBAC => _ssc.Length == 8;
        internal byte[] KEnc { get; private set; }
        internal byte[] KMac { get; private set; }

        //Session identifier
        private byte[] _ssc;

        internal byte[] IncrementSSC()
        {
            var increment = new BigInteger("1");
            var newSsc = new BigInteger(_ssc).Add(increment).ToByteArray();
            Buffer.BlockCopy(newSsc, 0, _ssc, _ssc.Length - newSsc.Length, newSsc.Length);
            return _ssc;
        }

        public SMContext()
        {
        }

        public SMContext(byte[] kEnc, byte[] kMac, byte[] ssc)
        {
            KEnc = kEnc;
            KMac = kMac;
            _ssc = ssc;
        }

        internal void Init(byte[] kEnc, byte[] kMac)
        {
            KEnc = kEnc;
            KMac = kMac;
            _ssc = new byte[16];
        }
    }
}
