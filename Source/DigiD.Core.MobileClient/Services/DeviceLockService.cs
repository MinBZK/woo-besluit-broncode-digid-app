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
using System.Diagnostics;
using System.Threading.Tasks;
using DigiD.Common.BaseClasses;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using Xamarin.Forms;

namespace DigiD.Services
{
    public static class DeviceLockService
    {
        public static event EventHandler<EventArgs> DeviceLocked;
        private static DateTimeOffset? _lastKeyPress;

        private static readonly Timer Timer = new Timer(TimeSpan.FromSeconds(1), async () =>
        {
            if (_lastKeyPress == null)
                return;

            if (DateTimeOffset.UtcNow - _lastKeyPress.Value > AppConfigConstants.DisplayLockTimeout)
            {
                LockApp();
            }

            await Task.Delay(0);
        });

        static DeviceLockService()
        {
            Timer.Start();
        }

        public static void LockApp()
        {
            AppSession.AuthenticationSessionId = null;
            _lastKeyPress = null;
            DeviceLocked?.Invoke(null, EventArgs.Empty);
            Debug.WriteLine($"DeviceLockService elapsed - {DateTime.Now}");
        }

        public static void SetTimer()
        {
            if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.NotActivated)
            {
                _lastKeyPress = null;
                return;
            }

            if (Application.Current.MainPage is CustomNavigationPage navPage && navPage.CurrentPage.BindingContext is CommonBaseViewModel {PreventLock: true})
            {
                _lastKeyPress = null;
                return;
            }
            
            _lastKeyPress = DateTimeOffset.Now;
            Debug.WriteLine($"DeviceLockService reset - {_lastKeyPress}");
        }
    }
}
