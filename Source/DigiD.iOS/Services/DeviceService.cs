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
using System.Net;
using System.Threading.Tasks;
using CoreGraphics;
using DigiD.Common.Enums;
using DigiD.Common.Services;
using DigiD.iOS.Helpers;
using DigiD.iOS.Services;
using Foundation;
using UIKit;
using UserNotifications;
using WebKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceService))]
namespace DigiD.iOS.Services
{
    internal class DeviceService : IDevice
    {
        public int PiwikId => App.PiwikId;

        public async Task<string> UserAgent()
        {
            try
            {
                using (var view = new WKWebView(CGRect.Empty, new WKWebViewConfiguration()))
                {
                    var userAgent = await view.EvaluateJavaScriptAsync("navigator.userAgent");
                    return userAgent.ToString();
                }
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        public string RuntimePlatform => Device.RuntimePlatform;

        public string DeviceTypeName => Device.Idiom == TargetIdiom.Tablet ? "iPad" : "iPhone";

        public bool HasHomeButton
        {
            get
            {
                var window = UIApplication.SharedApplication.Delegate.GetWindow();
                var bottom = window?.SafeAreaInsets.Bottom;

                //Devices which has a home button, does not have a safe-area inset.
                return bottom == 0;
            }
        }

        public string DefaultLanguage => NSLocale.PreferredLanguages[0].Substring(0, 2);
        public bool IsScreenCaptured => UIScreen.MainScreen.Captured;

        // De app blijft goed werken ook al staat het device in "Developer Mode", daarom
        // wordt 'false' teruggegeven, dit property wordt alleen gebruik voor de SupportCode.
        // Als true wordt teruggegeven dan zal de helpdesk onjuiste info teruggeven richting gebruiker.
        public bool IsInDeveloperMode => false;

        public void OpenSettings()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                UIApplication.SharedApplication.OpenUrl(new Uri(UIApplication.OpenSettingsUrlString));
            });
        }

        public bool OpenBrowser(string name, string uri)
        {
            return true;
        }

        public Task<bool> AskForLocalNotificationPermission()
        {
            var task = new TaskCompletionSource<bool>();
            // Ask the user for permission to get notifications on iOS 10.0+
            UNUserNotificationCenter.Current.BeginInvokeOnMainThread(async () =>
            {
                var (result, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound);
                if (result)
                {
                    UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
                }

                task.TrySetResult(result);
            });

            return task.Task;
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public void CloseApp()
        {
            throw new NotImplementedException();
        }

        public SystemFontSize GetSystemFontSize()
        {
            var preferredSize = UIApplication.SharedApplication.PreferredContentSizeCategory.ToString();
            SystemFontSize result;

                switch (preferredSize)
                {
                    case "UICTContentSizeCategoryXS":
                        result = SystemFontSize.XS;
                        break;
                    case "UICTContentSizeCategoryS":
                        result = SystemFontSize.S;
                        break;
                    case "UICTContentSizeCategoryM":
                        result = SystemFontSize.M;
                        break;
                    case "UICTContentSizeCategoryL":
                        result = SystemFontSize.L;
                        break;
                    case "UICTContentSizeCategoryXL":
                        result = SystemFontSize.XL;
                        break;
                    case "UICTContentSizeCategoryXXL":
                        result = SystemFontSize.XXL;
                        break;
                    case "UICTContentSizeCategoryXXXL":
                        result = SystemFontSize.XXXL;
                        break;
                    case "UICTContentSizeCategoryAccessibilityM":
                        result = SystemFontSize.ExtraM;
                        break;
                    case "UICTContentSizeCategoryAccessibilityL":
                    case "UICTContentSizeCategoryAccessibilityXL":
                    case "UICTContentSizeCategoryAccessibilityXXL":
                    case "UICTContentSizeCategoryAccessibilityXXXL":
                        result = SystemFontSize.ExtraL;
                        break;
                    default:
                        result = SystemFontSize.M;
                        break;
                }


            return result;
        }
    }
}
