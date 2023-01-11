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
﻿using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Settings;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class SettingsAppThemeViewModel : BaseViewModel
    {
        public bool IsInDarkMode { get; set; } = Application.Current.UserAppTheme == OSAppTheme.Dark;
        public bool IsInLightMode { get; set; } = Application.Current.UserAppTheme == OSAppTheme.Light;
        public bool IsInAutomaticMode { get; set; } = Application.Current.UserAppTheme == OSAppTheme.Unspecified;

        public string DarkModeAccessibilityText => string.Format(IsInDarkMode ? AppResources.AccessibilityItemSelected : AppResources.AccessibilityItemNotSelected, AppResources.DarkmodeOnButtonText);
        public string LightModeAccessibilityText => string.Format(IsInLightMode ? AppResources.AccessibilityItemSelected : AppResources.AccessibilityItemNotSelected, AppResources.DarkmodeOffButtonText);
        public string AutomaticAccessibilityText => string.Format(IsInAutomaticMode ? AppResources.AccessibilityItemSelected : AppResources.AccessibilityItemNotSelected, AppResources.DarkmodeAutomaticButtonText);

        public AsyncCommand<OSAppTheme> SetAppThemeCommand
        {
            get
            {
                return new AsyncCommand<OSAppTheme>(async appTheme =>
                {
                    DialogService.ShowProgressDialog();

                    await Task.Delay(100);

                    Application.Current.UserAppTheme = appTheme;
                    DependencyService.Get<IGeneralPreferences>().AppTheme = Application.Current.UserAppTheme;
                    IsInDarkMode = Application.Current.UserAppTheme == OSAppTheme.Dark;
                    IsInLightMode = Application.Current.UserAppTheme == OSAppTheme.Light;
                    IsInAutomaticMode = Application.Current.UserAppTheme == OSAppTheme.Unspecified;

                    DialogService.HideProgressDialog();
                });
            }
        }

        public SettingsAppThemeViewModel()
        {
            HasBackButton = true;
            PageId = "AP097";
            NavCloseCommand = new AsyncCommand(async () => { await NavigationService.PopToRoot(); });
        }
    }
}
