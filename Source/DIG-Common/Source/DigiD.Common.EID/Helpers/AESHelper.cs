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
using DigiD.Common.NFC.Helpers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace DigiD.Common.EID.Helpers
{
    public static class AesHelper
    {
        /**
	 * Encrypt/DecryptAES the provided data using AES in CBC mode and the specified key and
	 * initial vector. The data will not be padded before encryption.
	 * 
	 * @param data
	 *            the data to be encrypted
	 * @param keyBytes
	 *            the bytes of the AES key to use for the encryption
	 * @return the encrypted data
	 * @throws IllegalStateException
	 *             if there was a problem during encryption
	 */

        public static byte[] AESCBC(bool forEncryption, byte[] data, byte[] key, byte[] iv = null)
        {
            var cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");

            var keyParameter = ParameterUtilities.CreateKeyParameter("AES", key);
            var cipherParameters = new ParametersWithIV(keyParameter, iv ?? new byte[16]);

            cipher.Init(forEncryption, cipherParameters);

            try
            {
                return cipher.DoFinal(data);
            }
            catch (Exception ex)
            {
                Debugger.DumpInfo(ex);
                throw;
            }
        }

        public static byte[] CalculateHash(byte[] data, string algorithm, int? length = null)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var dig = DigestUtilities.GetDigest(algorithm);
            dig.BlockUpdate(data, 0, data.Length);

            var result = new byte[dig.GetDigestSize()];
            dig.DoFinal(result, 0);

            return length.HasValue ? result.Take(length.Value).ToArray() : result.ToArray();
        }

        /**
	 * AES [FIPS 197] SHALL be used in CMAC-mode [SP 800-38B] with a MAC length of 8
	 * bytes.
	 *
	 * @param data
	 *            the data to MAC
	 * @param key
	 *            the key to use
	 * @return the 8 byte MAC of the data
	 */
        public static byte[] PerformCBC8(byte[] data, byte[] key)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var cbc8 = new CMac(new AesEngine(), 64);
            cbc8.Init(new KeyParameter(key));

            var result = new byte[8];
            cbc8.BlockUpdate(data, 0, data.Length);
            cbc8.DoFinal(result, 0);

            return result;
        }

        
    }
}
