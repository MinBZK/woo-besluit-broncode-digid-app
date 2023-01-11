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
using System.Threading.Tasks;
using DigiD.Common.Helpers;
using DigiD.Common.Mobile.Helpers;
using DigiD.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DigiD.UI.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuPopup : Popup
    {
        const uint Velocity = 250;

        public double MenuWidth => DisplayHelper.Width;
        public double MenuHeight => DisplayHelper.Height;

        public MainMenuPopup()
        {
            ResizeMenu(OrientationHelper.IsInLandscapeMode);

            InitializeComponent();
            BindingContext = new MainMenuViewModel();

            Opened += OpenPopup;

            IsLightDismissEnabled = true;

            DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;
        }

        private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            ResizeMenu(e.DisplayInfo.Orientation == DisplayOrientation.Landscape);
        }

        private void ResizeMenu(bool isInLandscapeMode)
        {
            Size = new Size(isInLandscapeMode ? MenuWidth * .45 : MenuWidth * .8, MenuHeight);
        }

        protected override async void LightDismiss()
        {
            await ClosePopup();
            base.LightDismiss();
        }

        public void Close()
        {
            LightDismiss();
        }

        private async Task ClosePopup()
        {
            await popupGrid.TranslateTo(-MenuWidth, 0, Velocity, Easing.Linear);
        }

        async void OpenPopup(object sender, PopupOpenedEventArgs e)
        {
            await popupGrid.TranslateTo(0, 0, Velocity, Easing.Linear);
        }

        private void PopupGrid_OnClose(object sender, EventArgs e)
        {
            LightDismiss();
        }
    }
}
