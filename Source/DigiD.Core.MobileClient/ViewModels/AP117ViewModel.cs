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
using DigiD.Common.Mobile.BaseClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using MenuItem = DigiD.Models.MenuItem;

namespace DigiD.ViewModels
{
    public class AP117ViewModel : BaseViewModel
    {
        public List<MenuItem> MenuItems { get; set; }

        public AP117ViewModel()
        {
            PageId = "AP117";
            NavCloseCommand = new AsyncCommand(async () => { await NavigationService.PopToRoot(); });
            HeaderText = AppResources.AP117_Header;
            HasBackButton = true;

            MenuItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Title = AppResources.AP117_MenuItem1_Name,
                    Icon = "resource://DigiD.Resources.icon_menu_externe_link.svg",
                    Action = async () => { await Browser.OpenAsync(AppResources.AP117_MenuItem1_Value); },
                    IsExternalLink = true
                },
                new MenuItem
                {
                    Title = AppResources.AP117_MenuItem2_Name,
                    Icon = "resource://DigiD.Resources.icon_menu_externe_link.svg",
                    Action = async () => { await Browser.OpenAsync(AppResources.AP117_MenuItem2_Value); },
                    IsExternalLink = true
                }
            };
        }
    }
}
