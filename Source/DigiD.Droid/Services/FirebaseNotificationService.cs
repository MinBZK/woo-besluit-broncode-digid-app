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
﻿using Android.App;
using Android.Util;
using DigiD.Helpers;
using Firebase.Messaging;

namespace DigiD.Droid.Services
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseNotificationService : FirebaseMessagingService
    {
        private const string TAG = "FirebaseNotificationService";

        public override async void OnMessageReceived(RemoteMessage p0)
        {
            Log.Debug(TAG, "From: " + p0.From);

            if (p0.GetNotification() == null) return;

            // These is how most messages will be received
            Log.Debug(TAG, "Notification Message Body: " + p0.GetNotification().Body);
            await RemoteNotificationHelper.ConfirmShowNotifications(true);
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);

            if (MainActivity.IsInitialized)
                RemoteNotificationHelper.SetToken(token);
            else
                App.NewNotificationToken = token;
        }
    }
}
