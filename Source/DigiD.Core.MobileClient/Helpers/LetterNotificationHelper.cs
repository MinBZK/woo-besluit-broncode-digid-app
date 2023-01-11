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
using System.Threading.Tasks;
using DigiD.Common.Enums;

namespace DigiD.Helpers
{
    public static class LetterNotificationHelper
    {
#if PROD || PREPROD
        public static readonly TimeSpan FirstNotificationAfter = TimeSpan.FromDays(4);
        public static readonly TimeSpan SecondNotificationAfter = TimeSpan.FromDays(15);
#else
        public static readonly TimeSpan FirstNotificationAfter = TimeSpan.FromMinutes(2);
        public static readonly TimeSpan SecondNotificationAfter = TimeSpan.FromMinutes(4);
#endif

        internal static async Task<bool> CreateLetterNotification()
        {
            LocalNotificationHelper.LocalNotificationService.Cancel((int)NotificationType.LetterFirst);
            LocalNotificationHelper.LocalNotificationService.Cancel((int)NotificationType.LetterSecond);

            var now = DateTimeOffset.UtcNow;

#if PROD || PREPROD
            now = now.Date.AddHours(19);
#endif

            var result = await LocalNotificationHelper.SetNotification(now.Add(FirstNotificationAfter), NotificationType.LetterFirst);

            if (result)
                result = await LocalNotificationHelper.SetNotification(now.Add(SecondNotificationAfter), NotificationType.LetterSecond);

            return result;
        }

        internal static void ResetNotifications()
        {
            LocalNotificationHelper.CancelNotification(NotificationType.LetterFirst);
            LocalNotificationHelper.CancelNotification(NotificationType.LetterSecond);
        }
    }
}
