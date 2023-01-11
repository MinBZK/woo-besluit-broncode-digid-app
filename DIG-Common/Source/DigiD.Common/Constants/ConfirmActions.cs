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
    public static class ConfirmActions
    {
        public const string ChangeWIDPIN = "change_wid_pin";
        public const string UpgradeToSubWhileSub = "upgrade_to_sub_while_sub";
        public const string IDcheckStart = "id_check_start";
        public const string IDcheckStartFromMenu = "id_check_start_from_menu";
        public const string IDCheckPostpone = "id_check_postpone";
        public const string IDCheckPostponeAuthentication = "id_check_postpone_authentication";
        public const string IDCheckPostponeSMSActivation = "id_check_postpone_sms";
        public const string IDCheckPostponeAppActivation = "id_check_postpone_app_letter";
        public const string ActivateViaRequestStationStart = "activate_app_via_request_station_start";
        public const string ActivateViaRequestStationCancel = "activate_app_via_request_station_cancel";
        public const string RequestPushNotificationPermission = "request_pushnotification_permission";
        public const string RegisterEmailAddress = "register_email_address";
        public const string RdaNoDocumentsFound = "rda_no_document_found";
        public const string ConfirmEmailAddress = "confirm_email_address";
        public const string ConfirmExistingEmailAddress = "confirm_existing_email_address";
        
        //Request DigiD account vanuit app:
        public const string RequestAccountExistingApp = "request_account_existing_request";
        public const string RequestAccountExistingAccount = "request_account_existing_account";
    }
}
