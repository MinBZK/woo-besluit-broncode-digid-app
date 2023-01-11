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
﻿
using Android.Content;
using DigiD.Common.Controls;
using DigiD.Droid.CustomRenderers;
using DigiD.Droid.Extensions;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AccessibilityContentView), typeof(AccessibilityContentViewRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class AccessibilityContentViewRenderer : ViewRenderer
    {
        AccessibilityContentView AccessibilityContentView => (AccessibilityContentView)Element;

        public AccessibilityContentViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                this.SetAccessibilityElements();
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            SetAccessibilityElements();
            base.OnLayout(changed, l, t, r, b);
        }

        void SetAccessibilityElements()
        {
            var viewOrder = AccessibilityContentView.ViewOrder.OfType<BorderedButton>().ToList();

            if (viewOrder.Count != 2)
                return;

            var view1 = viewOrder[0].GetViewForAccessibility();
            var view2 = viewOrder[1].GetViewForAccessibility();

            if (view1 == null || view2 == null)
                return;

            view2.AccessibilityTraversalAfter = view1.Id;
            view1.AccessibilityTraversalBefore = view2.Id;

        }
    }
}
