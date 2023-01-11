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
using DigiD.Common.Enums;
using DigiD.Common.Http.Models;
using DigiD.Common.SessionModels;
using Newtonsoft.Json;

namespace DigiD.Common.Models.ResponseModels
{
    public class AccountStatusResponse : BaseResponse
    {
        [JsonProperty("email_status")]
        public EmailStatus EmailStatus { get; set; }

        private int _unreadMessagesCount;
        
        [JsonProperty("unread_notification_count")]
        public int UnreadMessagesCount
        {
            get => _unreadMessagesCount;
            set
            {
                var changed = _unreadMessagesCount != value;
                _unreadMessagesCount = value;

                if (changed)
                    AppSession.MenuItemChange();
            }
        }

        public bool HasUnreadMessages => UnreadMessagesCount > 0;

        [JsonProperty("setting_2_factor")]
        public bool TwoFactorEnabled { get; set; }
        
        [JsonProperty("classified_deceased")]
        public bool ClassifiedDeceased { get; set; }

        [JsonProperty("current_email_address")]
        public string CurrentEmailAddress { get; set; }

        public List<AccountTask> OpenTasks { get; } = new List<AccountTask>();
    }
}
