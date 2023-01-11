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
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using DigiD.Common.Mobile.Helpers;
using DigiD.Droid.Services;
using Newtonsoft.Json;

namespace DigiD.Droid.Helpers
{
    [BroadcastReceiver(Enabled = true, Label = "DigiD Notification Receiver", Exported = false)]
    [IntentFilter(new[] { NotificationAction })]
    public class AlarmReceiver : BroadcastReceiver
    {
        private const string NotificationAction = "nl.rijksoverheid.digid.NOTIFY";
        public const string NotificationIdKey = "LocalNotificationId";
        private const string CHANNEL_ID = "location_notification";

        private NotificationManager _manager => (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

        public override void OnReceive(Context context, Intent intent)
        {
            System.Diagnostics.Debug.WriteLine($"==> DigiD - AlarmReceiver.OnReceive()");
            if (intent.Action.StartsWith(NotificationAction))
            {
                try
                {
                    var localNotification = JsonConvert.DeserializeObject<LocalNotification>(intent.GetStringExtra(NotificationIdKey));
                    System.Diagnostics.Debug.WriteLine($"==> DigiD - AlarmReceiver.OnReceive() - received notification#: {localNotification.Id}");

                    var intentToOpen = new Intent(context, typeof(MainActivity));
                    intentToOpen.SetFlags(ActivityFlags.SingleTop);
                    intentToOpen.PutExtra(NotificationIdKey, localNotification.Id);

                    var pendingIntent = PendingIntent.GetActivity(context, localNotification.Id, intentToOpen, PendingIntentFlags.CancelCurrent);

                    // Build the notification
                    var builder = new NotificationCompat.Builder(context, CHANNEL_ID);
                    builder.SetAutoCancel(true)    // dismiss the notification form the notification area when the ser clicks on it
                        .SetContentIntent(pendingIntent)    // start up this activity when the user clicks the intent
                        .SetSmallIcon(DigiD.Droid.Resource.Drawable.icon_android_statusbar_notification)  // this is the icon to display
                        .SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, DigiD.Droid.Resource.Drawable.digid_logo))
                        .SetVibrate(new long[] { 0, 500, 1000 })
                        .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate))
                        .SetLights(Color.Blue, 1000, 1000)
                        .SetContentTitle(localNotification.Title)
                        .SetContentText(localNotification.Body)
                        .SetTicker(localNotification.Body)
                        .SetStyle(new NotificationCompat.BigTextStyle(builder));

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        var channel = new NotificationChannel(CHANNEL_ID, "Notification", NotificationImportance.High)
                        {
                            LockscreenVisibility = NotificationVisibility.Public
                        };
                        _manager.CreateNotificationChannel(channel);
                    }

                    var notification = builder.Build();

                    System.Diagnostics.Debug.WriteLine($"==> DigiD - AlarmReceiver.OnReceive() - Notification with Id {localNotification.Id} send");
                    _manager.Notify("DigiD_App", localNotification.Id, notification);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"\n\n********** Invalid notification!!!! **********\n{e.Message}\n");
                }
            }
        }

        public void SetAlarm(LocalNotification notification)
        {
            System.Diagnostics.Debug.WriteLine($"==> DigiD - SetAlarm() - Notification {JsonConvert.SerializeObject(notification)} set at {notification.NotifyTime}");
            var intent = CreateIntent(Android.App.Application.Context, notification.Id);

            intent.PutExtra(NotificationIdKey, JsonConvert.SerializeObject(notification));

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, notification.Id, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var t2 = notification.NotifyTime.ToUnixTimeMs();

            System.Diagnostics.Debug.WriteLine($"EpochTime trigger: {t2}ms");

            var alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
            alarmManager?.Set(AlarmType.RtcWakeup, t2, pendingIntent);
        }

        private static Intent CreateIntent(Context context, int id)
        {
            return new Intent(context, typeof(AlarmReceiver)).SetAction($"{NotificationAction}_{id}");
        }

        public void CancelAlarm(int notificationId)
        {
            var intent = CreateIntent(Android.App.Application.Context, notificationId);
            var sender = PendingIntent.GetBroadcast(Android.App.Application.Context, notificationId, intent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);
            var alarmManager = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
            alarmManager?.Cancel(sender);
        }
    }
}
