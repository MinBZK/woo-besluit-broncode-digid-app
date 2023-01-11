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
using DigiD.Common.Helpers;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Xamarin.Forms;

namespace DigiD.Security
{
    internal class BouncyCastleImplementation : ISigningService
    {
        public bool GenerateKeyPair()
        {
            try
            {
                //Delete potential 
                DependencyService.Get<IKeyStore>().Delete("PrivateKey");
                DependencyService.Get<IKeyStore>().Delete("PublicKey");

                var gen = new ECKeyPairGenerator("EC");
                var keyParam = new ECKeyGenerationParameters(ECNamedCurveTable.GetOid("prime256v1"), new SecureRandom());
                gen.Init(keyParam);
                var key1 = gen.GenerateKeyPair();

                //Store keys in keychain
                DependencyService.Get<IKeyStore>().Insert(Convert.ToBase64String(PrivateKeyInfoFactory.CreatePrivateKeyInfo(key1.Private).ToAsn1Object().GetEncoded()), "PrivateKey");
                DependencyService.Get<IKeyStore>().Insert(Convert.ToBase64String(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key1.Public).PublicKeyData.GetBytes()), "PublicKey");
                return true;
            }
            catch (Exception e)
            {
                AppCenterHelper.TrackError(e);
                return false;
            }
        }

        public byte[] ExportPublicKey()
        {
            var data = DependencyService.Get<IKeyStore>().FindValueForKey("PublicKey");
            return Convert.FromBase64String(data);
        }

        public byte[] Sign(string data)
        {
            var bytes = data.GetBytes();

            var key = (ECPrivateKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(DependencyService.Get<IKeyStore>().FindValueForKey("PrivateKey")));

            var signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, key);
            signer.BlockUpdate(bytes, 0, bytes.Length);

            return signer.GenerateSignature();
        }

        public bool IsSupported => false;
        public bool HardwareSupport => false;
        public bool RemoveKeyPair()
        {
            var res1 = DependencyService.Get<IKeyStore>().Delete("PrivateKey");
            var res2 = DependencyService.Get<IKeyStore>().Delete("PublicKey");

            return res1 && res2;
        }
    }
}
