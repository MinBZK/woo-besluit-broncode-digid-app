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
﻿using System.Collections.Generic;
using DigiD.Common.Http.Models;
using Newtonsoft.Json;

namespace DigiD.Common.Models.ResponseModels
{
    public class InitApp2AppResponse : BaseResponse
    {
        [JsonProperty("app_session_id")]
        public string WebSessionId { get; set; }

        [JsonProperty("authentication_level")]
        public int AuthenticationLevel { get; set; }

        [JsonProperty("image_domain")]
        public string ImagedDomain { get; set; }

        [JsonProperty("apps")] public List<App> Apps { get; set; } = new List<App>();

        public string SAMLart { get; set; }

        public string RelayState { get; set; }
    }
}
