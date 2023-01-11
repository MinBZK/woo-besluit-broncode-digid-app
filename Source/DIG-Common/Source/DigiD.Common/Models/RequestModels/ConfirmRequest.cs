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
﻿using Newtonsoft.Json;

namespace DigiD.Common.Models.RequestModels
{
    public class ConfirmRequest : BaseRequest
    {
        [JsonProperty("user_app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// Indien de app in het koppelvlak aanvragen_gegevens_web_sessie
        /// een hashed_pip ontvangen heeft, stuurt de app hier de met de
        /// private key gemaakte handtekening van de hashed_pip terug naar DigiD Kern.
        /// Van toepassing bij webdiensten die een vi en vp verwachten (o.a. eIDAS-Uit).
        /// </summary>
        [JsonProperty("signature_of_pip")]
        public string SignatureOfPIP { get; set; }
    }
}
