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
