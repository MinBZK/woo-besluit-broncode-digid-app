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
﻿using DigiD.Common.SessionModels;
using Newtonsoft.Json;

namespace DigiD.Common.Models.RequestModels
{
    public class BaseRequest 
    {
        [JsonProperty("auth_session_id")] public string AuthenticationSessionId { get; protected set; } = AppSession.AuthenticationSessionId;

        /// <summary>
        /// In geval van bevestiging van inloggen bij een
        /// webdienst, van inloggen in een afnemer app of
        /// van een Mijn DigiD actie: De app heeft naast het
        /// app_sessie_id ook een (nog te authenticeren)
        /// web_sessie_id (ontvangen via QR-code, via
        /// universal of app link, of als antwoord op het
        /// SAML_authenticatieverzoek bij app2app). De
        /// app vraagt de gebruiker om bevestiging van het
        /// inloggen en stuurt die bevestiging met beide
        /// sessie_id's door naar Kern.
        /// </summary>
        [JsonProperty("app_session_id")] public string AppSessionId { get; } = HttpSession.AppSessionId;
    }
}
