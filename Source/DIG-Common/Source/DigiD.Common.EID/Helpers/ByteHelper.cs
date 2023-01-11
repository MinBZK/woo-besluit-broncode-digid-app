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
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using BerTlv;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.Helpers
{
    public static class ByteHelper
    {
        public static byte[] ToBytes(this SecureString secureString)
        {
            return Encoding.UTF8.GetBytes(secureString.ToPlain());
        }

        public static string ToPlain(this SecureString value)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        /// <summary>Method <c>Xor</c> returns the exclusive OR of two byte arrays.</summary>
        /// <param name="lhs">byte array to xor (left hand side)</param>
        /// <param name="rhs">byte array to xor (right hand side)</param>
        /// <returns>byte[] the xor of input arrays</returns>
        public static byte[] Xor(byte[] lhs, byte[] rhs)
        {
            if (lhs.Length != rhs.Length)
                throw new ArgumentException("The arrays to XOR are not the same length");

            byte[] xor = new byte[lhs.Length];

            for (int i = 0; i < lhs.Length; ++i)
                xor[i] = (byte)(lhs[i] ^ rhs[i]);

            return xor;
        }

        public static byte[] AdjustPadding(byte[] data, int size)
        {
            using var memStream = new MemoryStream();
            memStream.Write(data, 0, data.Length);
            memStream.WriteByte(0x80);

            while (memStream.Length % size != 0)
            {
                memStream.WriteByte(0x00);
            }

            return memStream.ToArray();
        }

        /**
	 * Add AES padding to the provided data. Padding scheme appends '80' byte to
	 * data and then zero or more '00' bytes until data length is modulo 16 (defined
	 * in GlobalPlatform Card Specification Amendment D section 4.1.4).
	 * 
	 * @param data
	 *            the data to be padded
	 * @return the padded data
	 * @throws NullPointerException
	 *             if the input parameter is null
	 */
        public static byte[] RemovePadding(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var dataLength = data.Length;

            if (data[dataLength - 1] == 0x80)
            {
                // Only one byte of padding.
                dataLength--;
            }
            else
            {
                // Reverse search through the data looking for the padding 80. Should be 00
                // until we reach the 80.
                for (var i = data.Length - 1; i >= 0; i--)
                {
                    if (data[i] == 0x00)
                        dataLength--;
                    else if (data[i] == 0x80)
                    {
                        dataLength--;
                        break;
                    }
                    else
                        throw new InvalidDataException("Error removing AES padding");
                }
            }

            return data.Take(dataLength).ToArray();
        }

        public static byte[] AdjustParity(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                bytes[i] = (byte)((b & 0xfe) | (((b >> 1) ^ (b >> 2) ^ (b >> 3) ^ (b >> 4) ^ (b >> 5) ^ (b >> 6) ^ (b >> 7) ^ 0x01) & 0x01));
            }
            return bytes;
        }

        internal static byte[] GetSignature(byte[] rawSignature)
        {
            var tlvs = Tlv.ParseTlv(rawSignature);
            var d = tlvs.First().Children.ToList();

            var p1 = d[0].Value;
            var p2 = d[1].Value;

            if (p1[0] == 0x00)
                p1 = p1.Skip(1).ToArray();
            if (p2[0] == 0x00)
                p2 = p2.Skip(1).ToArray();

            return p1.Concat(p2).ToArray();
        }

        /// <summary>
        /// Get length of data based on the length-tag
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Tuple<int, int> GetLength(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            const int lengthBit = 2;

            if (data.Length <= lengthBit)
                return new Tuple<int, int>(0, 0);

            if (data[1] != 0x81 && data[1] != 0x82)
                return new Tuple<int, int>(data[1], lengthBit);

            var length = new[] { data[1] };

            var bits = new BitArray(length);

            if (bits[7])
            {
                var arr = new int[1];

                bits[7] = false;

                bits.CopyTo(arr, 0);
                var l = arr[0];

                var lengthBytes = new byte[l];
                Buffer.BlockCopy(data, lengthBit, lengthBytes, 0, l);

                return new Tuple<int, int>(lengthBytes.ConvertHexToInt(), l + lengthBit);
            }

            return new Tuple<int, int>(data[1], lengthBit);
        }
    }
}
