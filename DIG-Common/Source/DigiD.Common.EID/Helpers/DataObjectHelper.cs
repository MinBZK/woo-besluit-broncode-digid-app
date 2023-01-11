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
using System.Collections.Generic;
using System.Linq;
using BerTlv;
using DigiD.Common.EID.Models;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.Helpers
{
    internal static class DataObjectHelper
    {
        internal static DataObject[] GetDataObjects(byte[] data)
        {
            var list = new List<DataObject>();

            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var bytesProcessed = 0;
                
                //Loop through the bytes and stop at last 2 bytes (which are the status bytes)
                while (data.Length > bytesProcessed)
                {
                    var totalBytes = data.Skip(bytesProcessed).ToArray();
                    var length = ByteHelper.GetLength(totalBytes);
                    var valueBytes = totalBytes.Take(length.Item1 + length.Item2).ToArray();

                    list.Add(GetDataObject(valueBytes));

                    bytesProcessed += valueBytes.Length;
                }

                return list.ToArray();
            }
            catch (Exception e)
            {
                Debugger.DumpInfo(e);
                throw;
            }
        }

        /// <summary>
        /// Extract Tag and Value from data
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static DataObject GetDataObject(byte[] bytes)
        {
            try
            {
                var tlvs = Tlv.ParseTlv(bytes);
                var tlv = tlvs.First();

                var result = new DataObject
                {
                    Tag = new[]{ (byte)tlv.Tag },
                    Value = tlv.Value
                };

                if (result.Tag[0] == 0x30 || result.Tag[0] == 0x31 || result.Tag[0] == 0x61 || result.Tag[0] == 0x7C) // Check if tag is parenttag
                    result.Children = GetDataObjects(result.Value);

                return result;
            }
            catch (Exception e)
            {
                Debugger.DumpInfo(e);
                throw;
            }
        }
    }
}
