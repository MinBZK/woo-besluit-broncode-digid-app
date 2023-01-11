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
﻿using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.iOS.Services;
using UIKit;
using Xamarin.Forms;
using DigiD.iOS.Extensions;
using System.Threading.Tasks;

[assembly: Dependency(typeof(A11YService))]
namespace DigiD.iOS.Services
{
    public class A11YService : IA11YService
    {
        public Task ChangeA11YFocus(VisualElement visualElement)
        {
            if (!UIAccessibility.IsVoiceOverRunning || visualElement == null)
                return Task.CompletedTask;

            var nativeView = visualElement.GetViewForAccessibility();
            if (nativeView != null)
                UIAccessibility.PostNotification(UIAccessibilityPostNotification.LayoutChanged, nativeView);

            return Task.CompletedTask;
        }

        public bool IsInVoiceOverMode()
        {
            return UIAccessibility.IsVoiceOverRunning;
        }

        public Task NotifyForNewPage()
        {
            if (UIAccessibility.IsVoiceOverRunning)
            {
                UIAccessibility.PostNotification(UIKit.UIAccessibilityPostNotification.ScreenChanged, null);
            }

            return Task.CompletedTask;
        }

        public async Task Speak(string text, int pauseInMs = 0)
        {
            if (UIAccessibility.IsVoiceOverRunning && !string.IsNullOrEmpty(text))
            {
                if (pauseInMs > 0)
                    await Task.Delay(pauseInMs);

                UIAccessibility.PostNotification(UIAccessibilityPostNotification.Announcement, Foundation.NSObject.FromObject(text.StripHtml()));
            }
        }
    }
}
