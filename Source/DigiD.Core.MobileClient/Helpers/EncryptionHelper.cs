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
using System.Security.Cryptography;
using DigiD.Common.Helpers;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Helpers
{
    internal static class EncryptionHelper
	{
        /// <summary>
        /// Will encrypt data and return result as byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="iv"></param>
        /// <returns>Encrypted data as byte[]</returns>
        internal static string Encrypt(string data, byte[] iv, byte[] key)
        {
            using (var cipher = Aes.Create())
            {
                cipher.Mode = CipherMode.CBC;
                cipher.KeySize = 256;

                using (var encryptor = cipher.CreateEncryptor(key, iv))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            var result = data.GetBytes();
                            cryptoStream.Write(result, 0, result.Length);
                            cryptoStream.FlushFinalBlock();

                            return memoryStream.ToArray().ToBase16();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Will decrypt data and return result as byte[]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="iv"></param>
        /// <param name="key"></param>
        /// <returns>Decrypt data as byte[]</returns>
        internal static string Decrypt(string data, byte[] iv, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;

                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    var cipherText = data.FromBase16();
                    using (var memoryStream = new MemoryStream(cipherText))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(cryptoStream))  
                            {  
                                return streamReader.ReadToEnd();  
                            } 
                        }
                    }
                }
            }
        }
    }
}
