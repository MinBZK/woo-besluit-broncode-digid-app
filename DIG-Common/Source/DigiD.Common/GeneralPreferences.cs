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
using DigiD.Common;
using DigiD.Common.Helpers;
using DigiD.Common.Services;
using DigiD.Common.Settings;
using Xamarin.Forms;

[assembly: Dependency(typeof(GeneralPreferences))]
namespace DigiD.Common
{
    public class GeneralPreferences : IGeneralPreferences
    {
        public GeneralPreferences()
        {

        }

#if ENVIRONMENT_SELECTABLE
        public string SelectedHost
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(SelectedHost), "SSSSSSSSSSS");
            set => Xamarin.Essentials.Preferences.Set(nameof(SelectedHost), value);
        }
#else
#if PROD
        public string SelectedHost => "digid.nl";
#elif WCAG
        public string SelectedHost => "SSSSSSSSSSS";
#else
        public string SelectedHost => "SSSSSSSSSSSSSSSSS";
#endif
#endif

        public int NumberOfSuccessfulLogins
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(NumberOfSuccessfulLogins), 0);
            set => Xamarin.Essentials.Preferences.Set(nameof(NumberOfSuccessfulLogins), value);
        }

        public bool PiwikTrackEnabled
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(PiwikTrackEnabled), true);
            set => Xamarin.Essentials.Preferences.Set(nameof(PiwikTrackEnabled), value);
        }

        public string Language
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(Language), DependencyService.Get<IDevice>().DefaultLanguage);
            set => Xamarin.Essentials.Preferences.Set(nameof(Language), value);
        }

        public OSAppTheme AppTheme
        {
            get
            {
                var defaultAppTheme = ThemeHelper.IsAutomaticAppThemePossible ? OSAppTheme.Unspecified : OSAppTheme.Light;

                if (Xamarin.Essentials.Preferences.ContainsKey(nameof(AppTheme)))
                {
                    var value = Xamarin.Essentials.Preferences.Get(nameof(AppTheme), defaultAppTheme.ToString());
                    if (Enum.TryParse<OSAppTheme>(value, out var result))
                        return result;
                }

                return defaultAppTheme;
            }
            set => Xamarin.Essentials.Preferences.Set(nameof(AppTheme), value.ToString());
        }

        public string WhatsNewVersion
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(WhatsNewVersion), "");
            set => Xamarin.Essentials.Preferences.Set(nameof(WhatsNewVersion), value);
        }

        public string InstanceId
        {
            get
            {
                var storedValue = DependencyService.Get<IKeyStore>().FindValueForKey(nameof(InstanceId));
#if DEBUG
                if (storedValue != null && storedValue.Equals("request_account", StringComparison.InvariantCultureIgnoreCase) ||
                    storedValue != null && storedValue.StartsWith("u_", StringComparison.InvariantCultureIgnoreCase))
                    storedValue = null;
#endif

                if (string.IsNullOrEmpty(storedValue))
                {
                    storedValue = Guid.NewGuid().ToString();
                    DependencyService.Get<IKeyStore>().Insert(storedValue, nameof(InstanceId));
                    DependencyService.Get<IKeyStore>().Save();
                }

                return storedValue;
            }
        }
    }
}

