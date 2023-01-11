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
using System.Globalization;
using System.Linq;

namespace DigiD.Common.NFC.Helpers
{
    public static class ConvertHelpers
    {
        public static int ConvertHexToInt(this byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ConvertHexToInt(BitConverter.ToString(data));
        }
        
        public static int ConvertHexToInt(this string hexValue)
        {
            if (hexValue == null)
                throw new ArgumentNullException(nameof(hexValue));

            return int.Parse(hexValue.Replace("-",string.Empty), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        public static byte[] ConvertHexToBytes(this string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return new byte[]{ 0x00 };

            hexString = hexString.Replace("-", string.Empty);

            if (hexString.Length % 2 != 0)
                hexString = "0" + hexString;

            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
        }
    }
}
