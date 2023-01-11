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
﻿using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Helpers
{
    internal static partial class CommandApduBuilder
    {
        internal static CommandApdu GetMSESetATCAv1Command(byte[] caOID, SMContext context)
        {
            var data80 = new DataObject(new byte[] { 0x80 }, caOID).GetEncoded();

            var command = new CommandApdu(CLA.PLAIN, INS.MANAGE_SEC_ENV, (int)P1.COMPUTE_DIGITAL_SIGNATURE, (int)P2.SET_AT, data80);
            return command.Encrypt(context);
        }

        internal static CommandApdu GetGeneralAuthenticateCAv1(byte[] publicKey, SMContext context)
        {
            var data80 = new DataObject(new byte[] { 0x80 }, publicKey);
            var data7C = new DataObject(new byte[] { 0x7c }, data80);

            var command = new CommandApdu(CLA.PLAIN, INS.GENERAL_AUTH, 0, 0, data7C.GetEncoded(), 0x00);

            return command.Encrypt(context);
        }
    }
}
