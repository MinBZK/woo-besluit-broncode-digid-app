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
    public enum INS : byte
    {
        EXTERNAL_AUTHENTICATE = 0x82,
        CHALLENGE = 0x84,
        GENERAL_AUTH = 0x86,
        PERFORM_SEC_OP = 0x2A,
        MANAGE_SEC_ENV = 0x22,
        SELECT_FILE = 0xA4,
        READ_FILE = 0xB0,
        SHORT_FILE_ACCESS = 0xB1,
        VERIFY = 0x20,
        RESET_RETRY_COUNTER = 0x2c,
        CHANGE_REFERENCE_DATA = 0x24,
        EMPTY = 0x00

    }
}
