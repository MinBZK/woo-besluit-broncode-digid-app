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
﻿using System;
using System.Linq;
using BerTlv;
using DigiD.Common.EID.Helpers;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.NFC.Models;
using Org.BouncyCastle.Utilities;

namespace DigiD.Common.EID.Models
{
    internal static class SecureMessaging
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd">Plain APDU Command</param>
        /// <param name="ctx">Secure Context</param>
        /// <param name="chaining">Does the command need to be chained</param>
        /// <returns></returns>
        internal static CommandApdu Encrypt(this CommandApdu cmd, SMContext ctx, bool chaining = false)
        {
            Debugger.DumpInfo("PLAIN", cmd);

            var cla = chaining ? CLA.COMMAND_CHAINING_START : CLA.SECURE_MESSAGING;

            var smData = ctx.IsBAC ? GetCommandDataDES(cmd, ctx, cla) : GetCommandDataAES(cmd, ctx, cla);
            var command = new CommandApdu(cla, cmd.INS, cmd.P1, cmd.P2, smData) { IsSecureMessage = true };
            command.AppendLe();

            return command;
        }

        /// <summary>
        /// Encrypt the original command, and split if nessecery
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        internal static CommandApdu[] EncryptMulti(this CommandApdu cmd, SMContext ctx)
        {
            Debugger.DumpInfo("PLAIN", cmd);

            var smData = GetCommandDataAES(cmd, ctx, CLA.SECURE_MESSAGING);

            //Determine the total number of messages
            const int maxLength = 231;
            var itemLength = smData.Length / maxLength;

            //if there are rest bytes of modulo of the max bytes add 1
            if (smData.Length % maxLength != 0)
                itemLength += 1;

            var result = new CommandApdu[itemLength];

            for (var x = 0; x <= itemLength - 1; x++)
            {
                //determine the class byte
                var cla = itemLength == 1 ? CLA.SECURE_MESSAGING // of there 1 message, use default class
                    : x == 0 ? CLA.COMMAND_CHAINING_START // if index = 0, use 0x1c
                    : CLA.SECURE_MESSAGING; //every other message, use default

                //get the splitted data
                var splitData = smData
                    .Skip(x * maxLength)
                    .Take(x == itemLength - 1 ? smData.Length % maxLength : maxLength)
                    .ToArray();

                var command = new CommandApdu(cla, cmd.INS, cmd.P1, cmd.P2, splitData);

                if (x == itemLength - 1)
                    command.AppendLe();

                result[x] = command;
            }

            return result;
        }

        private static byte[] GetCommandDataDES(CommandApdu cmd, SMContext ctx, CLA cla)
        {
            var data97 = Array.Empty<byte>();

            if (cmd.Ne.HasValue)
                data97 = new DataObject(new byte[] { 0x97 }, new[] { (byte)cmd.Ne.Value }).GetEncoded();

            var header = new[] { (byte)cla, (byte)cmd.INS, (byte)cmd.P1, (byte)cmd.P2 };
            var paddedHeader = ByteHelper.AdjustPadding(header, 8);
            var do87 = GetEncryptData();

            var m = paddedHeader.Concat(do87).Concat(data97).ToArray();
            var ssc = ctx.IncrementSSC();
            var n = ssc.Concat(m).ToArray();
            var cc = CryptoDesHelper.ComputeMacTDes(n, ctx.KMac);

            var do8E = new DataObject(new byte[] { 0x8e }, cc);

            return do87.Concat(data97).Concat(do8E.GetEncoded()).ToArray();

            byte[] GetEncryptData()
            {
                if (cmd.Data.Length <= 0) return Array.Empty<byte>();

                var paddedData = ByteHelper.AdjustPadding(cmd.Data, 8);
                var encryptedData = CryptoDesHelper.TDesProvider(paddedData, ctx.KEnc, true);

                var data = new byte[] { 0x01 }.Concat(encryptedData).ToArray();
                return new DataObject(new byte[] { 0x87 }, data).GetEncoded();
            }
        }

        /// <summary>
        /// Construct the data for the SecureMessage
        /// </summary>
        /// <param name="cmd">Plain CommandApdu</param>
        /// <param name="ctx">Secure Context</param>
        /// <param name="cla">Class Identifier</param>
        /// <returns></returns>
        private static byte[] GetCommandDataAES(CommandApdu cmd, SMContext ctx, CLA cla)
        {
            var ssc = ctx.IncrementSSC();

            //Check if SSC is odd
            var increment = ssc.Skip(15).ToArray()[0];
            if (increment % 2 == 0)
                throw new ArgumentException("Wrong increment SSC", nameof(ctx));

            var data87 = GetEncryptData();
            var data97 = Array.Empty<byte>();

            if (cmd.Ne.HasValue)
                data97 = new DataObject(new byte[] { 0x97 }, new[] { (byte)cmd.Ne.Value }).GetEncoded();

            var paddedHeader = GetPaddedHeader();
            var messageToMac = ByteHelper.AdjustPadding(ssc.Concat(paddedHeader).Concat(data87).Concat(data97).ToArray(), 16);
            var mac = AesHelper.PerformCBC8(messageToMac, ctx.KMac);

            var data8E = new DataObject(new byte[] { 0x8e }, mac).GetEncoded();

            return data87.Concat(data97).Concat(data8E).ToArray();

            byte[] GetEncryptData()
            {
                if (cmd.Data.Length <= 0) return Array.Empty<byte>();

                var paddedData = ByteHelper.AdjustPadding(cmd.Data, 16);
                var iv = AesHelper.AESCBC(true, ssc, ctx.KEnc);
                var encryptedData = AesHelper.AESCBC(true, paddedData, ctx.KEnc, iv);

                var data = new byte[] { 0x01 }.Concat(encryptedData).ToArray();

                return new DataObject(new byte[] { 0x87 }, data).GetEncoded();
            }

            byte[] GetPaddedHeader()
            {
                var header = new[] { (byte)cla, (byte)cmd.INS, (byte)cmd.P1, (byte)cmd.P2 };
                return ByteHelper.AdjustPadding(header, 16);
            }
        }

        internal static ResponseApdu DecryptAES(this ResponseApdu resp, SMContext ctx, bool chaining = false)
        {
            var ssc = ctx.IncrementSSC();

            //Check if SSC is even
            var increment = ssc.Skip(15).ToArray()[0];
            if (increment % 2 != 0)
                throw new ArgumentException("Wrong increment SSC", nameof(ctx));

            var response = resp.Data;

            if (chaining && resp.Data.Length == 0)
                return resp;

            var dataToMac = Arrays.CopyOfRange(response, 0, response.Length - 10);

            dataToMac = Arrays.Concatenate(ssc, dataToMac);
            dataToMac = ByteHelper.AdjustPadding(dataToMac, 16);

            var mac = AesHelper.PerformCBC8(dataToMac, ctx.KMac);
            var data8E = Arrays.CopyOfRange(response, response.Length - 10, response.Length);

            if (!Arrays.AreEqual(mac, Arrays.CopyOfRange(data8E, 2, data8E.Length)))
                throw new ArgumentException("Could not verify integrity of response packet", nameof(ctx));

            var decryptedData = Array.Empty<byte>();

            if (response[0] == 0x87)
            {
                var tlvs = Tlv.ParseTlv(response);
                var encryptedData = tlvs.First().Value.Skip(1).ToArray();

                var iv = AesHelper.AESCBC(true, ssc, ctx.KEnc);
                decryptedData = AesHelper.AESCBC(false, encryptedData, ctx.KEnc, iv);
                decryptedData = ByteHelper.RemovePadding(decryptedData);
            }

            return new ResponseApdu(Arrays.Concatenate(decryptedData, new[] { (byte)resp.SW1, (byte)resp.SW2 }));
        }

        internal static ResponseApdu DecryptDES(this ResponseApdu response, SMContext context)
        {
            var data = response.Data;
            var ssc = context.IncrementSSC();
            var do8e = data.Skip(data.Length - 8).Take(8).ToArray();
            var do87 = data.Take(data.Length - 14).ToArray();
            var do99 = new DataObject(new byte[] { 0x99 }, new byte[] { 0x90, 0x00 });

            var k = ssc.Concat(do87).Concat(do99.GetEncoded()).ToArray();
            var cc = CryptoDesHelper.ComputeMacTDes(k, context.KMac);

            var decryptedData = Array.Empty<byte>();

            if (cc.SequenceEqual(do8e))
            {
                var index = do87.Length % 2 == 0 ? 4 : 3;
                var responseData = do87.Skip(index).Take(do87.Length - index).ToArray();

                decryptedData = CryptoDesHelper.TDesProvider(responseData, context.KEnc, false);
                decryptedData = ByteHelper.RemovePadding(decryptedData);
            }

            return new ResponseApdu(Arrays.Concatenate(decryptedData, new[] { (byte)response.SW1, (byte)response.SW2 }));
        }
    }
}
