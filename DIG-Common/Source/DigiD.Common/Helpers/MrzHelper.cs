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

namespace DigiD.Common.Helpers
{
    public static class MrzHelper
    {
        public static string GetMrzInfo(string documentNumber, string dateOfBirth, string expiryDate)
        {
            var part1 = CalculateCheckDigit(documentNumber);
            var part2 = CalculateCheckDigit(dateOfBirth);
            var part3 = CalculateCheckDigit(expiryDate);

            return part1 + part2 + part3;
        }

        /// <summary>
        /// https://help.microblink.com/hc/en-us/articles/360007487833-MRTD-MRZ-FAQ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string CalculateCheckDigit(string data)
        {
            var weighting = new[] { 7, 3, 1 };
            var index = 0;
            var sum = 0;

            foreach (var value in data.Select(c => char.IsNumber(c) ? (int) char.GetNumericValue(c) : c - 55))
            {
                if (index == 3)
                    index = 0;

                sum += value * weighting[index];
                index++;
            }

            var checkDigit = sum % 10;

            return data + checkDigit;
        }
    }
}
