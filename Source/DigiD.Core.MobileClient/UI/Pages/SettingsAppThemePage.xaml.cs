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
using DigiD.Common.Services;
using DigiD.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsAppThemePage : BaseContentPage
    {
        public SettingsAppThemePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (DependencyService.Get<IA11YService>().IsInVoiceOverMode() && BindingContext is SettingsAppThemeViewModel vm)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var selectedItem = darkmodeOffButton;
                    if (vm.IsInAutomaticMode)
                        selectedItem = automaticButton;
                    else if (vm.IsInDarkMode)
                        selectedItem = darkmodeOnButton;

                    selectedItem.Focus();
                    var tts = $"{header.Text} {AutomationProperties.GetName(selectedItem) ?? ""}";
                    await DependencyService.Get<IA11YService>().Speak(tts, 2000);
                });
            }
        }
    }
}
