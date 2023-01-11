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
using System.Security;
using System.Threading.Tasks;
using DigiD.Common.EID.Enums;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Helpers
{
    internal static partial class CommandApduBuilder
    {
        internal static async Task<ResponseApdu> SendAPDU(string header, CommandApdu command, SMContext context, bool autoDecrypt = true, bool alwaysDecrypt = false)
        {
            Debugger.DumpInfo(header, command);
            var result = await EIDSession.NfcService.SendApduAsync(command.Bytes);

            if (!result.Success)
                throw result.Exception;

            var response = new ResponseApdu(result.Data);

            if (autoDecrypt && (IsSuccessOrWarning() || alwaysDecrypt) && context != null && !context.IsBAC)
                response = response.DecryptAES(context);

            Debugger.DumpInfo(header, response);

            return response;

            bool IsSuccessOrWarning()
            {
                return response.SW1 == 0x90 || (response.SW1 == 0x63 && response.SW2 >= 0xc1 && response.SW2 <= 0xcf);
            }
        }

        internal static bool ValidateAPDU(byte[] bytes, int index)
        {
            var apdu = new CommandApdu(bytes);
            switch (index)
            {
                case 0: //Select PCA
                    {
                        return apdu.CLA == CLA.SECURE_MESSAGING && apdu.INS == INS.SELECT_FILE && apdu.P1 == (int)P1.APPLICATION_ID && apdu.P2 == (int)P2.NONE;
                    }
                case 1: //Set AT
                    {
                        return apdu.CLA == CLA.SECURE_MESSAGING && apdu.INS == INS.MANAGE_SEC_ENV && apdu.P1 == (int)P1.COMPUTE_DIGITAL_SIGNATURE && apdu.P2 == (int)P2.SET_AT;
                    }
                case 2: //General Authenticate
                    {
                        return apdu.CLA == CLA.SECURE_MESSAGING && apdu.INS == INS.GENERAL_AUTH && apdu.P1 == 0 && apdu.P2 == 0;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        internal static CommandApdu GetMFSelectCommand(SMContext context, bool enik)
        {
            var command = new CommandApdu(CLA.PLAIN, INS.SELECT_FILE, 0x00, enik ? 0x0C : 0x00, null, 0);
            return context != null ? command.Encrypt(context) : command;
        }

        internal static CommandApdu GetSelectFileCommand(CardFile file, SMContext context = null)
        {
            var command = new CommandApdu(CLA.PLAIN, INS.SELECT_FILE, (byte)file.Type, (byte)P2.NONE, file.Id);
            return context != null ? command.Encrypt(context) : command;
        }

        internal static CommandApdu ReadSelectedFileCommand(int offset, SMContext context)
        {
            var command = new CommandApdu(CLA.PLAIN, INS.READ_FILE, (offset & 0xff00) >> 8, offset & 0xff, Array.Empty<byte>(), context != null ? 0xE7 : 0xFF);
            return context != null ? command.Encrypt(context) : command;
        }

        internal static CommandApdu GetSelectApplicationCommand(byte[] aid, SMContext context)
        {
            var cmd = new CommandApdu(CLA.PLAIN, INS.SELECT_FILE, (int)P1.APPLICATION_ID, (int)P2.NONE, aid);
            return context != null ? cmd.Encrypt(context) : cmd;
        }

        internal static CommandApdu GetResetRetryCounterCommand(SecureString pin, SMContext context)
        {
            return new CommandApdu(CLA.PLAIN, INS.RESET_RETRY_COUNTER, (int)P1.CHILD_EF, 0x03, pin.ToBytes()).Encrypt(context);
        }

        internal static CommandApdu GetChangeReferenceDataCommand(SecureString newPin, SMContext context)
        {
            return new CommandApdu(CLA.PLAIN, INS.CHANGE_REFERENCE_DATA, (int)P1.CHILD_DF, 0, newPin.ToBytes()).Encrypt(context);
        }

        internal static CommandApdu GetVerifyCommand(byte[] pin, PasswordType passwordType, SMContext context)
        {
            return new CommandApdu(CLA.PLAIN, INS.VERIFY, 0, (byte)passwordType, pin).Encrypt(context);
        }
    }
}
