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
using System.Collections.Generic;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Common.Models.Piwik
{
    /// <summary>
    /// Class met alle gegevens voor een Piwik event.
    /// </summary>
    public class PiwikEvent
    {
        public string SiteID { get; set; }
        public Guid Uuid { get; set; }
        public Session Session { get; set; }

        public DateTime Date { get; set; }
        public string Url { get; set; }
        public string[] ActionName { get; set; }
        public string Language { get; set; }
        public bool IsNewSession { get; set; }
        public string EventCategory { get; set; }
        public string EventAction { get; set; }
        public string EventName { get; set; }
        public string EventValue { get; set; }

        public Dictionary<string, string[]> PageCustomVar { get; } = new Dictionary<string, string[]>();

        public PiwikEvent()
        {
            Date = DateTime.Now;
            Uuid = Guid.NewGuid();
            Url = $"https://{DependencyService.Get<IGeneralPreferences>().SelectedHost}/";
            
            if (HttpSession.IsApp2AppSession)
                PageCustomVar.Add("1", new[] {"App2App", "1"});
        }
    }
}
