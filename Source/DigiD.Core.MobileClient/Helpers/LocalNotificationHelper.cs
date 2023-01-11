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
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Interfaces;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    public static class LocalNotificationHelper
    {
        private static ILocalNotifications _service;
        public static ILocalNotifications LocalNotificationService
        {
            get => _service ??= DependencyService.Get<ILocalNotifications>();
            set => _service = value;
        }

        public static void CancelNotification(NotificationType type)
        {
            LocalNotificationService.Cancel((int)type);
        }

        public static async Task<bool> SetNotification(DateTimeOffset date, NotificationType type)
        {
            var title = string.Empty;
            var message = string.Empty;

            switch (type)
            {
                case NotificationType.LetterFirst:
                    title = AppResources.LetterNotificationHeader;
                    message = AppResources.FirstLetterNotificationMessage;
                    break;
                case NotificationType.LetterSecond:
                    title = AppResources.LetterNotificationHeader;
                    message = AppResources.SecondLetterNotificationMessage;
                    break;
                case NotificationType.IDcheck:
                    title = AppResources.IDCheckNotificationTitle;
                    message = AppResources.IDCheckNotificationMessage;
                    break;
            }

            if (await DependencyService.Get<IDevice>().AskForLocalNotificationPermission())
            {
                LocalNotificationService.Show(title, message, (int)type, date);
                return true;
            }

            return false;
        }
#pragma warning disable S3168 // "async" methods should not return "void"
        public static async void HandleIncomingNotification(NotificationType type)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            switch (type)
            {
                case NotificationType.LetterFirst:
                case NotificationType.LetterSecond:
                {
                    if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Pending)
                        await ActivationHelper.ContinueLetterActivation();
                    else
                        await DependencyService.Get<INavigationService>().PopToRoot();
                    break;
                }
                case NotificationType.IDcheck:
                {
                    if (DependencyService.Get<IMobileSettings>().LoginLevel == LoginLevel.Midden)
                        await RdaHelper.Init(false);
                    else
                        await DependencyService.Get<INavigationService>().PopToRoot();

                    break;
                }
            }
        }
    }
}
