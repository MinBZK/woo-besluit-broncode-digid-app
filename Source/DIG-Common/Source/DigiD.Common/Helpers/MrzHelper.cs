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
