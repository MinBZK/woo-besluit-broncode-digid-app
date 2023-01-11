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
