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
﻿namespace DigiD.Common.Mobile.Models
{
    public class DemoUser
    {
        public int UserId { get; set; }
        public string Bsn { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ActivationMethod { get; set; }
        public string HouseNumber { get; set; }
        public string Postalcode { get; set; }
        public bool HasNoDocuments { get; set; }
        public string DocumentNumber { get; set; }
        public bool RdaFailed { get; set; }
        public string EmailAddress { get; set; }
        public bool UserActionNeeded { get; set; }
        public bool IsEmailAddressVerified { get; set; } = true;
        public bool HasMessages { get; set; } = true;
        public bool IsSmsCheckRequested { get; set; }
        public bool EIDASUser { get; set; }
        public bool EIDASSuccess { get; set; } = true;
        public bool TwoFactorEnabled { get; set; }
    }
}
