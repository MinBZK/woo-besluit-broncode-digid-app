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
