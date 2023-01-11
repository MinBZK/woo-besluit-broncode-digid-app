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
using System.Globalization;
using System.IO;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.Models
{
    public class DataObject
    {
        public DataObject()
        {
            
        }

        public DataObject(byte[] tag, DataObject data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Tag = tag;
            Value = data.GetEncoded();
        }

        public DataObject(byte[] tag, byte[] data)
        {
            Tag = tag;
            Value = data;
        }

        public byte[] Tag { get; set; }
        public byte[] Value { get; set; }
        public byte[] Length => Value != null ? Value.Length.ToString("X2", CultureInfo.InvariantCulture).ConvertHexToBytes() : new byte[]{ 0x00 };
        public DataObject[] Children { get; set; }

        public string HexTag=> Tag.ToHexString();
        public string HexValue => Value.ToHexString();
        public string HexLength => Length.ToHexString();

        public byte[] GetEncoded()
        {
            using (var s = new MemoryStream())
            {
                using (var bw = new BinaryWriter(s))
                {
                    bw.Write(Tag);
                    if (Length.Length > 1)
                        bw.Write(new byte[]{0x82});

                    bw.Write(Length);

                    if (Value != null)
                        bw.Write(Value);

                    return s.ToArray();
                }
            }
        }
    }
}
