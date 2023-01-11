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
﻿using System.Threading.Tasks;
using Android.Content;
using Android.Views.Accessibility;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Droid.Services;
using Xamarin.Forms;
using DigiD.Droid.Extensions;

[assembly: Dependency(typeof(A11YService))]
namespace DigiD.Droid.Services
{
    public class A11YService : IA11YService
    {
        private readonly AccessibilityManager _accessibilityManager = (AccessibilityManager)Xamarin.Essentials.Platform.CurrentActivity.GetSystemService(Context.AccessibilityService);

        public bool IsInVoiceOverMode()
        {
            return _accessibilityManager.IsTouchExplorationEnabled;
        }

        public async Task Speak(string text, int pauseInMs = 0)
        {
            if (IsInVoiceOverMode() && !string.IsNullOrEmpty(text))
            {
                if (pauseInMs > 0)
                    await Task.Delay(pauseInMs);
                Xamarin.Essentials.Platform.CurrentActivity.Window?.DecorView?.AnnounceForAccessibility(text.StripHtml());
            }
        }

        public async Task NotifyForNewPage()
        {
            if (!IsInVoiceOverMode())
                return;
            System.Diagnostics.Debug.WriteLine("\n====> A11YService.NotifyForNewPage()...");
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                var @event = AccessibilityEvent.Obtain();
                if (@event != null)
                {
                    @event.EventType = EventTypes.WindowsChanged;
                    await Task.Delay(250);
                    _accessibilityManager.SendAccessibilityEvent(@event);
                }
            });
        }

        public async Task ChangeA11YFocus(VisualElement visualElement)
        {
            if (!IsInVoiceOverMode() || visualElement == null)
                return;

            var view = visualElement.GetViewForAccessibility();
            await Task.Delay(250);
            view?.SendAccessibilityEvent(EventTypes.ViewAccessibilityFocused);
        }
    }
}
