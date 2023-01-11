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
