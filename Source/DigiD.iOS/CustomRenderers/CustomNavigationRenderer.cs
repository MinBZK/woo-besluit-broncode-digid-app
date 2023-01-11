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
using System.Linq;
using DigiD.Common;
using DigiD.Common.BaseClasses;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer (typeof (CustomNavigationPage), typeof (CustomNavigationRenderer))] //Custom renderer to set left toolbar item and custom font for title
namespace DigiD.iOS.CustomRenderers
{
	internal class CustomNavigationRenderer : NavigationRenderer
	{
        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            var currentUIStatusBarStyle = base.PreferredStatusBarStyle();

            if (ThemeHelper.IsInDarkMode && currentUIStatusBarStyle != UIStatusBarStyle.LightContent)
                return UIStatusBarStyle.LightContent;
            if (!ThemeHelper.IsInDarkMode && currentUIStatusBarStyle != UIStatusBarStyle.DarkContent)
                return UIStatusBarStyle.DarkContent;

            return base.PreferredStatusBarStyle();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad();

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                NavigationBar.CompactAppearance.ShadowColor = null;
                NavigationBar.StandardAppearance.ShadowColor = null;
                NavigationBar.ScrollEdgeAppearance.ShadowColor = null;
                NavigationBar.ShadowImage = new UIImage(); // Deze is nog wel nodig!!
            }

            ModalPresentationStyle = UIModalPresentationStyle.None;

            if (!DependencyService.Get<IA11YService>().IsInVoiceOverMode())
                return;

            var nav = NativeView.Subviews.ToList().OfType<UINavigationBar>().FirstOrDefault();

            var navItem = nav?.Items.ToList().FirstOrDefault(x => x.LeftBarButtonItem != null);

            if (navItem?.LeftBarButtonItem != null)
            {
                if (AppSession.AccountStatus?.HasUnreadMessages == true)
                    navItem.LeftBarButtonItem.AccessibilityLabel = $"{AppResources.AccessibilityAppMainMenu}, {AppResources.NewNotificationsHeader}";
                else
                    navItem.LeftBarButtonItem.AccessibilityLabel = AppResources.AccessibilityAppMainMenu;
            }
        }
    }
}
