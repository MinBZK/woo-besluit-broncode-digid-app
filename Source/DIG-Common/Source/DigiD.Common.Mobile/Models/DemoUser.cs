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
