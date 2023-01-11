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
using System.Collections.Generic;
using DigiD.Common.Constants;
using DigiD.Common.Helpers;
using Foundation;
using Security;
using static DigiD.iOS.Helpers.SecurityStrings;

namespace DigiD.iOS.Helpers
{
    internal static class SecureEnclaveHelper
    {
        internal static bool Remove(string key)
        {
            var result = SecKeyChain.Remove(GetQuery(key, null));
            return result == SecStatusCode.Success;
        }

        internal static bool Generate(string key)
        {
            Remove(key);

            var sacObject = new SecAccessControl(SecAccessible.WhenUnlockedThisDeviceOnly, SecAccessControlCreateFlags.PrivateKeyUsage);

            var privateKey = key;
            var publicKey = key;

            var parameters = new Dictionary<string, NSObject>
            {
                [SecAttrKeyType] = new NSString(SecAttrKeyTypeEC),
                [SecAttrKeySizeInBits] = new NSNumber(256),
                [SecAttrApplicationTag] = NSData.FromString(StorageConstants.AppTag),
                [SecAttrTokenID] = new NSString(SecAttrTokenIDSecureEnclave),
                [SecAttrLabel] = NSData.FromString(key),

                // private key attributes
                [SecPrivateKeyAttrs] = new Dictionary<string, NSObject>
                {
                    [SecAttrAccessControl] = NSObject.FromObject(sacObject),
                    [SecAttrIsPermanent] = new NSNumber(true),
                    [SecAttrLabel] = NSData.FromString(privateKey),
                    [SecAttrCanSign] = new NSNumber(true),
                    [SecAttrApplicationTag] = NSData.FromString(StorageConstants.AppTag),
                }.ToNSDictionary(),

                // public key attributes
                [SecPublicKeyAttrs] = new Dictionary<string, NSObject>
                {
                    [SecAttrIsPermanent] = new NSNumber(true),
                    [SecAttrLabel] = NSData.FromString(publicKey),
                    [SecAttrCanSign] = new NSNumber(false),
                    [SecAttrApplicationTag] = NSData.FromString(StorageConstants.AppTag),
                }.ToNSDictionary(),
            };

            var result = SecKey.GenerateKeyPair(parameters.ToNSDictionary(), out var pubKey, out var priKey);

            return result == SecStatusCode.Success && pubKey != null && priKey != null;
        }

        internal static byte[] Export(string key)
        {
            var keyPair = GetKeyPair(key, false);
            return keyPair?.GetPublicKey().GetExternalRepresentation().ToArray();
        }

        internal static byte[] Decrypt(string key, byte[] cipherText)
        {
            var keyPair = GetKeyPair(key, true);
            var plaintText = keyPair.CreateDecryptedData(SecKeyAlgorithm.EciesEncryptionCofactorVariableIvx963Sha256AesGcm, NSData.FromArray(cipherText), out var error);
            return error == null ? plaintText.ToArray() : null;
        }

        internal static byte[] Sign(string key, string data)
        {
            var keyPair = GetKeyPair(key, true);
            var result = keyPair.RawSign(SecPadding.PKCS1, data.Hash(), out var signature);
            return result == SecStatusCode.Success ? signature : null;
        }

        private static SecRecord GetQuery(string key, bool? privateKey)
        {
            var query = new SecRecord(SecKind.Key)
            {
                ApplicationTag = StorageConstants.AppTag,
                Label = key
            };

            if (privateKey.HasValue)
                query.CanSign = privateKey.Value;

            return query;
        }

        private static SecKey GetKeyPair(string key, bool privateKey)
        {
            var keyPair = (SecKey)SecKeyChain.QueryAsConcreteType(GetQuery(key, privateKey), out var c);

            if (c != SecStatusCode.Success && c != SecStatusCode.ItemNotFound)
                return null;

            return keyPair;
        }
    }
}
