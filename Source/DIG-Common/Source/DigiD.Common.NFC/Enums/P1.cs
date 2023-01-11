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
