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
using System.Globalization;

namespace DigiD.Common.NFC.Models
{
    public class ResponseApdu
    {
        /// <summary>
        /// @serial </summary>
        private readonly byte[] apdu;

        /// <summary>
        /// Constructs a ResponseApdu from a byte array containing the complete
        /// APDU contents (conditional body and trailed).
        /// 
        /// <para>Note that the byte array is cloned to protect against subsequent
        /// modification.
        /// 
        /// </para>
        /// </summary>
        /// <param name="apdu"> the complete response APDU
        /// </param>
        public ResponseApdu(byte[] apdu)
        {
            if (apdu == null)
                apdu = new byte[] { 0x62, 0x00 }; //No information given (NV-Ram not changed)

            apdu = (byte[])apdu.Clone();

            if (apdu.Length < 2)
                throw new ArgumentException("apdu must be at least 2 bytes long");

            this.apdu = apdu;
        }

        /// <summary>
        /// Returns the number of data bytes in the response body (Nr) or 0 if this
        /// APDU has no body. This call is equivalent to
        /// <code>getData().length</code>.
        /// </summary>
        /// <returns> the number of data bytes in the response body or 0 if this APDU
        /// has no body. </returns>
        public virtual int Nr => apdu.Length - 2;

        /// <summary>
        /// Returns a copy of the data bytes in the response body. If this APDU as
        /// no body, this method returns a byte array with a length of zero.
        /// </summary>
        /// <returns> a copy of the data bytes in the response body or the empty
        ///    byte array if this APDU has no body. </returns>
        public virtual byte[] Data
        {
            get
            {
                var data = new byte[apdu.Length - 2];
                Array.Copy(apdu, 0, data, 0, data.Length);
                return data;
            }
        }

        /// <summary>
        /// Returns the value of the status byte SW1 as a value between 0 and 255.
        /// </summary>
        /// <returns> the value of the status byte SW1 as a value between 0 and 255. </returns>
        public virtual int SW1 => apdu[apdu.Length - 2] & 0xff;

        /// <summary>
        /// Returns the value of the status byte SW2 as a value between 0 and 255.
        /// </summary>
        /// <returns> the value of the status byte SW2 as a value between 0 and 255. </returns>
        public virtual int SW2 => apdu[apdu.Length - 1] & 0xff;

        /// <summary>
        /// Returns the value of the status bytes SW1 and SW2 as a single
        /// status word SW.
        /// It is defined as
        /// <code>(getSW1() << 8) | getSW2()</code>.
        /// </summary>
        /// <returns> the value of the status word SW. </returns>
        public virtual int SW => (SW1 << 8) | SW2;

        /// <summary>
        /// Returns a copy of the bytes in this APDU.
        /// </summary>
        /// <returns> a copy of the bytes in this APDU. </returns>
#pragma warning disable S2365 // Properties should not make collection or array copies
        public virtual byte[] Bytes => (byte[])apdu.Clone();
#pragma warning restore S2365 // Properties should not make collection or array copies

        /// <summary>
        /// Returns a string representation of this response APDU.
        /// </summary>
        /// <returns> a String representation of this response APDU. </returns>
        public override string ToString()
        {
            return "ResponseApdu: " + apdu.Length + " bytes, SW=" + SW.ToString("x", CultureInfo.InvariantCulture);
        }
    }
}
