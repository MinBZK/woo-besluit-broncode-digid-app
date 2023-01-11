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
using System.Text;

namespace DigiD.Common.NFC.Helpers
{
    public static class StringHelper
    {
        public static string ToHexString(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return BitConverter.ToString(data);
        }

        public static string FromBCD(this byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var temp = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
            {
                temp.Append((byte)((t & 0xf0) >> 4));
                temp.Append((byte)(t & 0x0f));
            }

            return temp.ToString();
        }

        public static string ToBase64(this byte[] ba)
        {
            if (ba == null)
                return string.Empty;

            return Convert.ToBase64String(ba);
        }

        public static string ToBase16(this byte[] ba)
        {
            if (ba == null)
                return string.Empty;

            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "").ToLowerInvariant();
        }

        public static byte[] HexToByteArray(this string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));

            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
