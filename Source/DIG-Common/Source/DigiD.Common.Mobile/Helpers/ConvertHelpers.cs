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
using DigiD.Common.Enums;

namespace DigiD.Common.Mobile.Helpers
{
    public static class ConvertHelpers
    {
        public static LoginLevel ToLoginLevel(this int value)
        {
            switch (value)
            {
                case 10:
                    return LoginLevel.Basis;
                case 20:
                    return LoginLevel.Midden;
                case 25:
                    return LoginLevel.Substantieel;
                case 30:
                    return LoginLevel.Hoog;
                default:
                    return LoginLevel.Unknown;
            }
        }

        public static int ToInt(this LoginLevel value)
        {
            switch (value)
            {
                case LoginLevel.Basis:
                    return 10;
                case LoginLevel.Midden:
                    return 20;
                case LoginLevel.Substantieel:
                    return 25;
                case LoginLevel.Hoog:
                    return 30;
                default:
                    return 20;
            }
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTimeOffset date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static long ToUnixTimeMs(this DateTimeOffset date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }
    }
}
