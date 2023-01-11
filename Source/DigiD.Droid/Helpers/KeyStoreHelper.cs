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
using Android.Security.Keystore;
using DigiD.Common.Helpers;
using Java.Lang;
using Java.Math;
using Java.Security;
using Java.Security.Spec;
using Java.Util;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Javax.Security.Auth.X500;
using Org.BouncyCastle.Asn1;
using Exception = Java.Lang.Exception;

namespace DigiD.Droid.Helpers
{
    internal static class KeyStoreHelper
    {
        private const string KeyStoreProvider = "AndroidKeyStore";

        private static IAlgorithmParameterSpec GetParameterSpec(string keyName)
        {
            var builder = new KeyGenParameterSpec.Builder(keyName, KeyStorePurpose.Sign);

            return builder
                .SetAlgorithmParameterSpec(new ECGenParameterSpec("prime256v1"))
                ?.SetDigests(KeyProperties.DigestSha256)
                .SetUserAuthenticationRequired(false)
                .Build();
        }

        internal static bool SupportHardwareBackedKeyStore()
        {
            bool result;
            try
            {
                const string key = "SecureHardwareTestKey";
                var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmEc, KeyStoreProvider);

                if (generator == null)
                    return false;

                generator.Initialize(GetParameterSpec(key));
                generator.GenerateKeyPair();

                var ks = KeyStore.GetInstance(KeyStoreProvider);

                if (ks == null)
                    return false;

                ks.Load(null);
                var factory = KeyFactory.GetInstance(ks.GetKey(key, null)?.Algorithm, KeyStoreProvider);
                var keyInfo = (KeyInfo)factory?.GetKeySpec(ks.GetKey(key, null), Class.FromType(typeof(KeyInfo)));

                if (keyInfo == null)
                    return false;

                result = keyInfo.IsInsideSecureHardware;
                ks.DeleteEntry(key);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        internal static bool? SupportStrongBoxBacked()
        {
            try
            {
                const string alias = "strongbox_test";
                var generator = CreateGenerator(alias, true);
                var keyPair = generator.GenKeyPair();
                return keyPair != null && Delete(alias);
            }
            catch (StrongBoxUnavailableException)
            {
                return false;
            }
            catch
            {
                return null;
            }
        }

        internal static bool CreateKeyPairForSigning(string keyName)
        {
            try
            {
                var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmEc, KeyStoreProvider);
                if (generator == null)
                    return false;

                generator.Initialize(GetParameterSpec(keyName));
                var result = generator.GenerateKeyPair();
                return result != null;
            }
            catch (Exception ex)
            {
                AppCenterHelper.TrackError(ex);
                return false;
            }
        }

        private static KeyPairGenerator CreateGenerator(string keyName, bool strongBoxBacked)
        {
            var dateValidFrom = Calendar.GetInstance(Java.Util.TimeZone.Default);
            var dateValidTo = Calendar.GetInstance(Java.Util.TimeZone.Default);
            dateValidTo.Add(CalendarField.Year, 10);

            var specOfKey = new KeyGenParameterSpec.Builder(keyName, KeyStorePurpose.Decrypt)
                .SetAlgorithmParameterSpec(new RSAKeyGenParameterSpec(2048, RSAKeyGenParameterSpec.F0))
                .SetKeySize(2048)
                .SetKeyValidityStart(dateValidFrom.Time)
                .SetKeyValidityEnd(dateValidTo.Time)
                .SetCertificateSerialNumber(BigInteger.One)
                .SetCertificateSubject(new X500Principal($"CN={keyName}"))
                .SetDigests(KeyProperties.DigestSha256)
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingRsaOaep);

            if (strongBoxBacked)
                specOfKey.SetIsStrongBoxBacked(true);

            var generator = KeyPairGenerator.GetInstance(KeyProperties.KeyAlgorithmRsa, KeyStoreProvider);

            if (generator == null)
                return null;

            generator.Initialize(specOfKey.Build());
            return generator;
        }

        internal static byte[] GenerateKeyPairForEncryption(string keyName)
        {
            var supportStrongBox = SupportStrongBoxBacked();
            var generator = CreateGenerator(keyName, supportStrongBox.HasValue && supportStrongBox.Value);
            var keyPair = generator.GenKeyPair();
            return keyPair?.Public?.GetEncoded();
        }

        internal static byte[] Export(string keyName, bool raw = false)
        {
            try
            {
                var ks = KeyStore.GetInstance(KeyStoreProvider);

                if (ks == null)
                    return new byte[] { };

                ks.Load(null);
                var entry = (KeyStore.PrivateKeyEntry)ks.GetEntry(keyName, null);
                var key = entry?.Certificate?.PublicKey?.GetEncoded();

                if (raw)
                    return key;

                using (var s = new Asn1InputStream(key))
                {
                    var sequence = (DerSequence)s.ReadObject();
                    var cert = (DerBitString)sequence[1];
                    return cert.GetBytes();
                }
            }
            catch (Exception e)
            {
                AppCenterHelper.TrackError(e);
                return null;
            }
        }

        internal static byte[] Sign(byte[] data, string keyName)
        {
            try
            {
                var ks = KeyStore.GetInstance(KeyStoreProvider);

                if (ks == null)
                    return null;

                ks.Load(null);
                var entry = (KeyStore.PrivateKeyEntry)ks.GetEntry(keyName, null);
                var s = Signature.GetInstance("SHA256withECDSA");

                if (s == null || entry == null)
                    return null;

                s.InitSign(entry.PrivateKey);
                s.Update(data);

                return s.Sign();
            }
            catch (Exception ex)
            {
                AppCenterHelper.TrackError(ex);
                return null;
            }
        }

        internal static byte[] Decrypt(byte[] cipherText, string keyName)
        {
            try
            {
                var ks = KeyStore.GetInstance(KeyStoreProvider);

                if (ks == null)
                    return null;

                ks.Load(null);
                var entry = (KeyStore.PrivateKeyEntry)ks.GetEntry(keyName, null);
                var cipher = GetCipher(CipherMode.DecryptMode, entry.PrivateKey);
                return cipher.DoFinal(cipherText);
            }
            catch (Exception ex)
            {
                AppCenterHelper.TrackError(ex);
                return null;
            }
        }

        internal static bool Delete(string keyName)
        {
            try
            {
                var ks = KeyStore.GetInstance(KeyStoreProvider);

                if (ks == null)
                    return false;

                ks.Load(null);
                ks.DeleteEntry(keyName);
                return true;
            }
            catch (Exception ex)
            {
                AppCenterHelper.TrackError(ex);
                return false;
            }
        }

        private static Cipher GetCipher(CipherMode operationMode, IKey key)
        {
            var cipher = Cipher.GetInstance("RSA/ECB/OAEPPadding");
            cipher?.Init(operationMode, key, new OAEPParameterSpec(KeyProperties.DigestSha256, "MGF1", MGF1ParameterSpec.Sha1, PSource.PSpecified.Default));
            return cipher;
        }
    }
}
