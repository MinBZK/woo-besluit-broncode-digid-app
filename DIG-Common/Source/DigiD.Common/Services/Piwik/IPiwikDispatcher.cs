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
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DigiD.Common.Models.Piwik;

namespace DigiD.Common.Services.Piwik
{
    /// <summary>
    /// Common interface to send the <see cref="PiwikEvent">events</see> to the Piwik site.  
    /// </summary>
    public interface IPiwikDispatcher
    {
        /// <summary>
        /// The base PIWIK url including piwik.php and starting with 
        /// </summary>
        /// <value>The base URL.</value>
        string BaseUrl { get; set; }

        /// <summary>
        /// Override the user agent, we moeten een App UserAgent faken
        /// </summary>
        /// <value>The user agent.</value>
        string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the size of the screen.
        /// </summary>
        /// <value>The size of the screen.</value>
        string ScreenSize { get; set; }

        /// <summary>
        /// Send a single event.
        /// </summary>
        /// <returns>True if the event is sent.</returns>
        /// <param name="singlePiwikEvent">Single event.</param>
        Task<bool> Send(PiwikEvent singlePiwikEvent);

        /// <summary>
        /// Send the specified list of events.
        /// </summary>
        /// <returns>True if the events are sent.</returns>
        /// <param name="events">Events. <see cref="PiwikEvent"/> </param>
        Task<bool> Send(IEnumerable<PiwikEvent> events);
    }
}
