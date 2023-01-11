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
﻿using System;
using System.Linq;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.SecurityInfoObjects;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Helpers
{
    internal static partial class CommandApduBuilder
    {
        internal static CommandApdu GetMSESetATCommand(PaceInfo paceInfo, Certificate certificate, PasswordType passwordType, SMContext context)
        {
            var cryptographicMechanism = new DataObject(new byte[] { 0x80 }, paceInfo.OID.GetEncoded().Skip(2).ToArray()).GetEncoded();
            var password = new DataObject(new byte[] { 0x83 }, new[] { (byte)passwordType }).GetEncoded();

            byte[] data;

            if (passwordType == PasswordType.PIN && certificate != null)
            {
                var oid = certificate.ChatOID.GetEncoded();
                var cdd = certificate.ChatDiscretionaryData.GetEncoded();

                var refDomainParams = new DataObject(new byte[] { 0x84 }, paceInfo.ParameterId.Value.ToByteArray()).GetEncoded();

                var chat = new DataObject(new byte[] { 0x7F, 0x4C }, oid.Concat(cdd).ToArray()).GetEncoded();
                data = cryptographicMechanism.Concat(password).Concat(chat).Concat(refDomainParams).ToArray();

                var data65 = certificate.CertRelativeAuth.GetEncoded();
                data = data.Concat(data65).ToArray();
            }
            else
                data = cryptographicMechanism.Concat(password).ToArray();
            
            var command = new CommandApdu(CLA.PLAIN, INS.MANAGE_SEC_ENV, (int)P1.PERFORM_SECURITY_OPERATION, (int)P2.SET_AT, data);

            if (context != null)
                return command.Encrypt(context);

            return command;
        }

        internal static CommandApdu GetNonceCommand(SMContext context)
        {
            var data = new DataObject(new byte[] { 0x7C }, Array.Empty<byte>());
            var command = new CommandApdu(CLA.COMMAND_CHAINING, INS.GENERAL_AUTH, (int)P1.SELECT_MF, (int)P2.DEFAULT_CHANNEL, data.GetEncoded(), 0);
            return context != null ? command.Encrypt(context, true) : command;
        }

        internal static CommandApdu GetMapCommand(byte[] x, byte[] y, SMContext context)
        {
            var coordinates = new byte[] { 0X04 }.Concat(x).Concat(y).ToArray();
            var mappingData = new DataObject(new byte[] { 0x81 }, coordinates);
            var dynamicAuthData = new DataObject(new byte[] { 0x7C }, mappingData);

            var command = new CommandApdu(CLA.COMMAND_CHAINING, INS.GENERAL_AUTH, 0, 0, dynamicAuthData.GetEncoded(), 0);
            return context != null ? command.Encrypt(context, true) : command;
        }

        internal static CommandApdu GetPairAgreementCommand(byte[] x, byte[] y, SMContext context)
        {
            var coordinates = new byte[] { 0X04 }.Concat(x).Concat(y).ToArray();
            var mappingData = new DataObject(new byte[] { 0x83 }, coordinates);
            var dynamicAuthData = new DataObject(new byte[] { 0x7C }, mappingData);

            var command = new CommandApdu(CLA.COMMAND_CHAINING, INS.GENERAL_AUTH, 0, 0, dynamicAuthData.GetEncoded(), 0);
            return context != null ? command.Encrypt(context, true) : command;
        }

        internal static CommandApdu GetAuthCommand(byte[] terminalKey, SMContext context)
        {
            var mac = new DataObject(new byte[] { 0x85 }, terminalKey);
            var dynamicAuthenticationData = new DataObject(new byte[] { 0x7c }, mac);
            var command = new CommandApdu(CLA.PLAIN, INS.GENERAL_AUTH, 0, 0, dynamicAuthenticationData.GetEncoded(), 0);
            return context != null ? command.Encrypt(context) : command;
        }
    }
}
