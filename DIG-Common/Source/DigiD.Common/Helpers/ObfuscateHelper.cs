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
using System.Text;

namespace DigiD.Common.Helpers
{
    /// <summary>
	/// Helper voor het generen van een byte array key en het obfuscaten van strings met deze key.
	/// </summary>
	public static class ObfuscateHelper
    {
        #region Public Static Methods
        
        /// <summary>
        /// Obfuscate de text met een XOR met de key met als resultaat een een string representatie van een bytearray.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="byteKey">Key</param>
        public static byte[] Obfuscate(this string text, byte[] byteKey)
        {
            if (byteKey == null)
                throw new ArgumentNullException(nameof(byteKey));

            var decrypted = Encoding.UTF8.GetBytes(text);
            var obfuscated = new byte[decrypted.Length];
            
            for (var i = 0; i < decrypted.Length; i++)
            {
                obfuscated[i] = (byte)(decrypted[i] ^ byteKey[i % byteKey.Length]);
            }

            return obfuscated;
        }

        /// <summary>
        /// DeObfuscate de text gerepresenteerd als obfuscated bytearray met een XOR met de key.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="key">Key</param>
        public static string DeObfuscate(this byte[] text, byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (text == null)
                return string.Empty;

            var result = new byte[text.Length];

            for (var c = 0; c < text.Length; c++)
            {
                result[c] = (byte)(text[c] ^ key[c % key.Length]);
            }

            return Encoding.UTF8.GetString(result);
        }

        #endregion
    }
}
