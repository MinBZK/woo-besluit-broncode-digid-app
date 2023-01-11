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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DigiD.Api;
using DigiD.Common;
using DigiD.Common.Api;
using DigiD.Common.BaseClasses;
using DigiD.Common.EID.Interfaces;
using DigiD.Common.EID.Services;
using DigiD.Common.EID.SessionModels;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.NFC.Interfaces;
using DigiD.Common.RDA.Api;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.Common.ViewModels;
using DigiD.Helpers;
using DigiD.Interfaces;
using DigiD.Services;
using DigiD.Themes;
using DigiD.UI.Pages;
using DigiD.UI.Popups;
using DigiD.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using Browser = DigiD.Common.Enums.Browser;
using Device = Xamarin.Forms.Device;

[assembly: ExportFont("RijksoverheidSansWebText-Bold.ttf", Alias = "RO-Bold")]
[assembly: ExportFont("RijksoverheidSansWebText-Italic.ttf", Alias = "RO-Italic")]
[assembly: ExportFont("RijksoverheidSansWebText-Regular.ttf", Alias = "RO-Regular")]
[assembly: ExportFont("FontAwesomeRegular.otf", Alias = "FontAwesomeRegular")]
[assembly: ExportFont("FontAwesomeSolid.otf", Alias = "FontAwesomeSolid")]
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DigiD
{
    public partial class App : Xamarin.Forms.Application
    {
        public static bool HasNfc { get; private set; }
        public static string NewNotificationToken { get; set; }

        public static double PopupWidth
        {
            get
            {
                if (Device.Idiom == TargetIdiom.Phone)
                    return DisplayHelper.Width - 40;

                if (OrientationHelper.IsInLandscapeMode)
                    return DisplayHelper.Width * .6;

                return DisplayHelper.Width * .8;
            }
        }

        public static double PopupHeight
        {
            get
            {
                if (OrientationHelper.IsInLandscapeMode && Device.Idiom == TargetIdiom.Phone)
                    return DisplayHelper.Height - 60;

                return DisplayHelper.Height * .8;

            }
        }

        public static bool IsAppInBackground { get; set; }

        public static int PiwikId
        {
            get
            {
#if PROD
                return DependencyService.Get<IDemoSettings>().IsDemo ? 41 : 18;
#else
                return DependencyService.Get<IDemoSettings>().IsDemo ? 42 : 3;
#endif
            }
        }

        
        internal static bool IsNewActivated { get; set; }
        internal static ConfigResponse Configuration { get; set; }
        internal static AppServiceResponse Apps { get; set; }

        private NoConnectivityPopup _popup;

#if DEBUG
        private void SetDebugView()
        {
#if A11YTEST
            var helper = new A11YTestHelper(false);

            MessagingCenter.Subscribe<object>(this, "A11YTEST_NEXT", async (sender) =>
            {
                await helper.Next();
            }); 

            await helper.Start();
#else
            Resources.Add(new Style(typeof(ContentPage))
            {
                ApplyToDerivedTypes = true,
                Setters = {
                    new Setter
                    {
                        Property = Xamarin.Forms.DebugRainbows.DebugRainbow.ShowColorsProperty,
                        Value = false //Turn on or off
                    }
                }
            });
#endif
        }
#endif

        private void PreventLinkerFromStrippingCommonLocalizationReferences()
        {
            _ = new ChineseLunisolarCalendar();
            _ = new GregorianCalendar();
            _ = new HebrewCalendar();
            _ = new HijriCalendar();
            _ = new JapaneseCalendar();
            _ = new JapaneseLunisolarCalendar();
            _ = new JulianCalendar();
            _ = new KoreanCalendar();
            _ = new KoreanLunisolarCalendar();
            _ = new PersianCalendar();
            _ = new TaiwanCalendar();
            _ = new ThaiBuddhistCalendar();
            _ = new UmAlQuraCalendar();
        }

        public App()
        {
            PreventLinkerFromStrippingCommonLocalizationReferences();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetEnableAccessibilityScalingForNamedFontSizes(true);

            IsAppInBackground = false;

            Current.RequestedThemeChanged += (sender, args) => SetAppTheme();
            Current.UserAppTheme = DependencyService.Get<IGeneralPreferences>().AppTheme;

            //Theme must always be set. In case of system is set to darkmode, and the app also, the RequestedThemeChanged is not fired
            SetAppTheme();

            InitializeComponent();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"); //20.2.0.36

            DependencyService.Register<INavigationService, NavigationService>();
            DependencyService.Register<IPopupService, PopupService>();

            RegisterServices(DependencyService.Get<IDemoSettings>().IsDemo ? AppMode.Demo : AppMode.Normal);
            
#if !SSL_UNPINNING
            DependencyService.Register<ISslPinningService, SslPinningRoot>();
#else
            DependencyService.Register<ISslPinningService, SSLPinningDisabled>();
#endif

#if !DEBUG

            AppCenter.Start($"SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS" +
                            "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"
                , typeof(Microsoft.AppCenter.Analytics.Analytics)
                , typeof(Microsoft.AppCenter.Crashes.Crashes));

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("App Started");
            AppCenter.SetUserId(DependencyService.Get<IGeneralPreferences>().InstanceId);
#endif

            Localization.Init(CultureInfo.GetCultureInfo(DependencyService.Get<IGeneralPreferences>().Language));
            
            AsyncUtil.RunSync(GetActualConfiguration);
            Apps = AsyncUtil.RunSync(() => DependencyService.Get<IGeneralServices>().GetServices());

            var app = Apps.Services.FirstOrDefault(x => x.Name == Common.Constants.DemoConstants.MijnDigidAppName);
            if (app != null)
                AppSession.MyDigiDUrl = app.Url;

            App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

#if DEBUG
            SetStartPage();
            SetDebugView();
#else
            if (DependencyService.Get<ISecurityService>().IsRunningOnVirtualDevice)
                Current.MainPage = new MessagePage { BindingContext = new MessageViewModel(MessagePageType.DeviceNotSupported) };
            else
                SetStartPage();
#endif
        }

        private static void SetStartPage()
        {
            AppSession.IsAppActivated = DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated;
            
            if (!AppSession.IsAppActivated && DependencyService.Get<IMobileSettings>().ActivationStatus != ActivationStatus.Pending)
                Current.MainPage =
                    new CustomNavigationPage(
                        DependencyService.Get<INavigationService>().GetPage(new LandingViewModel()));
            else
                Current.MainPage = new SplashPage(true);
        }

        internal static void RegisterServices(AppMode mode)
        {
            if (AppSession.AppMode == mode)
                return;

            switch (mode)
            {
                case AppMode.Normal:
                    {
                        AppSession.AppMode = AppMode.Normal;
                        DependencyService.Register<IRdaServices, RdaServices>();
                        DependencyService.Register<IEnrollmentServices, EnrollmentServices>();
                        DependencyService.Register<IGeneralServices, GeneralServices>();
                        DependencyService.Register<IVersionService, VersionService>();
                        DependencyService.Register<IChangePinService, ChangePinServiceServices>();
                        DependencyService.Register<IRequestStationServices, RequestStationServices>();
                        DependencyService.Register<IEmailService, EmailService>();
                        DependencyService.Register<IMessageCenterServices, MessageCenterServices>();
                        DependencyService.Register<IRemoteNotificationServices, RemoteNotificationServices>();
                        DependencyService.Register<IUsageHistoryService, UsageHistoryService>();
                        DependencyService.Register<IAccountInformationServices, AccountInformationServices>();
                        DependencyService.Register<IEIDOperationService, EIDOperationService>();
                        DependencyService.Register<IEIDServices, EIDServices>();
                        DependencyService.Register<IAuthenticationService, AuthenticationService>();
                        DependencyService.Register<IApp2AppService, App2AppService>();
                        break;
                    }
                case AppMode.Demo:
                    {
                        AppSession.AppMode = AppMode.Demo;
                        DependencyService.Register<IVersionService, Common.Api.Demo.VersionService>();
                        DependencyService.Register<IEnrollmentServices, Api.Demo.EnrollmentService>();
                        DependencyService.Register<IRequestStationServices, Api.Demo.RequestStationServices>();
                        DependencyService.Register<IGeneralServices, Common.Api.Demo.GeneralService>();
                        DependencyService.Register<IEmailService, Api.Demo.EmailService>();
                        DependencyService.Register<IRemoteNotificationServices, Api.Demo.RemoteNotificationServices>();
                        DependencyService.Register<IChangePinService, Api.Demo.ChangePinServiceServices>();
                        DependencyService.Register<IMessageCenterServices, Api.Demo.MessageCenterServices>();
                        DependencyService.Register<IRdaServices, Common.RDA.Api.Demo.RdaServices>();
                        DependencyService.Register<IUsageHistoryService, Api.Demo.UsageHistoryService>();
                        DependencyService.Register<IAccountInformationServices, Api.Demo.AccountInformationServices>();
                        DependencyService.Register<IEIDOperationService, Common.EID.Demo.EIDOperationService>();
                        DependencyService.Register<IEIDServices, Common.Api.Demo.EIDServices>();
                        DependencyService.Register<IAuthenticationService, Api.Demo.AuthenticationService>();
                        DependencyService.Register<IApp2AppService, Api.Demo.App2AppService>();
                        break;
                    }
            }
        }

        protected override async void OnStart()
        {
            base.OnStart();

            DeviceLockService.DeviceLocked += async (sender, args) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    if (MainPage is CustomNavigationPage { CurrentPage: LandingPage lp })
                    {
                        await lp.CloseMenu();
                    }

                    await SessionHelper.StartSession(async () =>
                    {
                        await Task.Delay(0);
                        DependencyService.Get<INavigationService>().SetPage(new LandingViewModel());
                    });
                });
            };

#if !DEBUG
            if (DependencyService.Get<ISecurityService>().IsDebuggerAttached)
            {
                Microsoft.AppCenter.Analytics.Analytics.TrackEvent("debugger_attached", new System.Collections.Generic.Dictionary<string, string>
                {
                    {"flow","app-startup"},
                    {"instance_id",DependencyService.Get<IGeneralPreferences>().InstanceId}
                });
            }
#endif


            if (HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession)
                ShowSplashScreen(false, false);

            HasNfc = await DependencyService.Get<INfcService>().HasNFCSupport();

            DependencyService.Get<IDialog>().ShowProgressDialog();
            await DependencyService.Get<IA11YService>().Speak("Opstartscherm met Rijksoverheid en DigiD logo en tekst 'Vertrouwd inloggen met DigiD'", 500);

            await AppCenter.SetEnabledAsync(DependencyService.Get<IGeneralPreferences>().PiwikTrackEnabled);
            await CheckVersion();

            DependencyService.Get<IDialog>().HideProgressDialog();

            EIDSession.Init(DependencyService.Get<INfcService>(), HttpHelper.GetClient(), Device.Idiom == TargetIdiom.Desktop || Device.Idiom == TargetIdiom.Unsupported);
            SetNoConnectionPopup();
        }

        protected override void OnResume()
        {
            base.OnResume();

            IsAppInBackground = false;

            if (HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession)
                ShowSplashScreen(false, false);

            SetNoConnectionPopup();

            if (Device.RuntimePlatform == Device.iOS)
            {
                try
                {
                    MessagingCenter.Send(this, "OnResume");
                }
                catch (Exception)
                {
                    //Do Nothing
                }
            }
        }

        private void SetNoConnectionPopup()
        {
            if (_popup is { IsOpened: true })
                return;

            _popup = new NoConnectivityPopup();

            if (AppSession.AppMode != AppMode.Demo && Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                try
                {
                    DependencyService.Get<INavigationService>().OpenPopup(_popup);
                }
                catch (Exception e)
                {
                    //this crash will occur only on Android while app is resumed from background
                    Crashes.TrackError(e, new Dictionary<string, string>
                    {
                        {"Network",Connectivity.NetworkAccess.ToString()}
                    });
                }
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            IsAppInBackground = true;
        }

        public static void ShowSplashScreen(bool showAlways, bool validateSession)
        {
            var pages = new List<string>
            {
                "AP001",
                "AP017a",
                "AP017b"
            };

            Console.WriteLine($"CurrentPageId: {DependencyService.Get<INavigationService>().CurrentPageId}");

            if (!pages.Contains(DependencyService.Get<INavigationService>().CurrentPageId) && !showAlways)
                return;

            Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage = new SplashPage(validateSession);
            });
        }

        private void SetAppTheme()
        {
            if (ThemeHelper.IsInDarkMode)
                Resources = new DarkTheme();
            else
                Resources = new LightTheme();
        }

        internal static async Task GetActualConfiguration()
        {
            Configuration = await DependencyService.Get<IGeneralServices>().GetConfig();
        }

        /// <summary>
        /// Will check the version of the application and platform.
        /// </summary>
        /// <returns>True if new page is shown, else false.</returns>
        internal static async Task CheckVersion()
        {
            var response = await DependencyService.Get<IVersionService>().CheckVersion();

            if (response.ApiResult == ApiResult.Ok)
            {
                if (response.Action == "active")
                    return;

                var ignore = Current.MainPage is CustomNavigationPage nav && nav.IgnoreVersionCheck;

                switch (response.VersionAction)
                {
                    case ApiResult.HostNotReachable:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.NoInternetConnection);
                        break;
                    case ApiResult.SSLPinningError:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SSLException);
                        break;
                    case ApiResult.ForceUpdate:
                        {
                            if (!ignore)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    var page = DependencyService.Get<INavigationService>().GetPage(new KillSwitchViewModel(response));
                                    Current.MainPage = new CustomNavigationPage(page, true);
                                });
                            }

                            break;
                        }
                    default:
                        {
                            if (!ignore)
                            {
                                await DependencyService.Get<INavigationService>().PushModalAsync(new KillSwitchViewModel(response));
                            }

                            break;
                        }
                }
            }
        }

        internal static async Task CancelSession(bool byUser, Func<Task> completeAction = null)
        {
            if (HttpSession.AppSessionId == null)
            {
                if (completeAction != null)
                    await completeAction.Invoke();

                return;
            }

            await DependencyService.Get<IGeneralServices>().Cancel(byUser);

            if (HttpSession.IsApp2AppSession)
            {
                //if not activated and user cancelled activation flow, 'not_activated' will be returned to client.
                var errorMessage = byUser ? "cancelled_by_user" : null;

                AppSession.IsAppActivated = DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.Activated;
                if (!AppSession.IsAppActivated)
                    errorMessage = "not_activated";

                await App2AppHelper.ReturnToClientApp(errorMessage);
            }
            else
            {
                ClearSession();

                if (completeAction != null)
                    await completeAction.Invoke();
            }
        }

        internal static void ClearSession()
        {
            HttpSession.WebService = null;
            HttpSession.AppSessionId = null;
            HttpSession.TempSessionData = null;
            HttpSession.ActivationSessionData = null;
            HttpSession.Browser = Browser.Unknown;
            HttpSession.IsApp2AppSession = false;
            HttpSession.IsWeb2AppSession = false;
            HttpSession.IsApp2AppSessionStarted = false;
            HttpSession.RDASessionData = null;
            HttpSession.SourceApplication = null;

            AppSession.Process = Process.NotSet;
            AppSession.ManagementAction = null;
            App2AppSession.AppIconUrl = null;
            EIDSession.Card = null;
        }

        public static void Reset()
        {
            DependencyService.Get<IMobileSettings>().Reset();
            AppSession.IsAppActivated = false;
            DependencyService.Get<IPreferences>().Reset();
            DependencyService.Get<IDemoSettings>().Reset();
        }
    }
}
