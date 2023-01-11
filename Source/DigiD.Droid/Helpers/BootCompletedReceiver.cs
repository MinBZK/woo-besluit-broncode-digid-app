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
using System.Globalization;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Droid.Services;
using DigiD.Helpers;

namespace DigiD.Droid.Helpers
{
    [BroadcastReceiver(Enabled = true, Exported = false, DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted})]
    public class BootCompletedReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            System.Diagnostics.Debug.WriteLine("==> DigiD - BootCompletedReceiver OnReceive");

            if (intent.Action != Intent.ActionBootCompleted) 
                return;

            context.StopService(new Intent(context, typeof(ScheduleService)));
            var preferences = Android.App.Application.Context.GetSharedPreferences("Notifications", FileCreationMode.Private);

            var language = "nl";
            if (preferences.Contains(nameof(GeneralPreferences.Language)))
                language = preferences.GetString(nameof(GeneralPreferences.Language), "nl");

            Localization.Init(CultureInfo.GetCultureInfo(language));

            var service = new LocalNotificationsService {FromBoot = true};
            LocalNotificationHelper.LocalNotificationService = service;

            await InitializeLocalNotificationsAfterReboot(service.Notifications);
        }

        public static async Task InitializeLocalNotificationsAfterReboot(Dictionary<NotificationType, DateTimeOffset> notifications)
        {
            System.Diagnostics.Debug.WriteLine($"==> DigiD - Now: {DateTimeOffset.UtcNow}");
            
            foreach (var (notificationType, date) in notifications)
            {
                if (date > DateTimeOffset.UtcNow)
                {
                    System.Diagnostics.Debug.WriteLine($"==> DigiD - Rescheduled() - {notificationType}: {date}");
                    await LocalNotificationHelper.SetNotification(date, notificationType);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"==> DigiD - Cancelled() - {notificationType}: {date}");
                    LocalNotificationHelper.CancelNotification(notificationType);
                }
            }
        }
    }
}
