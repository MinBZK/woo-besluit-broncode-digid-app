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
    public class RequestAccountRequest : BaseRequest
    {
        [JsonProperty("BSN")]
        public string Bsn { get; set; }

        [JsonProperty("date_of_birth")] // Format: "JJJMMDD"
        public string DateOfBirth { get; set; }

        [JsonProperty("postal_code")]  //Format: 9999AA
        public string Postalcode { get; set; }

        [JsonProperty("house_number")]
        public string HouseNumber { get; set; }

        [JsonProperty("house_number_additions")]
        public string HouseNumberSuffix { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("nfc_support")]
        public bool HasNfcSupport { get; set; }
    }
}
