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
using DigiD.Common.Http.Models;
using Newtonsoft.Json;

namespace DigiD.Common.Models.ResponseModels
{
    public class WebSessionInfoResponse : BaseResponse
    {
        /// <summary>
        /// het door de webdienst vereiste inlogniveau 
        /// </summary>
        [JsonProperty("authentication_level")]
        public int AuthenticationLevel { get; set; }

        /// <summary>
        /// de hash van de pip die de app moet ondertekenen om uiteindelijk een versleutelde identiteit en versleuteld pseudoniem te kunnen maken, bij inloggen bij een webdienst die een vi en vp verwacht (o.a. eIDASUit).
        /// </summary>
        [JsonProperty("hashed_pip")]
        public string HashedPIP { get; set; }
        
        /// <summary>
        /// het binnenkort door de webdienst vereiste niveau (de waarde van 'Naar zekerheidsniveau' bij beheer webdienst
        /// </summary>
        [JsonProperty("new_authentication_level")]
        public int NewAuthenticationLevel { get; set; }
        
        /// <summary>
        /// de datum waarop het nieuwe inlogniveau ingaat (de waarde van 'Ingangsdatum' bij beheer webdienst)
        /// </summary>
        [JsonProperty("new_level_start_date")] 
        public DateTimeOffset? NewAuthenticationLevelStartDate { get; set; }
        
        /// <summary>
        /// naam van de webdienst waar gebruiker wil inloggen 
        /// </summary>
        [JsonProperty("webservice")]
        public string WebService { get; set; }
        
        /// <summary>
        /// de uit te voeren actie, bijvoorbeeld
        /// wachtwoord of telefoonnummer wijzigen,
        /// e-mailadres toevoegen, e-mailadres wijzigen,
        /// e-mailadres verwijderen, deactiveren DigiD app,
        /// sms-controle activeren of opheffen DigiD.
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("return_url")]
        public string ReturnURL { get; set; }

        [JsonProperty("icon_uri")]
        public string IconUri { get; set; }

        [JsonProperty("oidc_session")]
        public bool IsOidcSession { get; set; }
        
        public bool IsWebservice => !string.IsNullOrEmpty(WebService);
        public bool IsConfirmAction => !string.IsNullOrEmpty(Action);
    }
}
