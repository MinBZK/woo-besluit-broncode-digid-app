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
﻿using System.Collections.Generic;
using DigiD.Common;
using DigiD.Common.Enums;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Settings;
using Microsoft.AppCenter;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using MenuItem = DigiD.Models.MenuItem;

namespace DigiD.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public bool PiwikEnabled { get; set; }

        private readonly IGeneralPreferences _generalSettings = DependencyService.Get<IGeneralPreferences>();

        public SettingsViewModel()
        {
            PageId = "AP068";
            HeaderText = AppResources.AppMenuSettings;
            PiwikEnabled = _generalSettings.PiwikTrackEnabled;
            HasBackButton = true;

            LoadItems();
        }

        public AsyncCommand<bool> SwitchChangedCommand => new AsyncCommand<bool>(async (value) =>
        {
            if (!value)
                PiwikHelper.Track("Preferences", "Piwik Analytics disabled", NavigationService.CurrentPageId);

            _generalSettings.PiwikTrackEnabled = value;

            await AppCenter.SetEnabledAsync(_generalSettings.PiwikTrackEnabled);
        });

        private void LoadItems()
        {
            MenuItems.Add(new MenuItem
            {
                Title = AppResources.Language,
                TargetViewModel = typeof(SettingsLanguageViewModel),
                Icon = "resource://DigiD.Common.Resources.icon_taal_selectie.svg?assembly=DigiD.Common"
            });

            MenuItems.Add(new MenuItem
            {
                Title = AppResources.AppThemeMenuText,
                TargetViewModel = typeof(SettingsAppThemeViewModel),
                Icon = "resource://DigiD.Resources.icon_instellingen_switch_theme.svg"
            });

            if (DependencyService.Get<IMobileSettings>().ActivationStatus != ActivationStatus.NotActivated)
            {
                MenuItems.Add(new MenuItem
                {
                    Title = AppResources.AppMenuDeactiveren,
                    Icon = "resource://DigiD.Resources.digid_icon_menu_app_deactiveren.svg",
                    TargetViewModel = typeof(DeactivationViewModel)
                });
            }
        }
    }
}
