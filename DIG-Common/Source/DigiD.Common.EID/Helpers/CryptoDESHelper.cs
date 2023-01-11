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
﻿using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Helpers;
using DigiD.Common.NFC.Models;

namespace DigiD.Common.EID.Helpers
{
    public static class CryptoDesHelper
    {
        public static byte[] ComputeKey(byte[] kSeed, string c)
        {
            // concat kSeed and c
            var cBytes = c.ConvertHexToBytes();
            var D = kSeed.Concat(cBytes).ToArray();

            var hash = ComputeSHA1Hash(D);

            // form DES keys Ka and Kb
            var keyA = hash.Take(8).ToArray();
            var keyB = hash.Skip(8).Take(8).ToArray();

            // adjust parity 
            var keyAParity = ByteHelper.AdjustParity(keyA);
            var keyBParity = ByteHelper.AdjustParity(keyB);

            // return concatenation two keys to form Kenc
            return keyAParity.Concat(keyBParity).ToArray();
        }

        public static byte[] ComputeSHA1Hash(byte[] toHash)
        {
            var sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(toHash);
        }

        internal static bool IsValidResponse(this ResponseApdu apdu, SMContext context)
        {
            if (context == null || !context.IsBAC)
                return true;
        
            var ssc = context.IncrementSSC();
            var do8e = apdu.Data.Skip(6).Take(8);
            var do99 = new DataObject(new byte[] { 0x99 }, new byte[] { 0x90, 0x00 });
            var k = ssc.Concat(do99.GetEncoded()).ToArray();
            var cc = ComputeMacTDes(k, context.KMac);
            
            return cc.SequenceEqual(do8e);
        }

        public static byte[] TDesProvider(byte[] inputBuffer, byte[] key, bool encrypt)
        {
            var tDes = new TripleDESCryptoServiceProvider
            {
                Key = key,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                IV = new byte[8]
            };

            byte[] result;

            using (var stream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(stream, encrypt ? tDes.CreateEncryptor() : tDes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(inputBuffer, 0, inputBuffer.Length);
                cryptoStream.Flush();
                result = stream.ToArray();
            }

            return result;
        }

        public static byte[] ComputeMacTDes(byte[] inputBuffer, byte[] key)
        {
            // split the 16 byte MAC key into two keys
            var key1 = key.Take(8).ToArray();
            var key2 = key.Skip(8).Take(8).ToArray();

            // first and second DES
            var des1 = DES.Create();
            des1.BlockSize = 64;
            des1.Key = key1;
            des1.Mode = CipherMode.CBC;
            des1.Padding = PaddingMode.None;
            des1.IV = new byte[8];

            var des2 = DES.Create();
            des2.BlockSize = 64;
            des2.Key = key2;
            des2.Mode = CipherMode.CBC;
            des2.Padding = PaddingMode.None;
            des2.IV = new byte[8];

            var eIfdPadded = ByteHelper.AdjustPadding(inputBuffer, 8);

            var intN = new byte[8];

            // MAC algorithm 3
            // initial transformation
            var hN = des1.CreateEncryptor().TransformFinalBlock(eIfdPadded.Take(8).ToArray(), 0, 8);

            // split the blocks
            // iteration on the rest of blocks
            for (var j = 1; j < eIfdPadded.Length / 8; j++)
            {
                var dN = eIfdPadded.Skip(8 * j).Take(8).ToArray();
                for (var i = 0; i < 8; i++)
                    intN[i] = (byte)(hN[i] ^ dN[i]);

                hN = des1.CreateEncryptor().TransformFinalBlock(intN, 0, 8);
            }

            // output Transformation 3
            var hNdecrypt = des2.CreateDecryptor().TransformFinalBlock(hN, 0, 8);
            var mIfd = des1.CreateEncryptor().TransformFinalBlock(hNdecrypt, 0, 8);

            return mIfd;
        }
    }
}
