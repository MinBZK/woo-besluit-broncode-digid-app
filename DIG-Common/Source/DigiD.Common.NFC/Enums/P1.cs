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
﻿namespace DigiD.Common.NFC.Enums
{
    public enum P1
    {
        SELECT_MF = 0x00,
        CHILD_DF = 0x01,
        CHILD_EF = 0x02,
        PARENT_DF = 0X03,
        APPLICATION_ID = 0x04,
        ABS_PATH = 0x08,
        REL_PATH = 0x09,
        COMPUTE_DIGITAL_SIGNATURE = 0x41,
        PUT_HASH = 0xA0,
        PERFORM_SECURITY_OPERATION = 0xC1,
        SET_DST = 0x81,
        ERASE = 0xF3,
        DECRYPT = 0x80,
        ENCRYPT = 0x86,
        SIGN_HASH = 0x9E,
    }
}
