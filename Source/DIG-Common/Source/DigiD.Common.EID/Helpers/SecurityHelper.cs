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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.EID.Models;
using DigiD.Common.EID.Models.Apdu;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace DigiD.Common.EID.Helpers
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Generate a KeyPair based on the specified curve <param name="param"></param>
        /// </summary>
        /// <param name="param">Curve</param>
        /// <param name="point">Point on the curve</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GenerateKeyPair(X9ECParameters param, ECPoint point = null)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            var gen = new ECKeyPairGenerator();
            var specs = new ECDomainParameters(param.Curve, point ?? param.G, param.N, param.H, param.GetSeed());
            gen.Init(new ECKeyGenerationParameters(specs, new SecureRandom()));
            return gen.GenerateKeyPair();
        }

        /// <summary>
        /// Generate a shared secret based on the private and public key
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static ECPoint GenerateSharedSecret(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException(nameof(publicKey));

            if (privateKey == null)
                throw new ArgumentNullException(nameof(privateKey));

            var ecPoint = publicKey.Q.Multiply(privateKey.D).Normalize();

            if (ecPoint.IsInfinity)
                throw new ArgumentException("Infinity is not a valid agreement value for ECDH", nameof(publicKey));

            return ecPoint;
        }

        /// <summary>
        /// Generate a keypair, based on the public and private key, with the nonce on the curve
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="nonce"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Task<AsymmetricCipherKeyPair> GenerateKeyPairAgreement(ECPublicKeyParameters publicKey, ECPrivateKeyParameters privateKey, byte[] nonce, X9ECParameters param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            var sharedSecret = GenerateSharedSecret(privateKey, publicKey).GetEncoded(false);

            var gCircumFlex = CalculateECDHGenericMapping(nonce, sharedSecret, param);

            var point = param.Curve.DecodePoint(gCircumFlex);

            return Task.FromResult(GenerateKeyPair(param, point));
        }

        /**
	 * Performs a G-Circumflex mapping of a nonce and a shared secret to a curve
	 * reference
	 *
	 * @param nonce
	 *            the random nonce to use
	 * @param sharedSecret
	 *            the shared secret point
	 * @param curveReference
	 *            the curve to map against
	 * @return the mapped point on the curve
	 */

        private static byte[] CalculateECDHGenericMapping(byte[] nonce, byte[] sharedSecret, X9ECParameters param)
        {
            var generator = param.Curve.CreatePoint(param.G.AffineXCoord.ToBigInteger(), param.G.AffineYCoord.ToBigInteger()); //G

            var nonceInteger = new BigInteger(1, nonce); //s
            var secretPoint = param.Curve.DecodePoint(sharedSecret);  //h
            var g = generator.Multiply(nonceInteger).Add(secretPoint); //G'

            return g.GetEncoded(false);
        }

        public static ECPublicKeyParameters DecodeKey(XYResponseAPDU response, X9ECParameters param)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (param == null)
                throw new ArgumentNullException(nameof(param));

            var xbi = new BigInteger(1, response.PointX);
            var ybi = new BigInteger(1, response.PointY);

            var point = param.Curve.CreatePoint(xbi, ybi);
            var specs = new ECDomainParameters(param.Curve, param.G, param.N, param.H);

            return new ECPublicKeyParameters(point, specs);
        }

        public static ECPublicKeyParameters DecodeKey(byte[] encoded, X9ECParameters param)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            if (param == null)
                throw new ArgumentNullException(nameof(param));

            var x = GetXFromUncompressedFormat(encoded);
            var y = GetYFromUncompressedFormat(encoded);

            var xbi = new BigInteger(1, x);
            var ybi = new BigInteger(1, y);

            var point = param.Curve.CreatePoint(xbi, ybi);
            var specs = new ECDomainParameters(param.Curve, param.G, param.N, param.H);

            return new ECPublicKeyParameters(point, specs);
        }

        public static (byte[] x, byte[] y) GetXandYPoints(ECPublicKeyParameters pk)
        {
            var pub = DecodeECPublicKeyX509(pk);
            return (GetXFromUncompressedFormat(pub), GetYFromUncompressedFormat(pub));
        }

        public static byte[] GetXFromUncompressedFormat(byte[] publicKeyRaw)
        {
            if (publicKeyRaw == null)
                throw new ArgumentNullException(nameof(publicKeyRaw));

            if (publicKeyRaw[0] != 0x04)
                throw new ArgumentException("EC component not in uncompressed format", nameof(publicKeyRaw));

            var length = (publicKeyRaw.Length - 1) / 2;
            return publicKeyRaw.Skip(1).Take(length).ToArray();
        }

        private static byte[] GetYFromUncompressedFormat(byte[] publicKeyRaw)
        {
            if (publicKeyRaw == null)
                throw new ArgumentNullException(nameof(publicKeyRaw));

            if (publicKeyRaw[0] != 0x04)
                throw new ArgumentException("EC component not in uncompressed format", nameof(publicKeyRaw));

            var length = (publicKeyRaw.Length - 1) / 2;
            return publicKeyRaw.Skip(1 + length).Take(length).ToArray();
        }


        internal static byte[] CalculateKEnc(byte[] secretX, Card card)
        {
            var bytes = secretX.Concat(new byte[] { 0x00, 0x00, 0x00, 0x01 }).ToArray();
            return AesHelper.CalculateHash(bytes, card.MessageDigestAlgorithm, card.KeyLength);
        }

        internal static byte[] CalculateKMac(byte[] secretX, Card card)
        {
            var bytes = secretX.Concat(new byte[] { 0x00, 0x00, 0x00, 0x02 }).ToArray();
            return AesHelper.CalculateHash(bytes, card.MessageDigestAlgorithm, card.KeyLength);
        }

        public static byte[] DecodeECPublicKeyX509(ECPublicKeyParameters pk)
        {
            if (pk == null)
                throw new ArgumentNullException(nameof(pk));

            var x = RemoveLeadingZero(pk.Q.AffineXCoord.ToBigInteger().ToByteArray());
            var y = RemoveLeadingZero(pk.Q.AffineYCoord.ToBigInteger().ToByteArray());

            return new byte[] { 0x04 }.Concat(x).Concat(y).ToArray();
        }

        private static IEnumerable<byte> RemoveLeadingZero(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length > 40 && data[0] == 0x00)
                return data.Skip(1).ToArray();

            if (data.Length < 0)
                return new byte[] { 0x00 }.Concat(data).ToArray();

            return data;
        }

        internal static byte[] Sign(AsymmetricKeyParameter privateKey, byte[] message, string algoritm)
        {
            var signer = SignerUtilities.GetSigner(algoritm);
            signer.Init(true, privateKey);
            signer.BlockUpdate(message, 0, message.Length);
            var signature = signer.GenerateSignature();
            return signature;
        }
    }
}
