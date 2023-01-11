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
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Extensions;
using Android.OS;
using AndroidX.Core.App;
using DigiD.Common.Interfaces;
using DigiD.Droid.Services;
using Firebase.Messaging;
using Xamarin.Forms;

[assembly: Dependency(typeof(PushNotificationService))]
namespace DigiD.Droid.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public async Task UnRegisterForRemoteNotifications()
        {
            try
            {
                await FirebaseMessaging.Instance.DeleteToken();
            }
            catch (Exception)
            {
                //Always ignore exception
            }
            
        }

        public bool NotificationsEnabled()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var nm = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);

                if (!nm.AreNotificationsEnabled())
                    return false;

                return nm.NotificationChannels == null || nm.NotificationChannels.All(channel => channel.Importance != NotificationImportance.None);
            }

            return NotificationManagerCompat.From(Android.App.Application.Context).AreNotificationsEnabled();
        }

        public bool NotificationsAvailable
        {
            get
            {
                var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Xamarin.Essentials.Platform.CurrentActivity);
                
                if (resultCode == ConnectionResult.Success) 
                    return true;

                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    // User can fix it.
                    var dict = new Dictionary<string, string>
                    {
                        { "resultCode", resultCode.ToString() },
                        { "errorString", GoogleApiAvailability.Instance.GetErrorString(resultCode) }
                    };

                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("google_play_service", dict);
                }
                else
                {
                    Microsoft.AppCenter.Analytics.Analytics.TrackEvent("google_play_service_not_supported");
                }
                return false;

            }
        }

        public async Task<string> GetToken()
        {
            try
            {
                var token = await FirebaseMessaging.Instance.GetToken();
                return token.ToString();
            }
            catch
            {
                Console.WriteLine("FireBase not initialized");
                return null;
            }
        }
    }
}
