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
﻿namespace DigiD.Common.Services.Piwik
{
    /// <summary>
    /// Device information used by PIWIK.
    /// </summary>
    public class Device
    {
        public Device()
        {
            OsVersion = Xamarin.Essentials.DeviceInfo.VersionString;
            Platform = Xamarin.Essentials.DeviceInfo.Manufacturer + " " + Xamarin.Essentials.DeviceInfo.Model;
            HumanReadablePlatformName = Xamarin.Essentials.DeviceInfo.Model;
        }
        /// <summary>
        /// Platform (device hardware model)
        /// </summary>
        /// <value>The platform.</value>
        public string Platform { get; set; }

        /// <summary>
        /// Leesbare naam van het toestel.
        /// </summary>
        /// <value>The name of the human readable platform.</value>
        public string HumanReadablePlatformName { get; set; }

        /// <summary>
        /// Versie van het systeem.
        /// </summary>
        /// <value>The os version.</value>
        public string OsVersion { get; set; }
        /// <summary>
        /// Screen size as string: hxw
        /// </summary>
        /// <value>The size of the screen.</value>
        public string ScreenSize { get; set; }

        /// <summary>
        /// Screen size in native units
        /// </summary>
        /// <value>The size of the native screen.</value>
        public string NativeScreenSize { get; set; }

        /// <summary>
        /// The user agent to report to PIWIK.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }
    }
}
