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
using DigiD.Common.EID.Models;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.NFC.Enums;
using DigiD.Common.NFC.Helpers;

namespace DigiD.Common.EID.Helpers
{
    public static class CardHelper
    {
        private static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static Card GetCardByATR(byte[] atr)
        {
            if (atr == null)
                return null;

            Console.WriteLine($"ATR Card Scanned: {atr.ToHexString()}");

            var card = Card.AvailableCards.FirstOrDefault(x =>
            {
                Console.WriteLine($"Card: {x.ATR.ToHexString()}");

                if (EIDSession.IsDesktop)
                    return atr.ToHexString().Contains(x.ATR.ToHexString());

                if (x.ATR.Length == atr.Length)
                    return x.ATR.SequenceEqual(atr);

                return x.ATR.TakeLast(atr.Length + 1).Take(atr.Length).SequenceEqual(atr);
            });

            return card;
        }

        public static void SetCard(DocumentType documentType)
        {
            EIDSession.Card = Card.AvailableCards.FirstOrDefault(x => x.DocumentType == documentType);
        }
    }
}
