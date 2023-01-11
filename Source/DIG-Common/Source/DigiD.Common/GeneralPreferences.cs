// De openbaarmaking van dit bestand is in het kader van de WOO geschied en 
// dus gericht op transparantie en niet op hergebruik. In het geval dat dit 
// bestand hergebruikt wordt, is de EUPL licentie van toepassing, met 
// uitzondering van broncode waarvoor een andere licentie is aangegeven.
//
// Het archief waar dit bestand deel van uitmaakt is te vinden op:
//   https://github.com/MinBZK/woo-verzoek-broncode-digid-app
//
// Eventuele kwetsbaarheden kunnen worden gemeld bij het NCSC via:
//   https://www.ncsc.nl/contact/kwetsbaarheid-melden
// onder vermelding van "Logius, openbaar gemaakte broncode DigiD-App" 
//
// Voor overige vragen over dit WOO-verzoek kunt u mailen met:
//   mailto://open@logius.nl
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

