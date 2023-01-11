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
﻿namespace DigiD.Common.EID.Constants
{
    public static class WidConstants
    {
        public const string RDW_GET_CERTIFICATE_URI = "SSSSSSSSSSSSSSSSSS";
        public const string RDW_POLYMORPHIC_INFO_URI = "SSSSSSSSSSSSSSSSSS";
        public const string RDW_SIGN_CHALLENGE_URI = "SSSSSSSSSSSSS";
        public const string RDW_SECURE_APDU_URI = "SSSSSSSSSSS";
        public const string RDW_GET_POLYMORPHIC_DATA_URI = "SSSSSSSSSSSSSSSSSS";

        public const string NIK_PREPARE_EAC_URI = "SSSSSSSSSSSSSSSSSS";
        public const string NIK_START_SESSION = "SSSSSSSSSSSSS";
        public const string NIK_POLYMORPHIC_DATA_URI = "SSSSSSSSSSSSSSSSSSSSSS";
        public const string NIK_PREPARE_PCA = "SSSSSSSSSSSSSSSSSS";

        public const int PINTries = 5;
        public static string CAN_NIK = "SSSSSS";
        public static string CAN_RDW = "SSSSSSSSSS";
    }
}
