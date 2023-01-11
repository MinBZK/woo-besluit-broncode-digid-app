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
﻿using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Settings;
using DigiD.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class AP116ViewModel : BaseViewModel
    {
        public string AppVersionNumber => $"{AppInfo.VersionString} ({AppInfo.BuildString})";
        public string AppCode => $"{DependencyService.Get<IGeneralPreferences>().InstanceId[..6].ToUpperInvariant()}";
        public string PlatformVersion => $"{Device.RuntimePlatform} {DeviceInfo.VersionString}";

        public string SupportCode
        {
            get
            {
                var vi = AppInfo.Version;
                var dv = DeviceInfo.Version;
                var osVersion = $"{dv.Major,3}{dv.Minor,1}{(dv.Build > 0 ? $"{dv.Build,2}" : "00")}";
                var tmp = $"-{osVersion.Substring(0, 4)}-{osVersion.Substring(4)}{vi.Major,2}-{vi.Minor,2}{(vi.Build > 0 ? $"{vi.Build,2}": "00")}-{AppInfo.BuildString,4}";
                tmp = tmp.Replace(' ', '0');    // eerst de spaties vervangen door voorloopnullen indien nodig.
                
                return SupportCodeHelper.GenerateSupportCode() + tmp.Replace("-", " - "); // tussen de nibbles van 4 bits een spatie rondom het streepje.
            }
        }

        public AP116ViewModel()
        {
            HasBackButton = true;
            PageId = "AP116";

            NavCloseCommand = new AsyncCommand(async () => { await NavigationService.PopToRoot(); });
        }
    }
}
