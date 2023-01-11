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
using DigiD.Common.Http.Models;
using DigiD.Common.Models.RequestModels;
using Newtonsoft.Json;

namespace DigiD.Common.Models
{
    public class UsageHistoryRequestModel : BaseMijnDigiDRequest
    {
        [JsonProperty("page_id")]
        public int PageId { get; set; }
        
        [JsonProperty("language")]
        public string Language { get; set; }
    }

    public class UsageHistoryResponseModel : BaseResponse
    {
        [JsonProperty("total_items")]
        public int TotalItems { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("logs")]
        public List<UsageHistoryModel> Items { get; set; }
    }

    public class UsageHistoryModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime DateTime { get; set; }
        
        [JsonProperty("name")]
        public string Message { get; set; }
    }
}
