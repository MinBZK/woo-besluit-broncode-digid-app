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
    public enum P2
    {
        FCP = 0x04,
        FMD = 0x08,
        NONE = 0x0C,
        SET_AT = 0xA4,
        HASH_ALGORITHM = 0xAA,
        COMPUTE_DIGITAL_SIGNATURE = 0xB6,
        ENCRYPT_OPERATION = 0xB8,
        DEFAULT_CHANNEL = 0x00,
        UNCRYPT_DATA = 0x80,
        ENCRYPT_DATA = 0x86,
        HASH_VALUE = 0x9A,
        CERTIFICATE = 0xBE
    }
}
