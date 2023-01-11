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
using System.Linq;
using System.Threading.Tasks;
using DigiD.Common.Constants;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Http.Enums;
using DigiD.Common.Interfaces;
using DigiD.Common.Mobile.Helpers;
using DigiD.Common.Models;
using DigiD.Common.Models.ResponseModels;
using DigiD.Common.Services;
using DigiD.Common.SessionModels;
using DigiD.Common.Settings;
using DigiD.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace DigiD.Helpers
{
    public static class IncomingDataHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "<Pending>")]
        public static async void HandleIncomingData(Uri uri)
        {
            HttpSession.IsWeb2AppSession = true;

            switch (uri.Scheme)
            {
                case QRCodeIdentifierConstants.Authentication:
                case QRCodeIdentifierConstants.WIDAuthentication:
                    await ScanCompletedAsync(uri, false);
                    break;
            }
        }

        internal static async Task ScanCompletedAsync(Uri uri, bool skipVerificationCode = true)
        {
            var scanResult = QRCodeHelper.ProcessScan(uri);
            App.ShowSplashScreen(uri.Scheme == QRCodeIdentifierConstants.WIDAuthentication, false);

            if (scanResult == null)
            {
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.InvalidScan);
                return;
            }

            var data = new SessionData();

            if (scanResult.Properties.ContainsKey("at"))
            {
                int.TryParse(scanResult.Properties["at"], out var seconds);

                var delta = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(seconds);
                if (delta > TimeSpan.FromMinutes(AppConfigConstants.QRCodeValidationTime))
                {
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.ScanTimeout);
                    return;
                }
            }

            data.Identifier = scanResult.Identifier;
            data.Host = scanResult.Properties.GetValue("host");
            data.WebSessionId = scanResult.Properties.GetValue("app_session_id");
            data.verification_code = scanResult.Properties.GetValue("verification_code");
            data.source = scanResult.Properties.GetValue("source");
            data.action = scanResult.Properties.GetValue("action");

            if (Device.RuntimePlatform == Device.Android && (HttpSession.IsWeb2AppSession || HttpSession.IsApp2AppSession)) //Browser is determined bij OS via intent
                HttpSession.Browser = Browser.Chrome;
            else if (Device.RuntimePlatform == Device.iOS && scanResult.Properties.ContainsKey("browser")) //for iOS we get this information via the qr-code
            {
                switch (scanResult.Properties.GetValue("browser").ToLowerInvariant())
                {
                    case "safari":
                        HttpSession.Browser = Browser.Safari;
                        break;
                    case "chrome":
                        HttpSession.Browser = Browser.Chrome;
                        break;
                    case "firefox":
                        HttpSession.Browser = Browser.Firefox;
                        break;
                    case "microsoft+edge":
                        HttpSession.Browser = Browser.Edge;
                        break;
                }

                HttpSession.SourceApplication = HttpSession.Browser.ToString();
            }

            if (HttpSession.IsWeb2AppSession && HttpSession.Browser == Browser.Unknown)
            {
                await DependencyService.Get<INavigationService>().PushAsync(new MessageViewModel(MessagePageType.BrowserUnsupported));
                return;
            }

            await ProcessData(data, false, skipVerificationCode);
        }

        private static async Task HandleApp2AppDataSuccess(SessionData data, App2AppSessionData sessionData, bool activation, InitApp2AppResponse sessionResponse)
        {
            data.WebSessionId = sessionResponse.WebSessionId;
            if (string.IsNullOrEmpty(data.WebSessionId))
            {
                await App2AppHelper.ReturnToClientApp();
                return;
            }

            if (DependencyService.Get<IMobileSettings>().ActivationStatus != ActivationStatus.Activated && !activation)
            {
                HttpSession.AppSessionId = sessionResponse.WebSessionId;
                await DependencyService.Get<IAuthenticationService>().AbortAuthentication(DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.NotActivated ? "not_activated" : "app_is_pending");
                await App2AppHelper.ReturnToClientApp();
                return;
            }

            if (!string.IsNullOrEmpty(sessionData.Data.ReturnUrl))
            {
                App2AppSession.App = sessionResponse.Apps.FirstOrDefault(x => x.Url == sessionData.Data.ReturnUrl);

                if (App2AppSession.App == null)
                {
                    HttpSession.AppSessionId = sessionResponse.WebSessionId;
                    await DependencyService.Get<IAuthenticationService>().AbortAuthentication("invalid_return_url");
                    await DependencyService.Get<INavigationService>().PopToRoot();
                    return;
                }

                App2AppSession.AppReturnUrl = sessionData.Data.ReturnUrl;
                App2AppSession.SamlArt = sessionResponse.SAMLart;
            }

            if (IsValidImage(sessionData, sessionResponse))
                App2AppSession.AppIconUrl = sessionData.Data.Icon;
        }

        private static async Task<bool> ProcessApp2AppData(SessionData data, App2AppSessionData sessionData, bool activation)
        {
            HttpSession.IsApp2AppSessionStarted = true;
            var response = await DependencyService.Get<IApp2AppService>().InitSessionApp2App(sessionData.Data);

            switch (response.ApiResult)
            {
                case ApiResult.Ok:
                    await HandleApp2AppDataSuccess(data, sessionData, activation, response);
                    return true;
                case ApiResult.Forbidden:
                    await App.CheckVersion();
                    break;
                default:
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SessionTimeout);
                    break;
            }

            return false;
        }

        private static bool IsValidImage(App2AppSessionData sessionData, InitApp2AppResponse app2AppSession)
        {
            if (Uri.TryCreate(sessionData.Data.Icon, UriKind.RelativeOrAbsolute, out var imageUri) && Uri.TryCreate(app2AppSession.ImagedDomain, UriKind.RelativeOrAbsolute, out var uri))
                return uri.Host.Equals(imageUri.Host, StringComparison.CurrentCultureIgnoreCase);

            return false;
        }

        private static async Task ProcessAuthentication(SessionData data, bool activation, bool skipVerificationCode)
        {
            if (DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.NotActivated)
            {
                HttpSession.TempSessionData = new TempSessionData(data.WebSessionId);

                if (HttpSession.IsApp2AppSession || HttpSession.IsWeb2AppSession)
                    DependencyService.Get<INavigationService>().SetPage(new LandingViewModel());
                else
                    DependencyService.Get<INavigationService>().SetPage(new ActivationIntro1ViewModel());

                DependencyService.Get<IDialog>().HideProgressDialog();
            }
            else
            {
                if (SkipVerificationValidation() || (data.verification_code != null && data.verification_code.Equals(HttpSession.VerificationCode, StringComparison.CurrentCultureIgnoreCase)))
                {
                    if (!await DependencyService.Get<IGeneralServices>().SslPinningCheck())
                    {
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.SSLException);
                        return;
                    }

                    await AuthenticationHelper.ContinueAuthenticationAsync();
                }
                else
                {
                    PiwikHelper.Track("Koppelcode fout",
                        $"Ingevoerde Koppelcode: {data.verification_code ?? "INVALID"}. Verwachte Koppelcode: {HttpSession.VerificationCode}", DependencyService.Get<INavigationService>().CurrentPageId);
                    await DependencyService.Get<IAuthenticationService>().AbortAuthentication(AbortConstants.VerificationCodeFailed);
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.VerificationCodeFailed);
                }

                bool SkipVerificationValidation()
                {
                    return HttpSession.Browser != Browser.Unknown || HttpSession.IsApp2AppSession || activation || skipVerificationCode;
                }
            }
        }

        internal static async Task ProcessData(SessionData data, bool activation, bool skipVerificationCode)
        {
            try
            {
                var host = !string.IsNullOrEmpty(data.Host) ? data.Host : "digid.nl";

                if (AppConfigConstants.HostWhiteList.All(x => x.ToString() != host))
                {
                    await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.InvalidScan);
                    return;
                }

#if ENVIRONMENT_SELECTABLE
                if (!DependencyService.Get<IDemoSettings>().IsDemo)
                    DependencyService.Get<IGeneralPreferences>().SelectedHost = host;
#endif

                if (HttpSession.IsApp2AppSession && !HttpSession.IsApp2AppSessionStarted && data is App2AppSessionData sessionData)
                {
                    var isSuccess = await ProcessApp2AppData(data, sessionData, activation);

                    if (!isSuccess)
                        return;
                }

                if (AppSession.AppMode == AppMode.Demo && !HttpSession.IsApp2AppSession && string.IsNullOrEmpty(data.action))
                {
                    DependencyService.Get<IDialog>().HideProgressDialog();

                    var actions = new List<string>()
                    {
                        AuthenticationActions.ChangePhoneNumber, AuthenticationActions.ActivateSMSAuthentication,
                        AuthenticationActions.ChangePassword, AuthenticationActions.CANCEL_ACCOUNT,
                        AuthenticationActions.DeactivateApp, AuthenticationActions.EMAIL_ADD,
                        AuthenticationActions.EMAIL_CHANGE, AuthenticationActions.EMAIL_REMOVE
                    };

                    if (data.Identifier == QRCodeIdentifierConstants.WIDAuthentication)
                        actions.Add(AuthenticationActions.ACTIVATE_IDENTITY_CARD);

                    var action = await Application.Current.MainPage.DisplayActionSheet("Wat wil je doen?", null, "Webdienst inloggen", actions.ToArray());

                    var loginLevel = LoginLevel.Midden;

                    if (action == "Webdienst inloggen")
                    {
                        action = "webservice";
                        if (data.Identifier != QRCodeIdentifierConstants.WIDAuthentication)
                            loginLevel = Enum.Parse<LoginLevel>(await Application.Current.MainPage.DisplayActionSheet("Welk inlogniveau?", null, LoginLevel.Basis.ToString(), LoginLevel.Midden.ToString(), LoginLevel.Substantieel.ToString()));
                    }

                    DemoHelper.NewSession(action, data.WebSessionId, loginLevel);
                }

                await Task.Delay(200);  // kleine pauze, lijkt noodzakelijk voor web2app.

                if (Device.RuntimePlatform == Device.iOS)
                    DependencyService.Get<IHttpMessageHandlerService>().RemoveNativeCookies();

                switch (data.Identifier)
                {
                    case QRCodeIdentifierConstants.WIDAuthentication:
                        App.ShowSplashScreen(true, false);
                        HttpSession.AppSessionId = data.WebSessionId;
                        await WidHelper.AuthenticateAsync(data);
                        break;
                    case QRCodeIdentifierConstants.Authentication:
                        if (data.action != null || DependencyService.Get<IMobileSettings>().ActivationStatus == ActivationStatus.NotActivated)
                            await ProcessAuthentication(data, activation, skipVerificationCode);
                        else
                        {
                            await SessionHelper.StartSession(async () =>
                            {
                                HttpSession.AppSessionId = data.WebSessionId;
                                await ProcessAuthentication(data, activation, skipVerificationCode);
                            });
                        }
                        break;
                    default:
                        await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.InvalidScan);
                        break;
                }
            }
            catch (Exception e)
            {
                var properties = new Dictionary<string, string> { { "data", JsonConvert.SerializeObject(data) } };
                AppCenterHelper.TrackError(e, properties);
                await DependencyService.Get<INavigationService>().ShowMessagePage(MessagePageType.LoginUnknown);
            }
        }
    }
}
