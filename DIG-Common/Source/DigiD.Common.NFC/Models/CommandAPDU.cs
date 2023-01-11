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
using System.IO;
using DigiD.Common.NFC.Enums;

namespace DigiD.Common.NFC.Models
{
    public class CommandApdu
    {
        public bool IsSecureMessage { get; set; }
        /// <summary>
        /// @serial </summary>
        private byte[] _apdu;

        // value of nc
        private int _nc;

        // value of ne
        private int? _ne;

        // index of start of data within the apdu array
        private int _dataOffset;

        private static void CheckArrayBounds(byte[] b, int ofs, int len)
        {
            if ((ofs < 0) || (len < 0))
            {
                throw new ArgumentException("Offset and length must not be negative");
            }
            if (b == null)
            {
                if ((ofs != 0) && (len != 0))
                {
                    throw new ArgumentException("offset and length must be 0 if array is null");
                }
            }
            else
            {
                if (ofs > b.Length - len)
                {
                    throw new ArgumentException("Offset plus length exceed array size");
                }
            }
        }

        /// <summary>
        /// Constructs a CommandApdu from a byte array containing the complete
        /// APDU contents (header and body).
        /// 
        /// <para>Note that the apdu bytes are copied to protect against
        /// subsequent modification.
        /// 
        /// </para>
        /// </summary>
        /// <param name="apdu"> the complete command APDU
        /// </param>
        public CommandApdu(byte[] apdu)
        {
            if (apdu == null)
                return;

            _apdu = (byte[])apdu.Clone();
            Parse();
        }

        /// <summary>
        /// Constructs a CommandApdu from a byte array containing the complete
        /// APDU contents (header and body). The APDU starts at the index
        /// <code>apduOffset</code> in the byte array and is <code>apduLength</code>
        /// bytes long.
        /// 
        /// <para>Note that the apdu bytes are copied to protect against
        /// subsequent modification.
        /// 
        /// </para>
        /// </summary>
        /// <param name="apdu"> the complete command APDU </param>
        /// <param name="apduOffset"> the offset in the byte array at which the apdu
        ///   data begins </param>
        /// <param name="apduLength"> the length of the APDU
        /// </param>
        public CommandApdu(byte[] apdu, int apduOffset, int apduLength)
        {
            CheckArrayBounds(apdu, apduOffset, apduLength);
            apdu = new byte[apduLength];
            Array.Copy(apdu, apduOffset, apdu, 0, apduLength);
            Parse();
        }

        /// <summary>
        /// Constructs a CommandApdu from the four header bytes. This is case 1
        /// in ISO 7816, no command body.
        /// </summary>
        /// <param name="cla"> the class byte CLA </param>
        /// <param name="ins"> the instruction byte INS </param>
        /// <param name="p1"> the parameter byte P1 </param>
        /// <param name="p2"> the parameter byte P2 </param>
        internal CommandApdu(CLA cla, INS ins, int p1, int p2) : this(cla, ins, p1, p2, null, 0, 0, null)
        {
        }

        /// <summary>
        /// Constructs a CommandApdu from the four header bytes and the expected
        /// response data length. This is case 2 in ISO 7816, empty command data
        /// field with Ne specified. If Ne is 0, the APDU is encoded as ISO 7816
        /// case 1.
        /// </summary>
        /// <param name="cla"> the class byte CLA </param>
        /// <param name="ins"> the instruction byte INS </param>
        /// <param name="p1"> the parameter byte P1 </param>
        /// <param name="p2"> the parameter byte P2 </param>
        /// <param name="ne"> the maximum number of expected data bytes in a response APDU
        /// </param>
        internal CommandApdu(CLA cla, INS ins, int p1, int p2, int? ne) : this(cla, ins, p1, p2, null, 0, 0, ne)
        {
        }

        internal CommandApdu(CLA cla, INS ins, int p1, int p2, byte[] data, int dataOffset, int dataLength) : this(cla, ins, p1, p2, data, dataOffset, dataLength, null)
        {
        }

        public CommandApdu(CLA cla, INS ins, int p1, int p2, byte[] data) : this(cla, ins, p1, p2, data, 0, ArrayLength(data), null)
        {
        }

        public CommandApdu(CLA cla, INS ins, int p1, int p2, byte[] data, int? ne) : this(cla, ins, p1, p2, data, 0, ArrayLength(data), ne)
        {
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
#pragma warning disable S107 // Methods should not have too many parameters
        internal CommandApdu(CLA cla, INS ins, int p1, int p2, byte[] data, int dataOffset, int dataLength, int? ne)
#pragma warning restore S107 // Methods should not have too many parameters
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            CheckArrayBounds(data, dataOffset, dataLength);
            if (dataLength > 65535)
            {
                throw new ArgumentException("dataLength is too large");
            }
            if (ne < 0)
            {
                throw new ArgumentException("ne must not be negative");
            }
            if (ne > 65536)
            {
                throw new ArgumentException("ne is too large");
            }
            _ne = ne;
            _nc = dataLength;
            if (dataLength == 0)
            {
                if (ne == 0)
                {
                    // case 1
                    _apdu = new byte[4];
                    SetHeader(cla, ins, p1, p2);
                }
                else
                {
                    // case 2s or 2e
                    if (ne <= 256)
                    {
                        // case 2s
                        // 256 is encoded as 0x00
                        var len = (ne != 256) ? (byte)ne : 0;
                        _apdu = new byte[5];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[4] = Convert.ToByte(len);
                    }
                    else
                    {
                        // case 2e
                        byte l1, l2;
                        // 65536 is encoded as 0x00 0x00
                        if (ne == 65536)
                        {
                            l1 = 0;
                            l2 = 0;
                        }
                        else
                        {
                            l1 = (byte)(ne >> 8);
                            l2 = (byte)ne;
                        }
                        _apdu = new byte[7];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[5] = l1;
                        _apdu[6] = l2;
                    }
                }
            }
            else
            {
                if (ne == null)
                {
                    // case 3s or 3e
                    if (dataLength <= 255)
                    {
                        // case 3s
                        _apdu = new byte[4 + 1 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[4] = (byte)dataLength;
                        _dataOffset = 5;
                        Array.Copy(data, dataOffset, _apdu, 5, dataLength);
                    }
                    else
                    {
                        // case 3e
                        _apdu = new byte[4 + 3 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[4] = 0;
                        _apdu[5] = (byte)(dataLength >> 8);
                        _apdu[6] = (byte)dataLength;
                        _dataOffset = 7;
                        Array.Copy(data, dataOffset, _apdu, 7, dataLength);
                    }
                }
                else
                {
                    // case 4s or 4e
                    if ((dataLength <= 255) && (ne <= 256))
                    {
                        // case 4s
                        _apdu = new byte[4 + 2 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[4] = (byte)dataLength;
                        _dataOffset = 5;
                        Array.Copy(data, dataOffset, _apdu, 5, dataLength);
                        _apdu[_apdu.Length - 1] = Convert.ToByte((ne != 256) ? (byte)ne : 0);
                    }
                    else
                    {
                        // case 4e
                        _apdu = new byte[4 + 5 + dataLength];
                        SetHeader(cla, ins, p1, p2);
                        _apdu[4] = 0;
                        _apdu[5] = (byte)(dataLength >> 8);
                        _apdu[6] = (byte)dataLength;
                        _dataOffset = 7;
                        Array.Copy(data, dataOffset, _apdu, 7, dataLength);
                        if (ne == 65536) return;

                        var leOfs = _apdu.Length - 2;
                        _apdu[leOfs] = (byte)(ne >> 8);
                        _apdu[leOfs + 1] = (byte)ne;
                    }
                }
            }
        }

        private static int ArrayLength(byte[] b)
        {
            return b?.Length ?? 0;
        }

#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private void Parse()
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
        {
            if (_apdu.Length < 4)
            {
                throw new ArgumentException("apdu must be at least 4 bytes long");
            }
            if (_apdu.Length == 4)
            {
                // case 1
                return;
            }
            var l1 = _apdu[4] & 0xff;
            if (_apdu.Length == 5)
            {
                // case 2s
                _ne = (l1 == 0) ? 256 : l1;
                return;
            }
            if (l1 != 0)
            {
                if (_apdu.Length == 4 + 1 + l1)
                {
                    // case 3s
                    _nc = l1;
                    _dataOffset = 5;
                    return;
                }
                if (_apdu.Length != 4 + 2 + l1)
                    throw new ArgumentException("Invalid APDU: length=" + _apdu.Length + ", b1=" + l1);

                // case 4s
                _nc = l1;
                _dataOffset = 5;
                var newLe2 = _apdu[_apdu.Length - 1] & 0xff;
                _ne = (newLe2 == 0) ? 256 : newLe2;
                return;
            }
            if (_apdu.Length < 7)
            {
                throw new ArgumentException("Invalid APDU: length=" + _apdu.Length + ", b1=" + l1);
            }
            var l2 = ((_apdu[5] & 0xff) << 8) | (_apdu[6] & 0xff);
            if (_apdu.Length == 7)
            {
                // case 2e
                _ne = (l2 == 0) ? 65536 : l2;
                return;
            }
            if (l2 == 0)
            {
                throw new ArgumentException("Invalid APDU: length=" + _apdu.Length + ", b1=" + l1 + ", b2||b3=" + l2);
            }
            if (_apdu.Length == 4 + 3 + l2)
            {
                // case 3e
                _nc = l2;
                _dataOffset = 7;
                return;
            }

            if (_apdu.Length == 4 + 5 + l2)
            {
                // case 4e
                _nc = l2;
                _dataOffset = 7;
                var leOfs = _apdu.Length - 2;
                var l3 = ((_apdu[leOfs] & 0xff) << 8) | (_apdu[leOfs + 1] & 0xff);
                _ne = (l3 == 0) ? 65536 : l3;
            }
            else
            {
                throw new ArgumentException("Invalid APDU: length=" + _apdu.Length + ", b1=" + l1 + ", b2||b3=" + l2);
            }
        }

        private void SetHeader(CLA cla, INS ins, int p1, int p2)
        {
            _apdu[0] = (byte)cla;
            _apdu[1] = (byte)ins;
            _apdu[2] = (byte)p1;
            _apdu[3] = (byte)p2;
        }

        /// <summary>
        /// Returns the value of the class byte CLA.
        /// </summary>
        /// <returns> the value of the class byte CLA. </returns>
        public virtual CLA CLA => (CLA)(_apdu[0] & 0xff);

        /// <summary>
        /// Returns the value of the instruction byte INS.
        /// </summary>
        /// <returns> the value of the instruction byte INS. </returns>
        public virtual INS INS => (INS)(_apdu[1] & 0xff);

        /// <summary>
        /// Returns the value of the parameter byte P1.
        /// </summary>
        /// <returns> the value of the parameter byte P1. </returns>
        public virtual int P1 => _apdu[2] & 0xff;

        /// <summary>
        /// Returns the value of the parameter byte P2.
        /// </summary>
        /// <returns> the value of the parameter byte P2. </returns>
        public virtual int P2 => _apdu[3] & 0xff;

        /// <summary>
        /// Returns the number of data bytes in the command body (Nc) or 0 if this
        /// APDU has no body. This call is equivalent to
        /// <code>getData().length</code>.
        /// </summary>
        /// <returns> the number of data bytes in the command body or 0 if this APDU
        /// has no body. </returns>
        public virtual int Nc => _nc;

        /// <summary>
        /// Returns a copy of the data bytes in the command body. If this APDU as
        /// no body, this method returns a byte array with length zero.
        /// </summary>
        /// <returns> a copy of the data bytes in the command body or the empty
        ///    byte array if this APDU has no body. </returns>
        public virtual byte[] Data
        {
            get
            {
                byte[] data = new byte[_nc];
                Array.Copy(_apdu, _dataOffset, data, 0, _nc);
                return data;
            }
        }

        /// <summary>
        /// Returns the maximum number of expected data bytes in a response
        /// APDU (Ne).
        /// </summary>
        /// <returns> the maximum number of expected data bytes in a response APDU. </returns>
        public virtual int? Ne => _ne;

        /// <summary>
        /// Returns a copy of the bytes in this APDU.
        /// </summary>
        /// <returns> a copy of the bytes in this APDU. </returns>
#pragma warning disable S2365 // Properties should not make collection or array copies
        public virtual byte[] Bytes => (byte[])_apdu.Clone();
#pragma warning restore S2365 // Properties should not make collection or array copies

        /// <summary>
        /// Returns a string representation of this command APDU.
        /// </summary>
        /// <returns> a String representation of this command APDU. </returns>
        public override string ToString()
        {
            return "CommmandAPDU: " + _apdu.Length + " bytes, nc=" + _nc + ", ne=" + _ne;
        }

        /// <summary>
        /// we need to manually append Le to the command,
        /// otherwise it won't show up in the command bytes.
        /// </summary>
        /// <returns>The commandAPDU with Le appended to it.</returns>
        public void AppendLe()
        {
            using (var outputStream = new MemoryStream())
            {
                var repeat = 1;

                var cmdBytes = Bytes;

                if (0 == cmdBytes[4])
                    repeat = 2;

                using (var writer = new BinaryWriter(outputStream))
                {
                    writer.Write(cmdBytes);

                    for (var i = 0; i < repeat; i++)
                    {
                        writer.Write(new byte[] { 0x00 });
                    }

                    _apdu = outputStream.ToArray();
                }
            }
        }
    }
}
