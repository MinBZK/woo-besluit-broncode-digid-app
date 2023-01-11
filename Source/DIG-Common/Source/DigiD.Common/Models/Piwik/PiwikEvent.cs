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
