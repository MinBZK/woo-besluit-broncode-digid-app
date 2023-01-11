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
using Android.Content;
using DigiD.Common.Enums;
using DigiD.Common.Mobile.Interfaces;
using DigiD.Droid.Services;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(LocalNotificationsService))]
namespace DigiD.Droid.Services
{
    public class LocalNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public DateTimeOffset NotifyTime { get; set; }
    }

    public class LocalNotificationsService : ILocalNotifications
    {
        internal bool FromBoot;

        public void Cancel(int id)
        {
            ScheduleService.AlarmReceiver.Value.CancelAlarm(id);

            var notifications = Notifications;

            if (notifications.ContainsKey((NotificationType)id))
                notifications.Remove((NotificationType)id);

            Notifications = notifications;
        }

        public void Show(string title, string message, int id = 0)
        {
            Show(title, message, id, DateTime.UtcNow);
        }

        public void Show(string title, string message, int id, DateTimeOffset notifyTime)
        {
            ScheduleService.AlarmReceiver.Value.SetAlarm(new LocalNotification
            {
                Body = message,
                Id = id,
                NotifyTime = notifyTime,
                Title = title
            });

            if (FromBoot) 
                return;

            var notifications = Notifications;

            if (notifications.ContainsKey((NotificationType) id))
                notifications[(NotificationType) id] = notifyTime;
            else
                notifications.Add((NotificationType)id, notifyTime);

            Notifications = notifications;
        }

        public Dictionary<NotificationType, DateTimeOffset> Notifications
        {
            get
            {
                var preferences = Android.App.Application.Context.GetSharedPreferences("Notifications", FileCreationMode.Private);
                var notifications = new Dictionary<NotificationType, DateTimeOffset>();

                if (preferences.Contains(nameof(Notifications)))
                {
                    var value = preferences.GetString(nameof(Notifications), JsonConvert.SerializeObject(new Dictionary<NotificationType, DateTimeOffset>()));
                    if (!string.IsNullOrEmpty(value))
                        notifications = JsonConvert.DeserializeObject<Dictionary<NotificationType, DateTimeOffset>>(value);
                }

                return notifications;
            }
            set
            {
                var preferences = Android.App.Application.Context.GetSharedPreferences("Notifications", FileCreationMode.Private);
                preferences.Edit().PutString(nameof(Notifications), JsonConvert.SerializeObject(value)).Commit();
            }
        }
    }
}
