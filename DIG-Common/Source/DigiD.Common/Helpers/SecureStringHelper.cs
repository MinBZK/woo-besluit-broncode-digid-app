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
using System.Globalization;
using System.Linq;
using System.Security;
using DigiD.Common.EID.Helpers;

namespace DigiD.Common.Helpers
{
    public static class SecureStringHelper
    {
        public static SecureString ToSecureString(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var result = new SecureString();
            foreach (var c in value)
                result.AppendChar(c);

            return result;
        }

        public static bool IsIncreasing(this SecureString s)
        {
            return "01234567890".Contains(s.ToPlain());
        }

        public static bool IsDecreasing(this SecureString s)
        {
            return "09876543210".Contains(s.ToPlain());
        }

        public static bool AreEqual(this SecureString s)
        {
            return s.ToPlain().Distinct().Count() == 1;
        }

        public static void AppendChar(this SecureString secureString, int value)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            foreach (var c in value.ToString(CultureInfo.InvariantCulture))
                secureString.AppendChar(c);
        }

        public static void RemoveChar(this SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            secureString.RemoveAt(secureString.Length - 1);
        }
    }
}
