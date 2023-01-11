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
