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
