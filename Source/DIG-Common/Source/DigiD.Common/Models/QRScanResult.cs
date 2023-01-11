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
using DigiD.Common.Constants;

namespace DigiD.Common.Models
{
    public class QRScanResult
    {
        public string Identifier { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }

    public class SessionData
    {
        public string Identifier { get; set; }
        public string Host { get; set; }
        public string WebSessionId { get; set; }
        public string verification_code { get; set; }
        public string source { get; set; }
        public string action { get; set; }
    }

    public class App2AppSessionData : SessionData
    {
        public App2AppRequest Data { get; }

        public App2AppSessionData(App2AppRequest data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Host = data.Host;
            Identifier = QRCodeIdentifierConstants.Authentication;
        }
    }
}

