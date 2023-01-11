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
﻿namespace DigiD.Common.Constants
{
    public static class AuthenticationActions
    {
        public const string ChangePassword = "change_password";
        public const string ActivationByLetter = "activation_by_letter";
        public const string ChangePhoneNumber = "change_phone_number";
        public const string ChangeAppPIN = "change_app_pin";
        public const string DeactivateApp = "deactivate_app";
        public const string ConfirmWebserviceUpgrade = "confirm_webservice_upgrade";
        public const string ConfirmWebserviceUpgradeNoNfc = "confirm_webservice_upgrade_no_nfc";

        public const string ActivateSMSAuthentication = "activate_sms_authenticator";
        public const string UpgradeWithWIDChecker = "upgrade_rda_widchecker";
        public const string ReRequestLetter = "re_request_letter";
        public const string RDAUpgrade = "upgrade_app";
        public const string LetterNotification = "letter_notification";
        public const string RequestSession = "new_session";

        public const string ACTIVATE_DRIVING_LICENSE = "activate_driving_licence";
        public const string ACTIVATE_IDENTITY_CARD = "activate_identity_card";
        public const string REACTIVATE_DRIVING_LICENSE = "reactivate_driving_license";
        public const string REACTIVATE_IDENTITY_CARD = "reactivate_identity_card";
        public const string REVOKE_DRIVING_LICENSE = "revoke_driving_license";
        public const string REVOKE_IDENTITY_CARD = "revoke_identity_card";

        public const string EMAIL_ADD = "add_email";
        public const string EMAIL_CHANGE = "change_email";
        public const string EMAIL_REMOVE = "remove_email";

        public const string ACTIVATE_WITH_APP = "activate_with_app";
        public const string CANCEL_ACCOUNT = "cancel_account";
    }
}
