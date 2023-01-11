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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.OS;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Droid.Services;
using Java.Lang;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

[assembly: Dependency(typeof(DeviceService))]
namespace DigiD.Droid.Services
{
    internal class DeviceService : IDevice
    {
        public int PiwikId => App.PiwikId;

        public bool IsScreenCaptured => false;

        public void OpenSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);
            intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, Xamarin.Essentials.Platform.CurrentActivity.PackageName);

            Xamarin.Essentials.Platform.CurrentActivity.StartActivityForResult(intent, 0);
        }

        public bool OpenBrowser(string name, string uri)
        {
            try
            {
                uri += $"?random={Guid.NewGuid()}";
                var intent = new Intent(Intent.ActionView, Uri.Parse(uri));
                intent.SetPackage(name);
                Xamarin.Essentials.Platform.CurrentActivity.StartActivity(intent);
                return true;
            }
            catch (System.Exception e)
            {
                AppCenterHelper.TrackError(e, new Dictionary<string, string>
                {
                    {"Name",name }
                });
                return false;
            }
        }

        public string RuntimePlatform => Device.RuntimePlatform;

        public string DeviceTypeName => Device.Idiom == TargetIdiom.Tablet ? "tablet" : "smartphone";

        public string DefaultLanguage => CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;

        public Task<string> UserAgent()
        {
            return Task.FromResult(JavaSystem.GetProperty("http.agent"));
        }

        public void CloseApp()
        {
            throw new NotImplementedException();
        }

        public bool HasHomeButton => throw new NotImplementedException();

        // de app zal niet goed werken indien in 'Developer Mode', dit wordt via de 'Supportcode' teruggegeven
        // richting Helpdesk zodat zij de gebruiker kunnen adviseren om deze optie uit te zetten op hun device.
        public bool IsInDeveloperMode
        {
            get
            {
                // https://stackoverflow.com/questions/63348116/foolproof-way-to-detect-if-developer-options-are-enabled
                var context = Xamarin.Essentials.Platform.CurrentActivity.ApplicationContext;

                var isSettingEnabled = Android.Provider.Settings.Global.GetInt(context.ContentResolver,
                    Android.Provider.Settings.Global.DevelopmentSettingsEnabled, Build.Type.Equals("eng") ? 1 : 0) != 0;

                return isSettingEnabled;
            }
        }

        public Task<bool> AskForLocalNotificationPermission()
        {
            return Task.FromResult(true);
        }

        public SystemFontSize GetSystemFontSize()
        {
            var scale = Xamarin.Essentials.Platform.CurrentActivity.Resources.Configuration.FontScale;
            var result = SystemFontSize.L;
            switch (scale)
            {
                case 0.8f:
                    result = SystemFontSize.XS;
                    break;
                case 0.9f:
                    result = SystemFontSize.S;
                    break;
                case 1.0f:
                    result = SystemFontSize.M;
                    break;
                case 1.1f:
                    result = SystemFontSize.L;
                    break;
                case 1.3f:
                    result = SystemFontSize.XL;
                    break;
                case 1.5f:
                    result = SystemFontSize.XXL;
                    break;
                case 1.7f:
                    result = SystemFontSize.XXXL;
                    break;
                case 2.0f:  // 200%!!
                    result = SystemFontSize.ExtraM;
                    break;
            }
            return result;
        }
    }
}
