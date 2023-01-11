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
using DigiD.Common.Interfaces;
using Xamarin.Forms;

namespace DigiD.Common.Models.Piwik
{
    /// <summary>
    /// Session properties. Class internally used by Piwik, no need to use this class in your code.
    /// </summary>
    public class Session
    {
        public int SessionsCount { get; set; }
        public DateTime LastVisit { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime CurrentVisit { get; set; }

        public Session()
        {
            DependencyService.Get<IPiwikSettings>().SessionsCount += 1;

            FirstVisit = DependencyService.Get<IPiwikSettings>().FirstVisit;
            LastVisit = DependencyService.Get<IPiwikSettings>().LastVisit;
            CurrentVisit = DateTime.Now;

            DependencyService.Get<IPiwikSettings>().Save();
        }
    }
}
