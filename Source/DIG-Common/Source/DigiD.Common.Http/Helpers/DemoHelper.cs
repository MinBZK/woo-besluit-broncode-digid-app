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

namespace DigiD.Common.Http.Helpers
{
    public static class DemoHelper
    {
        public static Dictionary<string, DemoSession> Sessions { get; } = new Dictionary<string, DemoSession>();

        public static DemoSession GetSession(string sessionId)
        {
            return string.IsNullOrEmpty(sessionId) ? null : Sessions[sessionId];
        }
    }

    public class DemoSession
    {
        public string IV { get; set; }
        public string Action { get; set; }
        public string Challenge { get; set; }
        public DateTime? DateTime { get; set; }
        public bool IsAuthenticated { get; set; }
        public int LoginLevel { get; set; }
    }
}
