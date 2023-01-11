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
