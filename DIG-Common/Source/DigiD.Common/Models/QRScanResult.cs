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

