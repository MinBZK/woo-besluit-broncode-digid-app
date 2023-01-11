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
﻿using DigiD.Common.BaseClasses;
using DigiD.Common.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.Common.Mobile.Helpers
{
    public static class OrientationHelper
    {
        public static bool IsInLandscapeMode => DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape;
        public static double TabletPaddingWidth => IsInLandscapeMode && Device.Idiom == TargetIdiom.Tablet ? DisplayHelper.Width * .2 : 0;    // Bij normale pages 20% nemen.

        public static void ResizeViews(VisualElement visualElement, Size toSize, bool isLandingPage)
        {
            var isLandscape = toSize.IsZero ? IsInLandscapeMode : toSize.Width > toSize.Height;

            SetNavigationPage(Application.Current.MainPage);

            var padding = isLandingPage || Device.Idiom == TargetIdiom.Phone ? 0.0 : OrientationHelper.TabletPaddingWidth;

            if (visualElement is CommonBaseContentPage contentPage)
            {
                SetContentPage(padding, contentPage, isLandscape);
            }
        }

        private static void SetNavigationPage(Element navPage)
        {
            navPage.SetDynamicResource(VisualElement.BackgroundColorProperty, "PageBackgroundColor");
            SetAsDemoBarIfNecessary(navPage);
        }

        private static void SetAsDemoBarIfNecessary(Element navPage)
        {
            if (navPage is NavigationPage navigationPage)
            {
                if (IsInLandscapeMode && DemoHelper.CurrentUser != null)
                {
                    navigationPage.SetDynamicResource(NavigationPage.BarBackgroundColorProperty, "DemoBarColor");
                    navigationPage.CurrentPage.Title = $"{DemoHelper.CurrentUser.UserName} - {((CommonBaseViewModel)navigationPage.CurrentPage.BindingContext).PageId}";
                    navigationPage.SetValue(NavigationPage.BarTextColorProperty, Color.Black);
                }
                else
                {
                    navigationPage.SetDynamicResource(NavigationPage.BarBackgroundColorProperty, "PageBackgroundColor");
                    navigationPage.CurrentPage.Title = null;
                    navigationPage.SetDynamicResource(NavigationPage.BarTextColorProperty, "BarTextColor");
                }
            }
        }

        private static void SetContentPage(double padding, ContentPage contentPage, bool isLandscape)
        {
            if (isLandscape)
            {
                contentPage.BackgroundColor = (Color)Application.Current.Resources["PageBackgroundColor"];
                if (contentPage.Content != null)
                    contentPage.Content.BackgroundColor = (Color)Application.Current.Resources["PageBackgroundColor"];
                contentPage.Padding = new Thickness(padding, Device.Idiom == TargetIdiom.Phone ? 0 : 20, padding, 0);
            }
            else
            {
                if (contentPage.BindingContext is CommonBaseViewModel bvm)
                    NavigationPage.SetHasBackButton(contentPage, bvm.HasBackButton);

                contentPage.BackgroundColor = (Color)Application.Current.Resources["PageBackgroundColor"];
                if (contentPage.Content != null)
                    contentPage.Content.BackgroundColor = (Color)Application.Current.Resources["PageBackgroundColor"];
                contentPage.Padding = new Thickness(0, 0);
            }
        }
    }
}
