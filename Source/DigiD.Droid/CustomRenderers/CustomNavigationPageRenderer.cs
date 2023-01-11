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
﻿using System.Collections.Generic;
using Android.Content;
using DigiD.Common;
using DigiD.Common.BaseClasses;
using DigiD.Common.Helpers;
using DigiD.Droid.CustomRenderers;
using Java.Interop;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android.AppCompat;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(CustomNavigationPageRenderer))]
namespace DigiD.Droid.CustomRenderers
{
    public class CustomNavigationPageRenderer : NavigationPageRenderer
    {

        public CustomNavigationPageRenderer(Context context) : base(context)
        {
        }
        
        public override void AddChildrenForAccessibility(IList<Android.Views.View> outChildren)
        {
            base.AddChildrenForAccessibility(outChildren);

            try
            {
                CheckAndModifyChildren(outChildren);
            }
            catch (System.Exception ex)
            {
                AppCenterHelper.TrackError(ex);
            }
        }

        private static void CheckAndModifyChildren(IEnumerable<View> outChildren)
        {
            foreach (var child in outChildren)
                CheckAndModifyView(child);
        }

        private static void CheckAndModifyView(IJavaPeerable child)
        {
            if (child is Android.Widget.ImageButton btn)
            {
                btn.Clickable = true;

                if (string.IsNullOrEmpty(btn.ContentDescription))
                    btn.ContentDescription = AppResources.AccessibilityNavigationBackButtonText;
                if (btn.ContentDescription.ToLowerInvariant().Equals("ok"))
                {
                    btn.ContentDescription = AppResources.AccessibilityAppMainMenu;
                }
                if (btn.ContentDescription.ToLowerInvariant().StartsWith("geen") ||
                    btn.ContentDescription.ToLowerInvariant().StartsWith("unlabeled"))
                {

                    btn.ImportantForAccessibility = Android.Views.ImportantForAccessibility.No;
                    btn.FocusableInTouchMode = false;
                }
            }
        }
    }
}
