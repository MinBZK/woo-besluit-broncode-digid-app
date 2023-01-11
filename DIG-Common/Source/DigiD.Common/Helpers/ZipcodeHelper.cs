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
﻿using System.Text.RegularExpressions;

namespace DigiD.Common.Helpers
{
    public static class ZipcodeHelper
    {
        /// <summary>
        /// Checks if a given string is a valid Postalcode. Based on the dutch postalcode.
        /// </summary>
        /// <param name="zipcode">The <see cref="string"/> to check</param>
        /// <returns>A <see cref="bool"/> indicating whether or not the value is a valid Postalcode</returns>
        public static bool IsValidZipcode(this string zipcode)
        {
            string POSTCODE_EXPRESSION = "^[1-9][0-9]{3}[A-Za-z]{2}$";
            Regex regex = new Regex(POSTCODE_EXPRESSION, RegexOptions.Compiled | RegexOptions.Singleline);

            if (string.IsNullOrEmpty(zipcode))
                return false;

            var match = regex.Match(zipcode);
            return match.Success;
        }
    }
}
