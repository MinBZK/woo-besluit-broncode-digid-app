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
﻿using System.Security;
using DigiD.Common.EID.Models;
using DigiD.Common.Enums;
using DigiD.Common.Models.ResponseModels;

namespace DigiD.Common.Models
{
    public class PinCodeModel
    {
        public PinEntryType EntryType { get; set; }
        public WebSessionInfoResponse SessionInfo { get; set; }
        public WidSessionResponse EIDSessionResponse { get; }
        public int? TriesLeft { get; }
        public bool WrongCAN { get; set; }
        public Card Card { get; }
        public bool IsStatusChecked { get; set; }
        public bool IsWIDActivation { get; set; }
        public SecureString PIN { get; set; }
        public string IV { get; set; }
        public bool NeedPinChange { get; set; }
        public bool ChangeAppPin { get; set; }
        public bool IsAppAuthentication { get; set; }
        
        public PinCodeModel(PinEntryType entryType, Card card = null, WidSessionResponse eidSessionResponse = null, int? triesLeft = null)
        {
            EntryType = entryType;
            EIDSessionResponse = eidSessionResponse;
            TriesLeft = triesLeft;
            Card = card;
        }
    }
}
