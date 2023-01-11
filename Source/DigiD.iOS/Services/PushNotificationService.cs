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
﻿using System.Diagnostics;
using System.Threading.Tasks;
using DigiD.Common.Interfaces;
using DigiD.Common.Settings;
using DigiD.iOS.Services;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PushNotificationService))]
namespace DigiD.iOS.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public Task UnRegisterForRemoteNotifications()
        {
            UIApplication.SharedApplication.UnregisterForRemoteNotifications();
            return Task.CompletedTask;
        }

        public bool NotificationsEnabled()
        {
            var types = UIApplication.SharedApplication.CurrentUserNotificationSettings.Types;
            return types.HasFlag(UIUserNotificationType.Alert);
        }

        public bool NotificationsAvailable => DeviceInfo.DeviceType == DeviceType.Physical;

        public async Task<string> GetToken()
        {
            var granted = await RegisterForRemoteNotifications();

            if (granted)
            {
                var token = DependencyService.Get<IMobileSettings>().NotificationToken;

                var sw = Stopwatch.StartNew();

                while (string.IsNullOrEmpty(token) && sw.Elapsed.TotalSeconds < 5)
                {
                    System.Console.WriteLine("Token not found");
                    token = DependencyService.Get<IMobileSettings>().NotificationToken;
                    await Task.Delay(200);
                }
            }

            return DependencyService.Get<IMobileSettings>().NotificationToken;
        }

        private static Task<bool> RegisterForRemoteNotifications()
        {
            var tcs = new TaskCompletionSource<bool>();
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert |
                                                                  UNAuthorizationOptions.Badge |
                                                                  UNAuthorizationOptions.Sound,
                (granted, error) =>
                {
                    if (granted)
                        Device.BeginInvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);

                    tcs.SetResult(granted);
                });

            return tcs.Task;
        }
    }
}
