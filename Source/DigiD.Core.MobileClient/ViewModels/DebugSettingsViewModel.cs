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
﻿#if ENVIRONMENT_SELECTABLE
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DigiD.Common.Constants;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class DebugSettingsViewModel : BaseViewModel
    {

        public List<string> AvailableHosts { get; set; }
        public string SelectedHost { get; set; }
        public AsyncCommand WhatsNewCommand => new AsyncCommand(async () => await WhatsNewHelper.Show());

        public DebugSettingsViewModel()
        {
            HasBackButton = true;
            PageId = "DBGS";
            HasBackButton = true;

            AvailableHosts = AppConfigConstants.HostWhiteList.Select(h => h.ToString()).OrderBy(x => x).ToList();
            SelectedHost = DependencyService.Get<IGeneralPreferences>().SelectedHost;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedHost))
            {
                DependencyService.Get<IGeneralPreferences>().SelectedHost = SelectedHost;
            }
        }
    }
}
#endif
