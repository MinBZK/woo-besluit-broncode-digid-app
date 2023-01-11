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
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Helpers;
using DigiD.Services;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using Foundation;
using LabelHtml.Forms.Plugin.iOS;
using Syncfusion.XForms.iOS.ProgressBar;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

namespace DigiD.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate, IUIGestureRecognizerDelegate
    {
        private bool SystemSettingsChanged { get; set; }

        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            HtmlLabelRenderer.Initialize();
            Forms.Init();
            CachedImageRenderer.Init();

            SfCircularProgressBarRenderer.Init();

            UIApplication.SharedApplication.StatusBarHidden = true;

#if DEBUG
            var config = new FFImageLoading.Config.Configuration
            {
                VerboseLogging = false,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
                Logger = new Common.CustomLogger(),
            };
            ImageService.Instance.Initialize(config);
#endif

#if TESTCLOUD
            Xamarin.Calabash.Start();
#endif

            if (!NSUserDefaults.StandardUserDefaults.BoolForKey("firstRun"))
            {
                if (!DependencyService.Get<ISecurityService>().IsRunningOnVirtualDevice)
                    App.Reset();

                NSUserDefaults.StandardUserDefaults.SetBool(true, "firstRun");
            }

            LoadApplication(new App());
            UISwitch.Appearance.OnTintColor = ((Color)Xamarin.Forms.Application.Current.Resources["PrimaryColor"]).ToUIColor();

            UIApplication.Notifications.ObserveContentSizeCategoryChanged((_, _) => SystemSettingsChanged = true);

            if (launchOptions != null && launchOptions.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey) && launchOptions[UIApplication.LaunchOptionsLocalNotificationKey] is UILocalNotification localNotification)
            {
                // check for a local notification
                LocalNotificationHelper.HandleIncomingNotification((NotificationType)Convert.ToInt32(localNotification.AlertAction));
            }

            Xamarin.Forms.Application.Current.RequestedThemeChanged += (_, _) => SetThemeChanged();

            SetThemeChanged();

            var ret = base.FinishedLaunching(uiApplication, launchOptions);

            if (ret)
            {
                UITapGestureRecognizer tap = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("gestureRecognizer:shouldReceiveTouch:"));
                tap.Delegate = (IUIGestureRecognizerDelegate)Self;
                uiApplication.KeyWindow?.AddGestureRecognizer(tap);
            }

            return ret;
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            DeviceLockService.SetTimer();
            return false;
        }

        private static void SetThemeChanged()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
            UIApplication.SharedApplication.StatusBarStyle = ThemeHelper.IsInDarkMode
                ? UIStatusBarStyle.LightContent
                : UIStatusBarStyle.DarkContent;
        }


        /// <summary>
        /// Universal link
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userActivity"></param>
        /// <param name="completionHandler"></param>
        /// <returns></returns>
        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            return HandleIncomingData(userActivity.WebPageUrl);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (url == null)
                return false;

            if (url.Host.StartsWith("app-app"))
            {
                HttpSession.IsApp2AppSession = true;
                App.ShowSplashScreen(true, false);
                HttpSession.SourceApplication = "ExternalApp";

                Task.Run(async () =>
                {
                    try
                    {
                        var response = await DependencyService.Get<IVersionService>().CheckVersion();
                        if (response.ApiResult == ApiResult.Ok && response.Action == "active")
                        {
                            await App2AppHelper.ProcessSamlRequest(url.Host);
                        }

                    }
                    catch (Exception ex)
                    {
                        AppCenterHelper.TrackError(ex);
                    }
                });

                return true;
            }
            var u = url.ToString().Replace("://", ":");
            IncomingDataHelper.HandleIncomingData(new Uri(u));

            return true;
        }

        private static bool HandleApp2App(string[] query)
        {
            HttpSession.IsApp2AppSession = true;
            App.ShowSplashScreen(true, false);

            HttpSession.SourceApplication = "ExternalApp";

            Task.Run(async () =>
            {
                try
                {
                    var response = await DependencyService.Get<IVersionService>().CheckVersion();
                    if (response.ApiResult == ApiResult.Ok && (response.Action == "active" || response.Action == "update_warning"))
                    {
                        await App2AppHelper.ProcessSamlRequest(query[0]);
                    }
                }
                catch (Exception ex)
                {
                    AppCenterHelper.TrackError(ex);
                }
            });

            return true;
        }

        private static bool HandleWeb2App(NSUrl userActivity)
        {
            var query = userActivity.Query.Split('&');

            string data;

            if (query.Length > 1 && query[0].StartsWith("browser=") && query[1].StartsWith("data="))
            {
                data = userActivity.Query.Replace($"{query[0]}&data=", string.Empty);

                if (!data.Contains("browser"))
                    data += "&" + query[0];
            }
            else if (query[0].StartsWith("data="))
            {
                data = query[0].Replace("data=", string.Empty);
            }
            else
                return false;

            var regex = new Regex(Regex.Escape("://"));
            var url = regex.Replace(WebUtility.UrlDecode(data), ":", 1);

            IncomingDataHelper.HandleIncomingData(new Uri(url));

            return true;
        }

        private static bool HandleIncomingData(NSUrl userActivity)
        {
            try
            {
                var query = userActivity.Query.Split('&');

                if (query[0].StartsWith("app-app=", StringComparison.CurrentCultureIgnoreCase))
                    return HandleApp2App(query);

                return HandleWeb2App(userActivity);
            }
            catch (Exception e)
            {
                AppCenterHelper.TrackError(e, new Dictionary<string, string>
                {
                    {"WebPageUrl",Base64Encode(userActivity.ToString()) },
                    {"Query",Base64Encode(userActivity.Query) },
                });
            }

            return true;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            try
            {
                var window = uiApplication.KeyWindow;

                var v = new UIView(window.Frame)
                {
                    BackgroundColor = ((Color)Xamarin.Forms.Application.Current.Resources["PrimaryColor"]).ToUIColor(),
                    AutoresizingMask = UIViewAutoresizing.FlexibleDimensions
                };

                var centerX = (float)window.Frame.Width / 2;
                var centerY = (float)window.Frame.Height / 2;

                var topImage = new UIImageView(UIImage.FromBundle("vaantje.png"))
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleMargins
                };

                var logo = new UIImageView(UIImage.FromBundle("splash.png"))
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleMargins
                };

                logo.Frame = new RectangleF(
                    centerX - ((float)logo.Frame.Width / 2),
                    centerY - (float)logo.Frame.Height / 2,
                    (float)logo.Frame.Width,
                    (float)logo.Frame.Height);

                topImage.Frame = new RectangleF(
                    centerX - ((float)topImage.Frame.Width / 2),
                    0,
                    (float)topImage.Frame.Width,
                    (float)topImage.Frame.Height);

                v.AddSubview(topImage);
                v.AddSubview(logo);

                window.AddSubview(v);

                window.BringSubviewToFront(v);

                UIView.Animate(0.5, () =>
                {
                    v.Alpha = 1;
                });
            }
            catch (Exception)
            {
                //No action needed
            }
        }

        public override void WillEnterForeground(UIApplication uiApplication)
        {
            try
            {
                var views = uiApplication.KeyWindow.Subviews.Length;
                uiApplication.KeyWindow.Subviews[views - 1].RemoveFromSuperview();
            }
            catch (Exception)
            {
                // doe niets, voorkomt crashes die af en toe optreden.
                // niet kunnen naspelen. Overlegd tussen RKO en KTR.
            }
            if (SystemSettingsChanged)
            {
                DependencyService.Get<INavigationService>().PopToRoot();
                SystemSettingsChanged = false;
            }
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            LocalNotificationHelper.HandleIncomingNotification((NotificationType)Convert.ToInt32(notification.AlertAction));
        }

        public override void DidRegisterUserNotificationSettings(UIApplication application, UIUserNotificationSettings notificationSettings)
        {
            DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ActivationPending);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var token = BitConverter.ToString(deviceToken.ToArray()).Replace("-", string.Empty);
            RemoteNotificationHelper.SetToken(token);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            var aps = userInfo?.ObjectForKey(new NSString("aps")) as NSDictionary;
            string message = null;

            if (aps?.ContainsKey(new NSString("alert")) == true)
            {
                if (aps.ObjectForKey(new NSString("alert")) is NSDictionary alert)
                {
                    message = alert.ObjectForKey(new NSString("body")).ToString();
                }
            }
            else
            {
                var param = userInfo?.ObjectForKey(new NSString("parameter")).ToString();
                if (!string.IsNullOrEmpty(param))
                {
                    message = param;
                }
            }

            if (string.IsNullOrEmpty(message))
                return;

            var appAlreadyActive = application != null && application.ApplicationState == UIApplicationState.Active;

            UNUserNotificationCenter.Current.GetNotificationSettings(async settings =>
            {
                if (settings.AlertSetting == UNNotificationSetting.Enabled)
                {
                    await RemoteNotificationHelper.ConfirmShowNotifications(appAlreadyActive);
                }
            });
        }
    }
}
