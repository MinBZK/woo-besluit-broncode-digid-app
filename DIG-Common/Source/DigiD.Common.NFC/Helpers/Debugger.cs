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
using System.Text;
using DigiD.Common.NFC.Models;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace DigiD.Common.NFC.Helpers
{
    public static class Debugger
    {
        public static void WriteLine(string line)
        {
#if DUMP_INFO
            System.Diagnostics.Debug.WriteLine("[DigiD] " +line + "\r\n");
#else
            //Just for debugging
#endif
        }

        public static void DumpInfo(Exception ex)
        {
#if DUMP_INFO
            System.Diagnostics.Debug.WriteLine("[DigiD] " + ex + "\r\b");
#else
            //Just for debugging
#endif
        }

        public static void DumpInfo(string header, CommandApdu command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var sb = new StringBuilder();

            if (command.IsSecureMessage)
                sb.Append("SM: ");

            sb.AppendLine($"++++ START {header}");

            sb.AppendLine($"CLA: {(int)command.CLA:X2}");
            sb.AppendLine($"INS: {(int)command.INS:X2}");
            sb.AppendLine($"P1: {command.P1:X2}");
            sb.AppendLine($"P2: {command.P2:X2}");
            sb.AppendLine($"Nc: {command.Nc:X2}");
            sb.AppendLine(command.Ne.HasValue ? $"Ne: {command.Ne:X2}" : "Ne: NULL");
            sb.AppendLine("---------------");
            sb.AppendLine($"Data: {command.Data.ToHexString()}");
            sb.AppendLine($"Bytes: {command.Bytes.ToHexString()}");

            WriteLine(sb.ToString());
        }

        public static void DumpInfo(string header, ResponseApdu response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var sb = new StringBuilder();

            sb.AppendLine(DumpBytes($"++++ RESPONSE {header}", response.Bytes));
            sb.AppendLine("");
            sb.AppendLine($"SW1: {response.SW1:X2}");
            sb.AppendLine($"SW2: {response.SW2:X2}");
            sb.AppendLine($"Nr: {response.Nr:X2}");
            sb.AppendLine("");
            sb.AppendLine($"++++ END {header}");
            sb.AppendLine("############################");

            WriteLine(sb.ToString());
        }

        public static void DumpInfo(string title, byte[] bytes)
        {
            var sb = new StringBuilder();

            using (var s = new Asn1InputStream(bytes))
            {
                sb.AppendLine($"File: {title}");
                sb.AppendLine(Asn1Dump.DumpAsString(s.ReadObject()));
            }

            WriteLine(sb.ToString());
        }

        private static string DumpBytes(string title, IReadOnlyList<byte> buffer)
        {
            return buffer == null ? $"{title} (0)" : $"{title} ({buffer.Count})\r\n{DumpBytes(buffer)}";
        }

        private static string DumpBytes(IReadOnlyList<byte> buffer)
        {
            var sb = new StringBuilder();

            const int step = 16;
            var lineBuffer = new StringBuilder(step);
            for (var n = 0; n < buffer.Count; n += step)
            {
                sb.Append($"{Hex.ToHexString(new[] { (byte)(n / (step * step)), (byte)n })}   ");
                for (var i = 0; i < step; i++)
                {
                    if (i + n < buffer.Count)
                    {
                        sb.Append($"{new[] { buffer[i + n] }.ToHexString()} ");
                    }
                    else
                    {
                        sb.Append("   ");
                        lineBuffer.Append(' ');
                    }
                }

                sb.AppendLine("  " + lineBuffer);

                lineBuffer.Clear();
            }

            return sb.ToString();
        }
    }
}
