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
﻿using System.Globalization;
using System.Threading.Tasks;
using DigiD.Common;
using DigiD.Common.Mobile.BaseClasses;
using DigiD.Common.Settings;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace DigiD.ViewModels
{
    public class SettingsLanguageViewModel : BaseViewModel
    {
        public bool IsEnglish { get; set; }
        public bool IsDutch { get; set; }

        //tbv accessibility
        public string EnglishAccessibilityText => string.Format(IsEnglish ? AppResources.AccessibilityItemSelected : AppResources.AccessibilityItemNotSelected, AppResources.English);
        public string DutchAccessibilityText => string.Format(IsDutch ? AppResources.AccessibilityItemSelected : AppResources.AccessibilityItemNotSelected, AppResources.Dutch);

        private readonly IGeneralPreferences _generalSettings = DependencyService.Get<IGeneralPreferences>();

        public SettingsLanguageViewModel()
        {
            HasBackButton = true;
            PageId = "AP096";
            HeaderText = AppResources.Language;

            IsEnglish = _generalSettings.Language == "en";
            IsDutch = _generalSettings.Language == "nl";

            NavCloseCommand = new AsyncCommand(async () => { await NavigationService.PopToRoot(); });
        }

        public AsyncCommand<string> SelectLanguageCommand
        {
            get
            {
                return new AsyncCommand<string>(async language =>
                {
                    DialogService.ShowProgressDialog();

                    await Task.Delay(100);

                    _generalSettings.Language = language;
                    
                    Localization.Init(CultureInfo.GetCultureInfo(language));
                    
                    await NavigationService.ResetMainPage(new SettingsViewModel(), new SettingsLanguageViewModel());
                    
                    DialogService.HideProgressDialog();
                });
            }
        }
    }
}
