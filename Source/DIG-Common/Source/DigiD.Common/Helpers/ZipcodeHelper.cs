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
