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
﻿using CoreGraphics;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.SessionModels;
using DigiD.iOS.CustomRenderers;
using DigiD.UI.Pages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

[assembly: ExportRenderer(typeof(BaseContentPage), typeof(CustomBaseContentPageRenderer))]
namespace DigiD.iOS.CustomRenderers
{
    public class CustomBaseContentPageRenderer : PageRenderer
    {
        private bool _isDisposed;
        
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            if (Element is BaseContentPage page && page.Content != null
                && page.On<Xamarin.Forms.PlatformConfiguration.iOS>().UsingSafeArea())
            {
                page.Content.Margin = new Thickness(
                    NativeView.SafeAreaInsets.Left,
                    NativeView.SafeAreaInsets.Top,
                    NativeView.SafeAreaInsets.Right,
                    OrientationHelper.IsInLandscapeMode ? 0 : NativeView.SafeAreaInsets.Bottom);
            }
        }

        public override UIViewController ChildViewControllerForStatusBarStyle()
        {
            var uiViewController = ChildViewControllers.Length >= 1 ? ChildViewControllers[0] : base.ChildViewControllerForStatusBarStyle();
            return uiViewController;
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            var currentUIStatusBarStyle = base.PreferredStatusBarStyle();

            if (ThemeHelper.IsInDarkMode && currentUIStatusBarStyle != UIStatusBarStyle.LightContent)
                return UIStatusBarStyle.LightContent;
            if (!ThemeHelper.IsInDarkMode && currentUIStatusBarStyle != UIStatusBarStyle.DarkContent)
                return UIStatusBarStyle.DarkContent;
            return base.PreferredStatusBarStyle();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _isDisposed = true;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            _isDisposed = false;

            if (!(e.NewElement is BaseContentPage newPage))
                return;

            if (!(newPage is MainMenuPage))
                newPage.BackgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["PageBackgroundColor"];
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            OrientationHelper.ResizeViews(Element, new Size(toSize.Width, toSize.Height), Element is LandingPage);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            OrientationHelper.ResizeViews(Element, Size.Zero, Element is LandingPage);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetNeedsStatusBarAppearanceUpdate();

            SetLayout();
        }

        private void SetLayout()
        {
            if (_isDisposed)
                return;

            if (ParentViewController == null || ParentViewController.IsBeingDismissed)
                return;

            var item = ParentViewController?.NavigationItem?.LeftBarButtonItem;

            if (item?.AccessibilityLabel != null && (item.AccessibilityLabel.StartsWith("icon_menu") || item.AccessibilityLabel.StartsWith("icon menu")))
            {
                if (AppSession.AccountStatus?.HasUnreadMessages == true)
                    item.AccessibilityLabel = $"{Common.AppResources.AccessibilityAppMainMenu}, {Common.AppResources.NewNotificationsHeader}";
                else
                    item.AccessibilityLabel = $"{Common.AppResources.AccessibilityAppMainMenu}";
            }
        }
    }
}
